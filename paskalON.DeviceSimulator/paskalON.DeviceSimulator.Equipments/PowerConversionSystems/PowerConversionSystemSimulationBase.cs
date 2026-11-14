// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems
{
    /// <summary>
    /// Vendor independent simulation of a PCS with its battery banks.
    /// </summary>
    /// <remarks>
    /// The PCS starts stopped (Off) with 0 active and reactive power. When started it follows its setpoints limited
    /// by the available power of the connected battery banks and splits its active power across these battery banks
    /// in proportion to their usable capacity. An injected fault puts the PCS into the fault state (no power, AC breaker
    /// open). Commands are rejected while the fault is active. After the fault is cleared the PCS stays in the fault state
    /// until the controller writes the next start, stop or standby command.
    /// The power response can be made non-ideal: a dead time delays the setpoints, a first-order lag (time constant)
    /// lets the power settle exponentially and a ramp limits the change per second. All of them are 0 (ideal) by default.
    /// Vendor simulations only map the command and telemetry registers.
    /// </remarks>
    public abstract class PowerConversionSystemSimulationBase : ISimulatedDevice, IEventInjectable
    {
        /// <summary>
        /// Maximum response dead time.
        /// </summary>
        public static readonly TimeSpan MaximumResponseDeadTime = TimeSpan.FromSeconds(10);


        /// <summary>
        /// Maximum response time constant of the first-order lag.
        /// </summary>
        public static readonly TimeSpan MaximumResponseTimeConstant = TimeSpan.FromSeconds(60);


        /// <summary>
        /// Warning, fault and vendor event registers.
        /// </summary>
        private readonly EventRegisters _events;


        /// <summary>
        /// Setpoints read from the controller with the simulation time from which they are effective, oldest first.
        /// </summary>
        /// <remarks>
        /// Holds the setpoints of the last dead time plus the last setpoint older than the dead time.
        /// </remarks>
        private readonly List<SetpointSample> _setpointHistory = new List<SetpointSample>();


        /// <summary>
        /// Battery banks connected to the DC side of this PCS.
        /// </summary>
        private IReadOnlyList<BatteryBankSimulationBase> _batteryBanks = Array.Empty<BatteryBankSimulationBase>();


        /// <summary>
        /// Simulation time, the sum of all elapsed tick times.
        /// </summary>
        private TimeSpan _simulationTime = TimeSpan.Zero;


        /// <summary>
        /// Data lock object.
        /// </summary>
        protected readonly object _dataLock = new object();


        /// <summary>
        /// Modbus data store of this PCS.
        /// </summary>
        protected SimulationModbusDataStore Store { get; }


        /// <summary>
        /// Shared simulated grid.
        /// </summary>
        protected SimulatedGrid Grid { get; }


        /// <summary>
        /// Nameplate settings.
        /// </summary>
        public PowerConversionSystemSimulationSettings Settings { get; }


        /// <inheritdoc/>
        public int DeviceId { get => Settings.DeviceId; }


        /// <inheritdoc/>
        public string Name { get => Settings.Name; }


        /// <summary>
        /// Battery banks connected to the DC side of this PCS.
        /// </summary>
        public IReadOnlyList<BatteryBankSimulationBase> BatteryBanks
        {
            get { lock (_dataLock) { return _batteryBanks; } }
        }


        /// <summary>
        /// Current simulated state.
        /// </summary>
        public PowerConversionSystemSimulationState State
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = PowerConversionSystemSimulationState.Off;


        /// <summary>
        /// Active power in watts (positive discharging, negative charging).
        /// </summary>
        public double ActivePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Reactive power in volt-amperes reactive.
        /// </summary>
        public double ReactivePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Available active power in watts. Positive discharge capability while discharging or idle,
        /// negative charge capability while charging.
        /// </summary>
        public double ActiveAvailablePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Available reactive power in volt-amperes reactive.
        /// </summary>
        public double ReactiveAvailablePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Maximum active power change per second in percent of the nameplate active power. 0 means instant (ideal).
        /// </summary>
        public double SlowActivePowerByPercent
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Maximum reactive power change per second in percent of the nameplate reactive power. 0 means instant (ideal).
        /// </summary>
        public double SlowReactivePowerByPercent
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Time after which the PCS starts to react to a new active or reactive power setpoint. Zero means no delay (ideal).
        /// </summary>
        public TimeSpan ResponseDeadTime
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = TimeSpan.Zero;


        /// <summary>
        /// Time constant of the first-order lag: after the dead time the power reaches 63% of a setpoint step after one
        /// time constant and 95% after three. Zero means instant (ideal).
        /// </summary>
        public TimeSpan ResponseTimeConstant
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = TimeSpan.Zero;


        /// <summary>
        /// Flag whether a fault is active (fault register other than 0). An active fault rejects all commands.
        /// </summary>
        public bool IsFaultActive { get => (_events.Fault ?? 0) != 0; }


        /// <inheritdoc/>
        public bool HasWarningRegister { get => _events.WarningAddress.HasValue; }


        /// <inheritdoc/>
        public bool HasFaultRegister { get => _events.FaultAddress.HasValue; }


        /// <inheritdoc/>
        public bool HasVendorEventRegister { get => _events.VendorEventAddress.HasValue; }


        /// <inheritdoc/>
        public ushort? Warning { get => _events.Warning; }


        /// <inheritdoc/>
        public ushort? Fault { get => _events.Fault; }


        /// <inheritdoc/>
        public ushort? VendorEvent { get => _events.VendorEvent; }


        /// <summary>
        /// Constructor of <see cref="PowerConversionSystemSimulationBase"/>.
        /// </summary>
        /// <param name="store">Modbus data store of this PCS.</param>
        /// <param name="settings">Nameplate settings.</param>
        /// <param name="grid">Shared simulated grid.</param>
        /// <param name="warningAddress">Warning register address or null when the device has none.</param>
        /// <param name="faultAddress">Fault register address or null when the device has none.</param>
        /// <param name="vendorEventAddress">Vendor event register address or null when the device has none.</param>
        protected PowerConversionSystemSimulationBase(SimulationModbusDataStore store, PowerConversionSystemSimulationSettings settings, SimulatedGrid grid,
            int? warningAddress, int? faultAddress, int? vendorEventAddress)
        {
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(grid);

            Store = store;
            Settings = settings;
            Grid = grid;
            _events = new EventRegisters(store.Device, warningAddress, faultAddress, vendorEventAddress);
        }


        /// <summary>
        /// Attaches the battery banks of the DC side of this PCS.
        /// </summary>
        /// <param name="batteryBanks">Battery banks of the same DER unit.</param>
        public void AttachBatteryBanks(IEnumerable<BatteryBankSimulationBase> batteryBanks)
        {
            ArgumentNullException.ThrowIfNull(batteryBanks);

            lock (_dataLock)
            {
                _batteryBanks = batteryBanks.ToArray();
            }
        }


        /// <summary>
        /// Slows down the active and reactive power response of the PCS.
        /// </summary>
        /// <param name="slowActivePowerByPercent">Maximum active power change per second in percent of the nameplate. 0 means instant.</param>
        /// <param name="slowReactivePowerByPercent">Maximum reactive power change per second in percent of the nameplate. 0 means instant.</param>
        public void SlowDownPower(double slowActivePowerByPercent, double slowReactivePowerByPercent)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(slowActivePowerByPercent);
            ArgumentOutOfRangeException.ThrowIfNegative(slowReactivePowerByPercent);

            lock (_dataLock)
            {
                SlowActivePowerByPercent = slowActivePowerByPercent;
                SlowReactivePowerByPercent = slowReactivePowerByPercent;
            }
        }


        /// <summary>
        /// Sets the dead time and the first-order lag of the active and reactive power response of the PCS.
        /// </summary>
        /// <param name="deadTime">Time after which the PCS starts to react to a new setpoint (0..10 s). Zero means no delay.</param>
        /// <param name="timeConstant">Time constant of the first-order lag (0..60 s). Zero means instant.</param>
        /// <remarks>
        /// The dead time and the lag act before the ramp of <see cref="SlowDownPower(double, double)"/>, so all three can be combined.
        /// </remarks>
        public void SetResponseDynamics(TimeSpan deadTime, TimeSpan timeConstant)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(deadTime, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(deadTime, MaximumResponseDeadTime);
            ArgumentOutOfRangeException.ThrowIfLessThan(timeConstant, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(timeConstant, MaximumResponseTimeConstant);

            lock (_dataLock)
            {
                ResponseDeadTime = deadTime;
                ResponseTimeConstant = timeConstant;
            }
        }


        /// <inheritdoc/>
        /// <remarks>
        /// A fault bitmask other than 0 puts the PCS into the fault state. While the fault register is not 0, all start,
        /// stop and standby commands are rejected. Clearing the fault bitmask only clears the register, the PCS stays in
        /// the fault state until the next start, stop or standby command.
        /// </remarks>
        public void SetEvents(ushort? warning, ushort? fault, ushort? vendorEvent)
        {
            lock (_dataLock)
            {
                _events.Write(warning, fault, vendorEvent);

                if (fault.HasValue && fault.Value != 0)
                {
                    State = PowerConversionSystemSimulationState.Fault;
                    ActivePower = 0;
                    ReactivePower = 0;
                }
            }
        }


        /// <inheritdoc/>
        public Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken)
        {
            lock (_dataLock)
            {
                // The command is always taken so that a command written while faulted is not applied later
                PowerConversionSystemSimulationState? command = ReadCommand();

                if (command.HasValue && IsFaultActive is false)
                {
                    State = command.Value;
                }

                // The setpoints written since the last tick are effective from the start of this tick
                (double activeSetpoint, double reactiveSetpoint) = ReadSetpoints();
                TimeSpan tickStart = _simulationTime;
                _simulationTime += elapsed;
                _setpointHistory.Add(new SetpointSample(tickStart, activeSetpoint, reactiveSetpoint));

                List<BatteryBankSimulationBase> connected = _batteryBanks.Where(bank => bank.IsConnected).ToList();
                double dischargeLimit = Math.Min(Settings.NameplateMaximumActivePower, connected.Sum(bank => bank.AvailableDischargePower));
                double chargeLimit = Math.Min(Settings.NameplateMaximumActivePower, connected.Sum(bank => bank.AvailableChargePower));
                bool isConverting = State == PowerConversionSystemSimulationState.On || State == PowerConversionSystemSimulationState.Standby;

                if (isConverting)
                {
                    FollowSetpoints(tickStart, _simulationTime, chargeLimit, dischargeLimit, connected.Count > 0);
                }
                else
                {
                    ActivePower = 0;
                    ReactivePower = 0;
                }

                PruneSetpointHistory();
                double reactiveLimit = connected.Count > 0 ? GetReactiveLimit(ActivePower) : 0;
                ActiveAvailablePower = ActivePower < 0 ? -chargeLimit : dischargeLimit;
                ReactiveAvailablePower = reactiveLimit;
                DistributeActivePower();
                WriteTelemetry(CreateTelemetry(connected, isConverting));
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// Reads the start, stop or standby command of the controller.
        /// </summary>
        /// <returns>The commanded state or null when the controller did not write a new command.</returns>
        /// <remarks>
        /// Use <see cref="SimulationModbusDataStore.TryTakeWrite(int)"/> so that a command written again (with the same value) is recognized.
        /// </remarks>
        protected abstract PowerConversionSystemSimulationState? ReadCommand();


        /// <summary>
        /// Reads the active and reactive power setpoints of the controller.
        /// </summary>
        /// <returns>Active power setpoint in watts and reactive power setpoint in volt-amperes reactive.</returns>
        protected abstract (double ActivePower, double ReactivePower) ReadSetpoints();


        /// <summary>
        /// Writes the telemetry to the vendor specific registers.
        /// </summary>
        /// <param name="telemetry">Telemetry of the PCS.</param>
        protected abstract void WriteTelemetry(PowerConversionSystemSimulationTelemetry telemetry);


        /// <summary>
        /// Moves the active and reactive power towards the delayed setpoints during one tick.
        /// </summary>
        /// <param name="tickStart">Simulation time at the start of the tick.</param>
        /// <param name="tickEnd">Simulation time at the end of the tick.</param>
        /// <param name="chargeLimit">Charge limit in watts (positive value).</param>
        /// <param name="dischargeLimit">Discharge limit in watts (positive value).</param>
        /// <param name="hasConnectedBank">Flag whether at least one battery bank is connected.</param>
        /// <remarks>
        /// The tick is split at the times where a delayed setpoint becomes effective, so the dead time is not rounded to
        /// the tick interval. Before the first setpoint passed the dead time, the PCS holds its power.
        /// </remarks>
        private void FollowSetpoints(TimeSpan tickStart, TimeSpan tickEnd, double chargeLimit, double dischargeLimit, bool hasConnectedBank)
        {
            TimeSpan cursor = tickStart;

            while (cursor < tickEnd)
            {
                TimeSpan delayedTime = cursor - ResponseDeadTime;
                int index = _setpointHistory.FindLastIndex(sample => sample.EffectiveFrom <= delayedTime);
                TimeSpan segmentEnd = tickEnd;

                if (index + 1 < _setpointHistory.Count)
                {
                    TimeSpan nextChange = _setpointHistory[index + 1].EffectiveFrom + ResponseDeadTime;
                    segmentEnd = nextChange < tickEnd ? nextChange : tickEnd;
                }

                TimeSpan segment = segmentEnd - cursor;
                double activeSetpoint = index >= 0 ? _setpointHistory[index].ActivePower : ActivePower;
                double reactiveSetpoint = index >= 0 ? _setpointHistory[index].ReactivePower : ReactivePower;

                double activeTarget = Math.Clamp(activeSetpoint, -chargeLimit, dischargeLimit);
                double active = Lag(ActivePower, activeTarget, ResponseTimeConstant, segment);
                active = Ramp(ActivePower, active, SlowActivePowerByPercent, Settings.NameplateMaximumActivePower, segment);
                ActivePower = Math.Clamp(active, -chargeLimit, dischargeLimit);

                double reactiveLimit = hasConnectedBank ? GetReactiveLimit(ActivePower) : 0;
                double reactiveTarget = Math.Clamp(reactiveSetpoint, -reactiveLimit, reactiveLimit);
                double reactive = Lag(ReactivePower, reactiveTarget, ResponseTimeConstant, segment);
                reactive = Ramp(ReactivePower, reactive, SlowReactivePowerByPercent, Settings.NameplateMaximumReactivePower, segment);
                ReactivePower = Math.Clamp(reactive, -reactiveLimit, reactiveLimit);

                cursor = segmentEnd;
            }
        }


        /// <summary>
        /// Removes the setpoints that can no longer become effective, keeping the last setpoint older than the dead time.
        /// </summary>
        private void PruneSetpointHistory()
        {
            TimeSpan delayedTime = _simulationTime - ResponseDeadTime;
            int index = _setpointHistory.FindLastIndex(sample => sample.EffectiveFrom <= delayedTime);

            if (index > 0)
            {
                _setpointHistory.RemoveRange(0, index);
            }
        }


        /// <summary>
        /// Moves a value towards its target along a first-order lag.
        /// </summary>
        /// <param name="current">Current value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="timeConstant">Time constant. Zero means instant.</param>
        /// <param name="elapsed">Elapsed time.</param>
        /// <returns>The new value.</returns>
        private static double Lag(double current, double target, TimeSpan timeConstant, TimeSpan elapsed)
        {
            if (timeConstant <= TimeSpan.Zero)
            {
                return target;
            }

            double factor = 1 - Math.Exp(-elapsed.TotalSeconds / timeConstant.TotalSeconds);

            return current + (target - current) * factor;
        }


        /// <summary>
        /// Moves a value towards its target with a maximum change per second.
        /// </summary>
        /// <param name="current">Current value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="percentPerSecond">Maximum change per second in percent of the nameplate. 0 means instant.</param>
        /// <param name="nameplate">Nameplate value.</param>
        /// <param name="elapsed">Elapsed time.</param>
        /// <returns>The new value.</returns>
        private static double Ramp(double current, double target, double percentPerSecond, double nameplate, TimeSpan elapsed)
        {
            if (percentPerSecond <= 0)
            {
                return target;
            }

            double maximumStep = percentPerSecond / 100 * nameplate * elapsed.TotalSeconds;

            return current + Math.Clamp(target - current, -maximumStep, maximumStep);
        }


        /// <summary>
        /// Gets the reactive power limit at an active power.
        /// </summary>
        /// <param name="activePower">Active power in watts.</param>
        /// <returns>Reactive power limit in volt-amperes reactive (positive value).</returns>
        private double GetReactiveLimit(double activePower)
        {
            double apparent = Settings.NameplateMaximumApparentPower;

            if (apparent <= 0)
            {
                return Settings.NameplateMaximumReactivePower;
            }

            double remaining = Math.Sqrt(Math.Max(0, apparent * apparent - activePower * activePower));

            return Math.Min(Settings.NameplateMaximumReactivePower, remaining);
        }


        /// <summary>
        /// Splits the active power across the battery banks in proportion to their usable capacity.
        /// </summary>
        private void DistributeActivePower()
        {
            bool isDischarging = ActivePower >= 0;
            List<(double Weight, double Limit)> candidates = _batteryBanks
                .Select(bank => (bank.UsableCapacity, isDischarging ? bank.AvailableDischargePower : bank.AvailableChargePower))
                .ToList();
            double[] allocations = BatteryPowerSplitter.Split(ActivePower, candidates);

            for (int index = 0; index < _batteryBanks.Count; index++)
            {
                _batteryBanks[index].SetAllocatedActivePower(allocations[index]);
            }
        }


        /// <summary>
        /// Creates the telemetry of the current tick.
        /// </summary>
        private PowerConversionSystemSimulationTelemetry CreateTelemetry(IReadOnlyCollection<BatteryBankSimulationBase> connected, bool isConverting)
        {
            double dcVoltage = connected.Count > 0 ? connected.Average(bank => bank.DcVoltage) : 0;
            double acVoltage = Settings.NominalACVoltage;
            double apparentPower = Math.Sqrt(ActivePower * ActivePower + ReactivePower * ReactivePower);

            return new PowerConversionSystemSimulationTelemetry
            {
                State = State,
                ActivePower = ActivePower,
                ReactivePower = ReactivePower,
                ActiveAvailablePower = ActiveAvailablePower,
                ReactiveAvailablePower = ReactiveAvailablePower,
                IsACBreakerClosed = isConverting,
                IsDcContactorClosed = connected.Count > 0,
                Frequency = Grid.Frequency,
                DcVoltage = dcVoltage,
                DcCurrent = dcVoltage > 0 ? ActivePower / dcVoltage : 0,
                AcVoltage = acVoltage,
                AcCurrent = acVoltage > 0 ? apparentPower / (Math.Sqrt(3) * acVoltage) : 0
            };
        }


        /// <summary>
        /// Setpoints read from the controller.
        /// </summary>
        /// <param name="EffectiveFrom">Simulation time from which the setpoints are effective (without dead time).</param>
        /// <param name="ActivePower">Active power setpoint in watts.</param>
        /// <param name="ReactivePower">Reactive power setpoint in volt-amperes reactive.</param>
        private readonly record struct SetpointSample(TimeSpan EffectiveFrom, double ActivePower, double ReactivePower);
    }
}

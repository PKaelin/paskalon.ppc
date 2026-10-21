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
    /// Vendor simulations only map the command and telemetry registers.
    /// </remarks>
    public abstract class PowerConversionSystemSimulationBase : ISimulatedDevice, IEventInjectable
    {
        /// <summary>
        /// Warning, fault and vendor event registers.
        /// </summary>
        private readonly EventRegisters _events;


        /// <summary>
        /// Battery banks connected to the DC side of this PCS.
        /// </summary>
        private IReadOnlyList<BatteryBankSimulationBase> _batteryBanks = Array.Empty<BatteryBankSimulationBase>();


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

                (double activeSetpoint, double reactiveSetpoint) = ReadSetpoints();
                List<BatteryBankSimulationBase> connected = _batteryBanks.Where(bank => bank.IsConnected).ToList();
                double dischargeLimit = Math.Min(Settings.NameplateMaximumActivePower, connected.Sum(bank => bank.AvailableDischargePower));
                double chargeLimit = Math.Min(Settings.NameplateMaximumActivePower, connected.Sum(bank => bank.AvailableChargePower));
                bool isConverting = State == PowerConversionSystemSimulationState.On || State == PowerConversionSystemSimulationState.Standby;

                if (isConverting)
                {
                    double activeTarget = Math.Clamp(activeSetpoint, -chargeLimit, dischargeLimit);
                    double active = Ramp(ActivePower, activeTarget, SlowActivePowerByPercent, Settings.NameplateMaximumActivePower, elapsed);
                    ActivePower = Math.Clamp(active, -chargeLimit, dischargeLimit);
                }
                else
                {
                    ActivePower = 0;
                }

                double reactiveLimit = connected.Count > 0 ? GetReactiveLimit(ActivePower) : 0;

                if (isConverting)
                {
                    double reactiveTarget = Math.Clamp(reactiveSetpoint, -reactiveLimit, reactiveLimit);
                    double reactive = Ramp(ReactivePower, reactiveTarget, SlowReactivePowerByPercent, Settings.NameplateMaximumReactivePower, elapsed);
                    ReactivePower = Math.Clamp(reactive, -reactiveLimit, reactiveLimit);
                }
                else
                {
                    ReactivePower = 0;
                }

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
    }
}

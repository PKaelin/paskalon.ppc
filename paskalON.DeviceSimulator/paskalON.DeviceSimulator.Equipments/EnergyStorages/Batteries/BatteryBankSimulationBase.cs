// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Maths.Randoms;
using paskalON.PhysicalUnits.Percentages;

namespace paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries
{
    /// <summary>
    /// Vendor independent simulation of a battery bank.
    /// </summary>
    /// <remarks>
    /// The battery bank starts disconnected and fully charged (100% preferred state of charge).
    /// Its active power is allocated by the PCS simulation of the same DER unit. The state of charge is integrated
    /// from the allocated active power and cut off hard at 0% and 100% usable state of charge. With the power taper enabled
    /// the available power reduces linearly near empty and full instead.
    /// Vendor simulations only map the command and telemetry registers.
    /// </remarks>
    public abstract class BatteryBankSimulationBase : ISimulatedDevice, IEventInjectable
    {
        /// <summary>
        /// Minimum charge speed factor (1 = real time).
        /// </summary>
        public const double MinimumChargeSpeedFactor = 1;


        /// <summary>
        /// Maximum charge speed factor (100 = 100 times faster than minimum charge speed so 100 seconds).
        /// </summary>
        public const double MaximumChargeSpeedFactor = 100;


        /// <summary>
        /// Usable state of charge in percent below which the discharge power tapers off linearly to 0 at 0% (power taper enabled).
        /// </summary>
        public const double LowerTaperStateOfCharge = 10;


        /// <summary>
        /// Usable state of charge in percent above which the charge power tapers off linearly to 0 at 100% (power taper enabled).
        /// </summary>
        public const double UpperTaperStateOfCharge = 90;


        /// <summary>
        /// Random walker for the state of health.
        /// </summary>
        private readonly RandomWalker<double> _sohWalker = new RandomWalker<double>(80, 0.1, 60, 90, 5);


        /// <summary>
        /// Warning, fault and vendor event registers.
        /// </summary>
        private readonly EventRegisters _events;


        /// <summary>
        /// Remaining pre-charge time.
        /// </summary>
        private TimeSpan _preChargeRemaining = TimeSpan.Zero;


        /// <summary>
        /// Data lock object.
        /// </summary>
        protected readonly object _dataLock = new object();


        /// <summary>
        /// Modbus data store of this battery bank.
        /// </summary>
        protected SimulationModbusDataStore Store { get; }


        /// <summary>
        /// Nameplate and simulation settings.
        /// </summary>
        public BatteryBankSimulationSettings Settings { get; }


        /// <inheritdoc/>
        public int DeviceId { get => Settings.DeviceId; }


        /// <inheritdoc/>
        public string Name { get => Settings.Name; }


        /// <summary>
        /// State of charge window the vendor reports in its state of charge register.
        /// </summary>
        protected virtual StateOfChargeType ReportedStateOfChargeType { get => StateOfChargeType.Usable; }


        /// <summary>
        /// Current simulated state.
        /// </summary>
        public BatteryBankSimulationState State
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = BatteryBankSimulationState.Disconnected;


        /// <summary>
        /// Flag whether the battery bank is connected to the DC bus.
        /// </summary>
        public bool IsConnected { get => State == BatteryBankSimulationState.Connected; }


        /// <summary>
        /// Usable state of charge in percent.
        /// </summary>
        public double UsableStateOfCharge
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Absolute state of charge in percent.
        /// </summary>
        public double AbsoluteStateOfCharge
        {
            get
            {
                return StateOfChargeCalculator.GetAbsoluteStateOfChargeFromUsable(UsableStateOfCharge, Settings.UsableMinimumStateOfCharge,
                    Settings.UsableMaximumStateOfCharge, Settings.AbsoluteMaximumStateOfCharge) ?? 0;
            }
        }


        /// <summary>
        /// Preferred (operator window) state of charge in percent. Can be below 0% or above 100% outside the preferred window.
        /// </summary>
        public double PreferredStateOfCharge
        {
            get
            {
                return StateOfChargeCalculator.GetPreferredStateOfChargeFromAbsolute(AbsoluteStateOfCharge, Settings.PreferredMinimumStateOfCharge,
                    Settings.PreferredMaximumStateOfCharge, Settings.AbsoluteMaximumStateOfCharge) ?? 0;
            }
        }


        /// <summary>
        /// Factor by which the state of charge changes faster than the physical definition (1 = real time,
        /// 100 = one real second charges or discharges like 100 seconds).
        /// </summary>
        public double ChargeSpeedFactor
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = MinimumChargeSpeedFactor;


        /// <summary>
        /// Flag whether the available power tapers off near empty and full instead of cutting off hard at 0% and 100% usable state of charge.
        /// </summary>
        public bool IsPowerTaperEnabled
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Usable capacity in watt-hours.
        /// </summary>
        public double UsableCapacity { get; }


        /// <summary>
        /// Active power allocated by the PCS in watts (positive discharging, negative charging).
        /// </summary>
        public double ActivePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Total DC voltage in volts, interpolated linearly between the minimum and maximum DC voltage by the absolute state of charge.
        /// </summary>
        public double DcVoltage
        {
            get
            {
                return Settings.MinimumDcVoltage + (Settings.MaximumDcVoltage - Settings.MinimumDcVoltage) * AbsoluteStateOfCharge / 100;
            }
        }


        /// <summary>
        /// Total DC current in amperes (positive discharging, negative charging).
        /// </summary>
        public double DcCurrent
        {
            get
            {
                double voltage = DcVoltage;

                return voltage > 0 ? ActivePower / voltage : 0;
            }
        }


        /// <summary>
        /// Active power in watts the battery bank can discharge right now.
        /// </summary>
        /// <remarks>
        /// With the power taper enabled the power reduces linearly from the nameplate at <see cref="LowerTaperStateOfCharge"/>
        /// to 0 at 0% usable state of charge.
        /// </remarks>
        public double AvailableDischargePower
        {
            get
            {
                lock (_dataLock)
                {
                    if (IsConnected is false || UsableStateOfCharge <= 0)
                    {
                        return 0;
                    }

                    double taper = IsPowerTaperEnabled ? Math.Min(1, UsableStateOfCharge / LowerTaperStateOfCharge) : 1;

                    return Settings.NameplateMaximumDischargeRate * taper;
                }
            }
        }


        /// <summary>
        /// Active power in watts (positive value) the battery bank can charge right now.
        /// </summary>
        /// <remarks>
        /// With the power taper enabled the power reduces linearly from the nameplate at <see cref="UpperTaperStateOfCharge"/>
        /// to 0 at 100% usable state of charge.
        /// </remarks>
        public double AvailableChargePower
        {
            get
            {
                lock (_dataLock)
                {
                    if (IsConnected is false || UsableStateOfCharge >= 100)
                    {
                        return 0;
                    }

                    double taper = IsPowerTaperEnabled ? Math.Min(1, (100 - UsableStateOfCharge) / (100 - UpperTaperStateOfCharge)) : 1;

                    return Settings.NameplateMaximumChargeRate * taper;
                }
            }
        }


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
        /// Constructor of <see cref="BatteryBankSimulationBase"/>.
        /// </summary>
        /// <param name="store">Modbus data store of this battery bank.</param>
        /// <param name="settings">Nameplate and simulation settings.</param>
        /// <param name="warningAddress">Warning register address or null when the device has none.</param>
        /// <param name="faultAddress">Fault register address or null when the device has none.</param>
        /// <param name="vendorEventAddress">Vendor event register address or null when the device has none.</param>
        protected BatteryBankSimulationBase(SimulationModbusDataStore store, BatteryBankSimulationSettings settings,
            int? warningAddress, int? faultAddress, int? vendorEventAddress)
        {
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentOutOfRangeException.ThrowIfNegative(settings.PreChargeDelay.Ticks);

            Store = store;
            Settings = settings;
            _events = new EventRegisters(store.Device, warningAddress, faultAddress, vendorEventAddress);

            // Start fully charged at 100% preferred state of charge
            double absoluteStateOfCharge = StateOfChargeCalculator.GetAbsoluteStateOfChargeFromPreferred(100, settings.PreferredMinimumStateOfCharge,
                settings.PreferredMaximumStateOfCharge, settings.AbsoluteMaximumStateOfCharge);

            UsableStateOfCharge = StateOfChargeCalculator.GetUsableStateOfChargeFromAbsolute(absoluteStateOfCharge, settings.UsableMinimumStateOfCharge,
                settings.UsableMaximumStateOfCharge, settings.AbsoluteMaximumStateOfCharge);

            UsableCapacity = StateOfChargeCalculator.GetUsableCapacity(settings.NameplateCapacity, settings.UsableMinimumStateOfCharge,
                settings.UsableMaximumStateOfCharge, settings.AbsoluteMinimumStateOfCharge, settings.AbsoluteMaximumStateOfCharge);
        }


        /// <summary>
        /// Sets the active power the PCS allocates to this battery bank.
        /// </summary>
        /// <param name="watts">Active power in watts (positive discharging, negative charging).</param>
        /// <remarks>
        /// A battery bank that is not connected does not take any power.
        /// </remarks>
        public void SetAllocatedActivePower(double watts)
        {
            lock (_dataLock)
            {
                ActivePower = IsConnected ? watts : 0;
            }
        }


        /// <summary>
        /// Sets the state of charge to a preferred (operator window) state of charge.
        /// </summary>
        /// <param name="preferredStateOfCharge">Preferred state of charge in percent (0..100).</param>
        /// <remarks>
        /// The value is converted to the usable state of charge and clamped to the usable window.
        /// </remarks>
        public void SetPreferredStateOfCharge(double preferredStateOfCharge)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(preferredStateOfCharge, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(preferredStateOfCharge, 100);

            double absoluteStateOfCharge = StateOfChargeCalculator.GetAbsoluteStateOfChargeFromPreferred(preferredStateOfCharge,
                Settings.PreferredMinimumStateOfCharge, Settings.PreferredMaximumStateOfCharge, Settings.AbsoluteMaximumStateOfCharge);
            double usableStateOfCharge = StateOfChargeCalculator.GetUsableStateOfChargeFromAbsolute(absoluteStateOfCharge,
                Settings.UsableMinimumStateOfCharge, Settings.UsableMaximumStateOfCharge, Settings.AbsoluteMaximumStateOfCharge);

            UsableStateOfCharge = Math.Clamp(usableStateOfCharge, 0, 100);
        }


        /// <summary>
        /// Sets how much faster the state of charge changes than the physical definition.
        /// </summary>
        /// <param name="chargeSpeedFactor">
        /// Charge speed factor from <see cref="MinimumChargeSpeedFactor"/> (1 = real time) to <see cref="MaximumChargeSpeedFactor"/>
        /// (100 = one real second charges or discharges like 100 seconds).
        /// </param>
        /// <remarks>
        /// Only the state of charge integration is accelerated. The active power and the pre-charge delay stay real time.
        /// </remarks>
        public void SetChargeSpeedFactor(double chargeSpeedFactor)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(chargeSpeedFactor, MinimumChargeSpeedFactor);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(chargeSpeedFactor, MaximumChargeSpeedFactor);

            ChargeSpeedFactor = chargeSpeedFactor;
        }


        /// <summary>
        /// Enables or disables the power taper near empty and full.
        /// </summary>
        /// <param name="isEnabled">
        /// True to reduce the available power linearly below <see cref="LowerTaperStateOfCharge"/> and above <see cref="UpperTaperStateOfCharge"/>
        /// usable state of charge, false to cut off hard at 0% and 100%.
        /// </param>
        public void SetPowerTaper(bool isEnabled)
        {
            IsPowerTaperEnabled = isEnabled;
        }


        /// <inheritdoc/>
        public void SetEvents(ushort? warning, ushort? fault, ushort? vendorEvent)
        {
            lock (_dataLock)
            {
                _events.Write(warning, fault, vendorEvent);
            }
        }


        /// <inheritdoc/>
        public Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken)
        {
            lock (_dataLock)
            {
                ApplyConnectCommand(ReadConnectCommand(), elapsed);
                IntegrateStateOfCharge(elapsed);

                WriteTelemetry(new BatteryBankSimulationTelemetry
                {
                    State = State,
                    ActivePower = ActivePower,
                    StateOfCharge = GetReportedStateOfCharge(),
                    StateOfHealth = _sohWalker.Next(),
                    DcVoltage = DcVoltage,
                    DcCurrent = DcCurrent
                });
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// Reads the connect command of the controller.
        /// </summary>
        /// <returns>True to connect, false to disconnect and null when no known command is present.</returns>
        protected abstract bool? ReadConnectCommand();


        /// <summary>
        /// Writes the telemetry to the vendor specific registers.
        /// </summary>
        /// <param name="telemetry">Telemetry of the battery bank.</param>
        protected abstract void WriteTelemetry(BatteryBankSimulationTelemetry telemetry);


        /// <summary>
        /// Applies the connect command and advances the pre-charge.
        /// </summary>
        private void ApplyConnectCommand(bool? connect, TimeSpan elapsed)
        {
            if (connect is false)
            {
                State = BatteryBankSimulationState.Disconnected;
                ActivePower = 0;
                _preChargeRemaining = TimeSpan.Zero;
            }
            else if (connect is true && State == BatteryBankSimulationState.Disconnected)
            {
                State = BatteryBankSimulationState.PreCharging;
                _preChargeRemaining = Settings.PreChargeDelay;
            }
            else if (State == BatteryBankSimulationState.PreCharging)
            {
                _preChargeRemaining -= elapsed;
            }

            if (State == BatteryBankSimulationState.PreCharging && _preChargeRemaining <= TimeSpan.Zero)
            {
                State = BatteryBankSimulationState.Connected;
            }
        }


        /// <summary>
        /// Integrates the usable state of charge from the active power with a hard cut off at 0% and 100%.
        /// </summary>
        /// <remarks>
        /// ActivePower in watts, elapsed.TotalHours in hours, UsableCapacity in watt-hours.
        /// E.g. NameplateCapacity=5MWh / Absolute=0..100% / Usable=10..90% => UsableCapacity=5MWh*0.8=4MWh.
        /// Discharging with 4MW for 30 minutes reduces the usable state of charge by 50%.
        /// The elapsed time is multiplied by the <see cref="ChargeSpeedFactor"/>.
        /// </remarks>
        private void IntegrateStateOfCharge(TimeSpan elapsed)
        {
            if (UsableCapacity <= 0)
            {
                return;
            }

            double socDelta = ActivePower * elapsed.TotalHours * ChargeSpeedFactor / UsableCapacity * 100;

            UsableStateOfCharge = Math.Clamp(UsableStateOfCharge - socDelta, 0, 100);
        }


        /// <summary>
        /// Gets the state of charge in the window the vendor reports.
        /// </summary>
        private double GetReportedStateOfCharge()
        {
            switch (ReportedStateOfChargeType)
            {
                case StateOfChargeType.Absolute:
                    return AbsoluteStateOfCharge;
                case StateOfChargeType.Preferred:
                    return Math.Clamp(PreferredStateOfCharge, 0, 100);
                default:
                    return UsableStateOfCharge;
            }
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.PhysicalUnits.Percentages;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples
{
    /// <summary>
    /// BB simple is a basic implementation of the battery bank base class <see cref="BatteryBankBase"/>.
    /// It shall be used for tests, simulations, analysis and as a reference for all concrete implementations.
    /// </summary>
    public class BbSimpleV1Proxy : BatteryBankBase, IDisposable
    {
        /// <summary>
        /// Mask of the 16 bits of a register, the bit flag registers are read as signed 16 bit values.
        /// </summary>
        private const int RegisterBitMask = 0xFFFF;


        /// <summary>
        /// Name of the state that is set when the device reports a bit that is not defined.
        /// </summary>
        private const string UnknownStateName = "Unknown";


        /// <summary>
        /// Modbus client communication.
        /// </summary>
        private readonly IModbusClient _client;


        /// <summary>
        /// Set state of charge by using the battery banks SOC endpoint value.
        /// </summary>
        private double? LocalStateOfCharge
        {
            set
            {
                double? usableSoc = value;

                if (field != usableSoc)
                {
                    field = value;

                    if (usableSoc == null)
                    {
                        AbsoluteStateOfCharge = null;
                        UsableStateOfCharge = null;
                        StateOfCharge = null;
                    }
                    else
                    {
                        UsableStateOfCharge = usableSoc;
                        AbsoluteStateOfCharge = StateOfChargeCalculator.GetAbsoluteStateOfChargeFromUsable(usableSoc, UsableMinimumStateOfCharge,
                            UsableMaximumStateOfCharge, AbsoluteMaximumStateOfCharge);
                        StateOfCharge = StateOfChargeCalculator.GetPreferredStateOfChargeFromAbsolute(AbsoluteStateOfCharge, PreferredMinimumStateOfCharge,
                            PreferredMaximumStateOfCharge, AbsoluteMaximumStateOfCharge);
                    }
                }
            }
        }

        /// <summary>
        /// Constructor of <see cref="BbSimpleV1Proxy"/>
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The battery bank configuration.</param>
        /// <param name="batteryStorageUnit">The paren battery storage unit.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        /// <param name="client">The Modbus client interface.</param>
        public BbSimpleV1Proxy(ILogger logger, BatteryBankConfig config, DerBatteryStorageUnit batteryStorageUnit, IMetricsPublisher publisher,
            IModbusDataface dataface, IModbusClient client) : base(logger, config, batteryStorageUnit, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(dataface);

            _client = client;
            _client.OnCommunicationError += OnCommunicationError;
        }


        /// <inheritdoc/>
        public override async Task ConnectAsync()
        {
            await _lifecycleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (_client.State != ModbusClientState.Connected && _client.State != ModbusClientState.Connecting)
                {
                    await _client.ConnectAsync();
                }

                try
                {
                    await base.ConnectAsync();
                    await _client.WriteSingleRegisterAsync((ushort)BbSimpleV1Description.Register.SelectorState,
                        (ushort)BbSimpleV1Description.State.Connected, ModbusDataType.MbInt16);
                }
                catch
                {
                    lock (dataLock)
                    {
                        RaiseCommunicationError();
                        throw;
                    }
                }
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }


        /// <inheritdoc/>
        public override async Task DisconnectAsync()
        {
            await _lifecycleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (_client.State == ModbusClientState.Connected)
                {
                    try
                    {
                        await base.DisconnectAsync();
                        await _client.WriteSingleRegisterAsync((ushort)BbSimpleV1Description.Register.SelectorState,
                            (ushort)BbSimpleV1Description.State.Disconnected, ModbusDataType.MbInt16);
                    }
                    catch
                    {
                        lock (dataLock)
                        {
                            RaiseCommunicationError();
                            throw;
                        }
                    }
                }
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }


        /// <inheritdoc/>
        public override async Task HeartbeatAsync(CancellationToken cancellationToken)
        {
            if (_client.State == ModbusClientState.Connected)
            {
                await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.Heartbeat, ControllerToDeviceHeartbeat, ModbusDataType.MbInt16);
            }
        }


        /// <inheritdoc/>
        protected override void RegisterDataface()
        {
            // State of charge and health
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.Register<BbSimpleV1Proxy, double?>(this, nameof(StateOfCharge),
                (x, v) => x.LocalStateOfCharge = v, (int)BbSimpleV1Description.Register.TotalStateOfCharge, ModbusScale.Downscale100, ModbusDataType.MbUint16));
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.Register<BbSimpleV1Proxy, double?>(this, nameof(StateOfHealth),
                (x, v) => x.StateOfHealth = v, (int)BbSimpleV1Description.Register.TotalStateOfHealth, ModbusScale.Downscale100, ModbusDataType.MbUint16));
            // Current, Voltage
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.Register<BbSimpleV1Proxy, double?>(this, nameof(TotalDCVoltage),
                (x, v) => x.TotalDCVoltage = v, (int)BbSimpleV1Description.Register.TotalDCVoltage, ModbusScale.NoScale, ModbusDataType.MbUint16));
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.Register<BbSimpleV1Proxy, double?>(this, nameof(TotalDCCurrent),
                (x, v) => x.TotalDCCurrent = v, (int)BbSimpleV1Description.Register.TotalDCCurrent, ModbusScale.NoScale, ModbusDataType.MbUint16));
            // Current, Voltage, state of charge and health range
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.RegisterRange((int)BbSimpleV1Description.Register.TotalStateOfCharge,
                (int)BbSimpleV1Description.Register.TotalDCCurrent, ModbusRegistryType.HoldingRegister, _config.ModbusConfig.ModbusConnectionConfig.PollingFactorClass1));
            // State, Warnings, Faults, VendorEvents
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.Register<BbSimpleV1Proxy, int>(this, nameof(State),
                (x, v) => x.SetState(v), (int)BbSimpleV1Description.Register.CurrentState, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.Register<BbSimpleV1Proxy, int>(this, nameof(WarningStates),
                (x, v) => x.SetWarning(v), (int)BbSimpleV1Description.Register.CurrentWarning, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.Register<BbSimpleV1Proxy, int>(this, nameof(FaultStates),
                (x, v) => x.SetFault(v), (int)BbSimpleV1Description.Register.CurrentFault, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.Register<BbSimpleV1Proxy, int>(this, nameof(VendorEvents),
                (x, v) => x.SetVendorEvent(v), (int)BbSimpleV1Description.Register.CurrentVendorEvent, ModbusScale.NoScale, ModbusDataType.MbInt16));
            // State, Warnings, Faults, VendorEvents range
            Dataface.Register<BbSimpleV1Proxy, IModbusRegister>(r => r.RegisterRange((int)BbSimpleV1Description.Register.CurrentState,
                (int)BbSimpleV1Description.Register.CurrentVendorEvent, ModbusRegistryType.HoldingRegister, _config.ModbusConfig.ModbusConnectionConfig.PollingFactorClass1));
        }


        /// <summary>
        /// Convert the device state with the domain state.
        /// </summary>
        /// <param name="state">The device state.</param>
        /// <remarks>
        /// All possible state should be mapped to the device state.
        /// Do not add states in base class without consulting the lead.
        /// The state register was read successfully, so the communication works again whatever the state is.
        /// </remarks>
        private void SetState(int state)
        {
            CommunicationError = false;

            // Check whether we have defined all state
            if (Enum.IsDefined(typeof(BbSimpleV1Description.State), state) == false)
            {
                _logger.LogError("{Name} Undefined state reported. State: {State}", Name, State);
            }

            switch (state)
            {
                case (int)BbSimpleV1Description.State.Disconnected:
                    UpdateState(BatteryBankState.Disconnected);
                    BatteryBankFlowDirection = Domain.EnergyStorages.Batteries.BatteryBankFlowDirection.Idle;
                    break;
                case (int)BbSimpleV1Description.State.Connected:
                case (int)BbSimpleV1Description.State.Idle:
                    UpdateState(BatteryBankState.Connected);
                    BatteryBankFlowDirection = Domain.EnergyStorages.Batteries.BatteryBankFlowDirection.Idle;
                    break;
                case (int)BbSimpleV1Description.State.Discharging:
                    UpdateState(BatteryBankState.Connected);
                    BatteryBankFlowDirection = Domain.EnergyStorages.Batteries.BatteryBankFlowDirection.Discharging;
                    break;
                case (int)BbSimpleV1Description.State.Charging:
                    UpdateState(BatteryBankState.Connected);
                    BatteryBankFlowDirection = Domain.EnergyStorages.Batteries.BatteryBankFlowDirection.Charging;
                    break;
                default:
                    // Do not change the state
                    break;
            }
        }


        /// <summary>
        /// Sets the warning states.
        /// </summary>
        /// <param name="warning">The warning register value, bit flags of <see cref="BbSimpleV1Description.WarningCode"/>.</param>
        private void SetWarning(int warning)
        {
            SetBitFlagStates<BbSimpleV1Description.WarningCode>(warning, WarningStates, SetWarning, nameof(BbSimpleV1Description.Register.CurrentWarning));
        }


        /// <summary>
        /// Sets the fault states.
        /// </summary>
        /// <param name="fault">The fault register value, bit flags of <see cref="BbSimpleV1Description.FaultCode"/>.</param>
        private void SetFault(int fault)
        {
            SetBitFlagStates<BbSimpleV1Description.FaultCode>(fault, FaultStates, SetFault, nameof(BbSimpleV1Description.Register.CurrentFault));
        }


        /// <summary>
        /// Sets the vendor events.
        /// </summary>
        /// <param name="vendorEvent">The vendor event register value, bit flags of <see cref="BbSimpleV1Description.VendorEvents"/>.</param>
        private void SetVendorEvent(int vendorEvent)
        {
            SetBitFlagStates<BbSimpleV1Description.VendorEvents>(vendorEvent, VendorEvents, SetVendorEvent, nameof(BbSimpleV1Description.Register.CurrentVendorEvent));
        }


        /// <summary>
        /// Sets the states of a bit flag register.
        /// </summary>
        /// <typeparam name="TCode">Bit flag enumeration of the register.</typeparam>
        /// <param name="value">The register value.</param>
        /// <param name="states">The current states of the register.</param>
        /// <param name="setState">Sets one state by its name.</param>
        /// <param name="registerName">Name of the register for logging.</param>
        /// <remarks>
        /// A set bit sets its state to true, a cleared bit sets an already reported state back to false.
        /// Codes that have never been active are not added, so that only reported codes are published.
        /// Bits that are not defined in the enumeration set the Unknown state, it is not reset.
        /// </remarks>
        private void SetBitFlagStates<TCode>(int value, IReadOnlyDictionary<string, bool> states, Action<string, bool> setState, string registerName)
            where TCode : struct, Enum
        {
            int bits = value & RegisterBitMask;
            int definedBits = 0;

            foreach (TCode code in Enum.GetValues<TCode>())
            {
                int codeBit = Convert.ToInt32(code);

                if (codeBit == 0)
                {
                    continue;
                }

                // Bitwise OR to accumulate all defined bits, so that we can check for undefined bits later.
                definedBits |= codeBit;
                string name = code.ToString();
                bool isActive = (bits & codeBit) == codeBit;

                if (isActive || states.ContainsKey(name))
                {
                    setState(name, isActive);
                }
            }

            int undefinedBits = bits & ~definedBits;

            if (undefinedBits != 0)
            {
                _logger.LogError("{Name} Undefined bits found in {Register}. Undefined bits: {UndefinedBits}, value: {Value}", Name, registerName, undefinedBits, bits);
                setState(UnknownStateName, true);
            }
        }


        /// <summary>
        /// Dispose instance.
        /// </summary>
        public void Dispose()
        {
            _client.OnCommunicationError -= OnCommunicationError;
        }
    }
}

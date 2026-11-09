// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.PowerConversionSystems.Simples
{
    /// <summary>
    /// PCS simple is a basic implementation of the PCS base class <see cref="PowerConversionSystemBase"/>.
    /// It shall be used for tests, simulations, analysis and as a reference for all concrete implementations.
    /// </summary>
    public class PcsSimpleV1Proxy : PowerConversionSystemBase, IDisposable
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
        /// Constructor of <see cref="PcsSimpleV1Proxy"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The power conversion system configuration.</param>
        /// <param name="derUnit">The parent DER unit.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The data face interface.</param>
        /// <param name="client">The Modbus client interface.</param>
        public PcsSimpleV1Proxy(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher publisher,
            IModbusDataface dataface, IModbusClient client) : base(logger, config, derUnit, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(dataface);

            _client = client;
            _client.OnCommunicationError += OnCommunicationError;
            _client.OnConnected += OnClientConnected;
        }



        /// <inheritdoc/>
        public override async Task StartAsync()
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
                    await base.StartAsync();
                    await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState,
                        (ushort)PcsSimpleV1Description.State.On, ModbusDataType.MbInt16);
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
        public override async Task StopAsync()
        {
            await _lifecycleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (_client.State == ModbusClientState.Connected)
                {
                    try
                    {
                        await base.StopAsync();
                        await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.PReference, 0, ModbusDataType.MbInt16);
                        await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.QReference, 0, ModbusDataType.MbInt16);
                        await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState,
                            (ushort)PcsSimpleV1Description.State.Off, ModbusDataType.MbInt16);
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
        public override async Task StandbyAsync(double? standbyActivePower = null)
        {
            if (_client.State == ModbusClientState.Connected)
            {
                try
                {
                    await base.StandbyAsync(standbyActivePower);

                    if (standbyActivePower != null)
                    {
                        await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.PReference, (double)standbyActivePower,
                            ModbusDataType.MbInt16, 2, ModbusScale.Downscale1000);
                    }
                    else
                    {
                        // Use kilo watts instead of ModbusScale.Downscale1000
                        await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.PReference, StandbyActivePowerWatts,
                            ModbusDataType.MbInt16, 2, ModbusScale.Downscale1000);
                    }

                    await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.QReference, 0,
                            ModbusDataType.MbInt16, 2, ModbusScale.Downscale1000);

                    await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState,
                        (ushort)PcsSimpleV1Description.State.Standby, ModbusDataType.MbInt16);
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


        /// <inheritdoc/>
        public override async Task SetActivePowerTargetAsync(double? value)
        {
            if (_client.State == ModbusClientState.Connected)
            {
                await base.SetActivePowerTargetAsync(value);

                if (ActivePowerTarget.HasValue)
                {
                    await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.PReference, ActivePowerTarget.Value.Watts,
                        ModbusDataType.MbInt16, 2, ModbusScale.Downscale1000);
                }
            }
        }


        /// <inheritdoc/>
        public override async Task SetReactivePowerTargetAsync(double? value)
        {
            if (_client.State == ModbusClientState.Connected)
            {
                await base.SetReactivePowerTargetAsync(value);

                if (ReactivePowerTarget.HasValue)
                {
                    await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.QReference, ReactivePowerTarget.Value.VoltAmperesReactive,
                        ModbusDataType.MbInt16, 2, ModbusScale.Downscale1000);
                }
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
            // Power
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ActivePower),
                (x, v) => x.ActivePowerValue = v, (int)PcsSimpleV1Description.Register.P, ModbusScale.Upscale1000, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ActiveAvailablePower),
                (x, v) => x.ActiveAvailablePowerValue = v, (int)PcsSimpleV1Description.Register.PAvailable, ModbusScale.Upscale1000, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ReactivePower),
                (x, v) => x.ReactivePowerValue = v, (int)PcsSimpleV1Description.Register.Q, ModbusScale.Upscale1000, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ReactiveAvailablePower),
                (x, v) => x.ReactiveAvailablePowerValue = v, (int)PcsSimpleV1Description.Register.QAvailable, ModbusScale.Upscale1000, ModbusDataType.MbInt16));
            // Power range
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.RegisterRange((int)PcsSimpleV1Description.Register.P, (int)PcsSimpleV1Description.Register.QAvailable,
                ModbusRegistryType.HoldingRegister, _config.ModbusConfig.ModbusConnectionConfig.PollingFactorClass1));
            // Current, Voltage, Frequency
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(Frequency),
                (x, v) => x.Frequency = v, (int)PcsSimpleV1Description.Register.Frequency, ModbusScale.Downscale100, ModbusDataType.MbUint16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(DCCurrent),
                (x, v) => x.DCCurrent = v, (int)PcsSimpleV1Description.Register.DCCurrent, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(DCVoltage),
                (x, v) => x.DCVoltage = v, (int)PcsSimpleV1Description.Register.DCVoltage, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ACCurrent),
                (x, v) => x.ACCurrent = v, (int)PcsSimpleV1Description.Register.ACCurrent, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ACVoltage),
                (x, v) => x.ACVoltage = v, (int)PcsSimpleV1Description.Register.ACVoltage, ModbusScale.NoScale, ModbusDataType.MbInt16));
            // Current, Voltage, Frequency range
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.RegisterRange((int)PcsSimpleV1Description.Register.Frequency, (int)PcsSimpleV1Description.Register.ACVoltage,
                ModbusRegistryType.HoldingRegister, _config.ModbusConfig.ModbusConnectionConfig.PollingFactorClass1));
            // State, Warnings, Faults, VendorEvents
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, int>(this, nameof(State),
                (x, v) => x.SetState(v), (int)PcsSimpleV1Description.Register.CurrentState, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, int>(this, nameof(WarningStates),
                (x, v) => x.SetWarning(v), (int)PcsSimpleV1Description.Register.CurrentWarning, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, int>(this, nameof(FaultStates),
                (x, v) => x.SetFault(v), (int)PcsSimpleV1Description.Register.CurrentFault, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, int>(this, nameof(VendorEvents),
                (x, v) => x.SetVendorEvent(v), (int)PcsSimpleV1Description.Register.CurrentVendorEvent, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, int>(this, nameof(IsACBreakerClosed),
                (x, v) => x.SetAcBreaker(v), (int)PcsSimpleV1Description.Register.ACBreaker, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, int>(this, nameof(IsDcContactorClosed),
                (x, v) => x.SetDcContactors(v), (int)PcsSimpleV1Description.Register.DcContactor, ModbusScale.NoScale, ModbusDataType.MbInt16));
            // State, Warnings, Faults, VendorEvents range
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.RegisterRange((int)PcsSimpleV1Description.Register.CurrentState, (int)PcsSimpleV1Description.Register.DcContactor,
                ModbusRegistryType.HoldingRegister, _config.ModbusConfig.ModbusConnectionConfig.PollingFactorClass1));
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
            if (Enum.IsDefined(typeof(PcsSimpleV1Description.State), state) == false)
            {
                _logger.LogError("{Name} Undefined state reported. State: {State}", Name, State);
            }

            // Logging and even invocation is done in the setter of the State property
            switch (state)
            {
                case (int)PcsSimpleV1Description.State.Initialization:
                    UpdateState(PcsState.Starting);
                    break;
                case (int)PcsSimpleV1Description.State.On:
                    UpdateState(PcsState.Started);
                    break;
                case (int)PcsSimpleV1Description.State.Unknown:
                case (int)PcsSimpleV1Description.State.Off:
                case (int)PcsSimpleV1Description.State.Stop:
                case (int)PcsSimpleV1Description.State.Fault:
                    UpdateState(PcsState.Stopped);
                    break;
                case (int)PcsSimpleV1Description.State.Standby:
                    UpdateState(PcsState.Standby);
                    break;
                case (int)PcsSimpleV1Description.State.NightMode:
                    UpdateState(PcsState.NightMode);
                    break;
                default:
                    // Do not change the state
                    break;
            }
        }


        /// <summary>
        /// Sets the warning states.
        /// </summary>
        /// <param name="warning">The warning register value, bit flags of <see cref="PcsSimpleV1Description.WarningCode"/>.</param>
        private void SetWarning(int warning)
        {
            SetBitFlagStates<PcsSimpleV1Description.WarningCode>(warning, WarningStates, SetWarning, nameof(PcsSimpleV1Description.Register.CurrentWarning));
        }


        /// <summary>
        /// Sets the fault states.
        /// </summary>
        /// <param name="fault">The fault register value, bit flags of <see cref="PcsSimpleV1Description.FaultCode"/>.</param>
        private void SetFault(int fault)
        {
            SetBitFlagStates<PcsSimpleV1Description.FaultCode>(fault, FaultStates, SetFault, nameof(PcsSimpleV1Description.Register.CurrentFault));
        }


        /// <summary>
        /// Sets the vendor events.
        /// </summary>
        /// <param name="vendorEvent">The vendor event register value, bit flags of <see cref="PcsSimpleV1Description.VendorEvents"/>.</param>
        private void SetVendorEvent(int vendorEvent)
        {
            SetBitFlagStates<PcsSimpleV1Description.VendorEvents>(vendorEvent, VendorEvents, SetVendorEvent, nameof(PcsSimpleV1Description.Register.CurrentVendorEvent));
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
        /// Sets whether the AC breaker is closed.
        /// </summary>
        /// <param name="acBreaker">The AC breaker value.</param>
        /// <remarks>
        /// Keep it simple in simple proxy
        /// </remarks>
        private void SetAcBreaker(int acBreaker)
        {
            if (acBreaker == 0)
            {
                IsACBreakerClosed = false;
            }
            else
            {
                IsACBreakerClosed = true;
            }
        }


        /// <summary>
        /// Sets whether one or many DC contactors are closed.
        /// </summary>
        /// <param name="dcContactors">The DC contactor value.</param>
        /// <remarks>
        /// Keep it simple in simple proxy
        /// </remarks>
        private void SetDcContactors(int dcContactors)
        {
            if (dcContactors == 0)
            {
                IsDcContactorClosed = new[] { false };
            }
            else if (dcContactors == 2)
            {
                IsDcContactorClosed = new[] { true, true };
            }
            else if (dcContactors == 3)
            {
                IsDcContactorClosed = new[] { true, true, true };
            }
            else
            {
                IsDcContactorClosed = new[] { true };
            }
        }


        /// <summary>
        /// Dispose instance.
        /// </summary>
        public void Dispose()
        {
            _client.OnCommunicationError -= OnCommunicationError;
            _client.OnConnected -= OnClientConnected;
        }
    }
}

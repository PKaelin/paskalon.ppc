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
        }



        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task StartAsync()
        {
            await base.StartAsync();
            await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState, 1, ModbusDataType.MbInt16);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task StopAsync()
        {
            await base.StopAsync();
            await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState, 0, ModbusDataType.MbInt16);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task StandbyAsync(double? standbyActivePower = null)
        {
            await base.StandbyAsync(standbyActivePower);

            if (standbyActivePower != null)
            {
                await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.PReference, (double)standbyActivePower, ModbusDataType.MbInt16);
            }

            await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState, 3, ModbusDataType.MbInt16);
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public override async Task SetActivePowerTargetAsync(double? value)
        {
            await base.SetActivePowerTargetAsync(value);

            if (ActivePowerTarget.HasValue)
            {
                await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.PReference, ActivePowerTarget.Value.KiloWatts, ModbusDataType.MbInt16);
            }
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public override async Task SetReactivePowerTargetAsync(double? value)
        {
            await base.SetReactivePowerTargetAsync(value);

            if (ReactivePowerTarget.HasValue)
            {
                await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.QReference, ReactivePowerTarget.Value.KiloVoltAmperesReactive, ModbusDataType.MbInt16);
            }
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        protected override void RegisterDataface()
        {
            // Power
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ActivePower),
                (x, v) => x.ActivePowerValue = v, (int)PcsSimpleV1Description.Register.P, ModbusScale.NoScale, ModbusDataType.MbUint16));

            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ActiveAvailablePower),
                (x, v) => x.ActiveAvailablePowerValue = v, (int)PcsSimpleV1Description.Register.PAvailable, ModbusScale.NoScale, ModbusDataType.MbUint16));

            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ReactivePower),
                (x, v) => x.ReactivePowerValue = v, (int)PcsSimpleV1Description.Register.Q, ModbusScale.NoScale, ModbusDataType.MbUint16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ReactiveAvailablePower),
                (x, v) => x.ReactiveAvailablePowerValue = v, (int)PcsSimpleV1Description.Register.QAvailable, ModbusScale.NoScale, ModbusDataType.MbUint16));
            // Power range
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.RegisterRange((int)PcsSimpleV1Description.Register.P, (int)PcsSimpleV1Description.Register.QAvailable,
                ModbusRegistryType.HoldingRegister, _config.ModbusConfig.ModbusConnectionConfig.PollingFactorClass1));
            // Current, Voltage, Frequency
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(Frequency),
                (x, v) => x.Frequency = v, (int)PcsSimpleV1Description.Register.Frequency, ModbusScale.Downscale100, ModbusDataType.MbUint16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(DCCurrent),
                (x, v) => x.DCCurrent = v, (int)PcsSimpleV1Description.Register.DCCurrent, ModbusScale.NoScale, ModbusDataType.MbUint16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(DCVoltage),
                (x, v) => x.DCVoltage = v, (int)PcsSimpleV1Description.Register.DCVoltage, ModbusScale.NoScale, ModbusDataType.MbUint16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ACCurrent),
                (x, v) => x.ACCurrent = v, (int)PcsSimpleV1Description.Register.ACCurrent, ModbusScale.NoScale, ModbusDataType.MbUint16));
            Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, double?>(this, nameof(ACVoltage),
                (x, v) => x.ACVoltage = v, (int)PcsSimpleV1Description.Register.ACVoltage, ModbusScale.NoScale, ModbusDataType.MbUint16));
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
                ModbusRegistryType.HoldingRegister, _config.ModbusConfig.ModbusConnectionConfig.PollingFactorClass2));
        }


        /// <summary>
        /// Convert the device state with the domain state.
        /// </summary>
        /// <param name="state">The device state.</param>
        /// <remarks>
        /// All possible state should be mapped to the device state.
        /// Do not add states in base class without consulting the lead.
        /// </remarks>
        private void SetState(int state)
        {
            // Check whether we have defined all state
            if (Enum.IsDefined(typeof(PcsSimpleV1Description.State), state) == false)
            {
                _logger.LogError("{Name} Undefined state reported. State: {State}", Name, State);
            }

            // Logging and even invocation is done in the setter of the State property
            switch (state)
            {
                case (int)PcsSimpleV1Description.State.Initialization:
                    State = PcsState.Starting;
                    break;
                case (int)PcsSimpleV1Description.State.On:
                    State = PcsState.Started;
                    CommunicationError = false;
                    break;
                case (int)PcsSimpleV1Description.State.Off:
                case (int)PcsSimpleV1Description.State.Stop:
                case (int)PcsSimpleV1Description.State.Fault:
                    State = PcsState.Stopped;
                    break;
                case (int)PcsSimpleV1Description.State.Standby:
                    State = PcsState.Standby;
                    break;
                case (int)PcsSimpleV1Description.State.NightMode:
                    State = PcsState.NightMode;
                    break;
                default:
                    // Do not change the state
                    break;
            }
        }


        /// <summary>
        /// Sets the warning states.
        /// </summary>
        /// <param name="warning">The warning value.</param>
        /// <remarks>
        /// Keep it simple in simple proxy
        /// </remarks>
        private void SetWarning(int warning)
        {
            if (Enum.IsDefined(typeof(PcsSimpleV1Description.WarningCode), warning) == false)
            {
                _logger.LogError("{Name} Unknow warning code found: {Warning}", Name, warning);
                WarningStates.Add("Unknown", true);
            }
            else
            {
                PcsSimpleV1Description.WarningCode code = (PcsSimpleV1Description.WarningCode)warning;

                if (code != PcsSimpleV1Description.WarningCode.None)
                {
                    SetWarning(code.ToString(), true);
                }
            }
        }


        /// <summary>
        /// Sets the fault states.
        /// </summary>
        /// <param name="fault">The fault value.</param>
        /// <remarks>
        /// Keep it simple in simple proxy
        /// </remarks>
        private void SetFault(int fault)
        {
            if (Enum.IsDefined(typeof(PcsSimpleV1Description.FaultCode), fault) == false)
            {
                _logger.LogError("{Name} Unknow fault code found: {Fault}", Name, fault);
                FaultStates.Add("Unknown", true);
            }
            else
            {
                PcsSimpleV1Description.FaultCode code = (PcsSimpleV1Description.FaultCode)fault;

                if (code != PcsSimpleV1Description.FaultCode.None)
                {
                    SetFault(code.ToString(), true);
                }
            }
        }


        /// <summary>
        /// Sets the vendor events.
        /// </summary>
        /// <param name="vendorEvent">The vendor event value.</param>
        /// <remarks>
        /// Keep it simple in simple proxy
        /// </remarks>
        private void SetVendorEvent(int vendorEvent)
        {
            if (Enum.IsDefined(typeof(PcsSimpleV1Description.VendorEvents), vendorEvent) == false)
            {
                _logger.LogError("{Name} Unknow vendor event found: {VendorEvent}", Name, vendorEvent);
                VendorEvents.Add("Unknown", true);
            }
            else
            {
                PcsSimpleV1Description.VendorEvents code = (PcsSimpleV1Description.VendorEvents)vendorEvent;

                if (code != PcsSimpleV1Description.VendorEvents.None)
                {
                    SetVendorEvent(code.ToString(), true);
                }
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
        ///  Triggered on client communication error.
        /// </summary>
        /// <param name="sender">The communication client.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCommunicationError(object? sender, EventArgs e)
        {
            // Logging and even invocation is done in the setter of the CommunicationError property
            CommunicationError = true;
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

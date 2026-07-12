// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Communication.Protocols.Modbus;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Equipments.Modbus;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples
{
    /// <summary>
    /// BB simple is a basic implementation of the battery bank base class <see cref="BatteryBankBase"/>.
    /// It shall be used for tests, simulations, analysis and as a reference for all concrete implementations.
    /// </summary>
    public class BbSimpleProxy : BatteryBankBase, IModbusPollingEngine
    {
        /// <summary>
        /// Modbus client communication.
        /// </summary>
        private readonly IModbusClient _client;


        /// <summary>
        /// The Modbus polling engine.
        /// </summary>
        private readonly ModbusPollingEngine _pollingEngine;


        /// <summary>
        /// Constructor of <see cref="BbSimpleProxy"/>
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The battery bank configuration.</param>
        /// <param name="batteryStorageUnit">The paren battery storage unit.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        /// <param name="client">The Modbus client interface.</param>
        public BbSimpleProxy(ILogger logger, BatteryBankConfig config, DerBatteryStorageUnit batteryStorageUnit, IMetricsPublisher publisher,
            IModbusDataface dataface, IModbusClient client) : base(logger, config, batteryStorageUnit, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(dataface);

            _client = client;
            _pollingEngine = new ModbusPollingEngine(client, dataface);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task PollAsync(int currentInterval, CancellationToken cancellationToken)
        {
            await _pollingEngine.PollAsync(currentInterval, cancellationToken);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task ConnectAsync()
        {
            await base.ConnectAsync();
            await _client.WriteSingleRegisterAsync((ushort)BbSimpleDescription.Register.SelectorState, 1);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task DisconnectAsync()
        {
            await base.DisconnectAsync();
            await _client.WriteSingleRegisterAsync((ushort)BbSimpleDescription.Register.SelectorState, 0);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterDataface()
        {
            // Current, Voltage
            Dataface.Register<BbSimpleProxy, IModbusRegister>(r => r.Register<BbSimpleProxy, double?>(this, nameof(TotalDCVoltage),
                (x, v) => x.TotalDCVoltage = v, (int)BbSimpleDescription.Register.TotalDCVoltage, ModbusScale.NoScale, ModbusDataType.MbUint16));
            Dataface.Register<BbSimpleProxy, IModbusRegister>(r => r.Register<BbSimpleProxy, double?>(this, nameof(TotalDCCurrent),
                (x, v) => x.TotalDCCurrent = v, (int)BbSimpleDescription.Register.TotalDCCurrent, ModbusScale.NoScale, ModbusDataType.MbUint16));
            // State, Warnings, Faults, VendorEvents
            Dataface.Register<BbSimpleProxy, IModbusRegister>(r => r.Register<BbSimpleProxy, int>(this, nameof(State),
                (x, v) => x.SetState(v), (int)PcsSimpleV1Description.Register.CurrentState, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<BbSimpleProxy, IModbusRegister>(r => r.Register<BbSimpleProxy, int>(this, nameof(WarningStates),
                (x, v) => x.SetWarning(v), (int)PcsSimpleV1Description.Register.CurrentWarning, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<BbSimpleProxy, IModbusRegister>(r => r.Register<BbSimpleProxy, int>(this, nameof(FaultStates),
                (x, v) => x.SetFault(v), (int)PcsSimpleV1Description.Register.CurrentFault, ModbusScale.NoScale, ModbusDataType.MbInt16));
            Dataface.Register<BbSimpleProxy, IModbusRegister>(r => r.Register<BbSimpleProxy, int>(this, nameof(VendorEvents),
                (x, v) => x.SetVendorEvent(v), (int)PcsSimpleV1Description.Register.CurrentVendorEvent, ModbusScale.NoScale, ModbusDataType.MbInt16));
            // State of charge and health
            Dataface.Register<BbSimpleProxy, IModbusRegister>(r => r.Register<BbSimpleProxy, double?>(this, nameof(StateOfCharge),
                (x, v) => x.StateOfCharge = v, (int)BbSimpleDescription.Register.TotalStateOfCharge, ModbusScale.Factor100, ModbusDataType.MbUint16));
            Dataface.Register<BbSimpleProxy, IModbusRegister>(r => r.Register<BbSimpleProxy, double?>(this, nameof(StateOfHealth),
                (x, v) => x.StateOfHealth = v, (int)BbSimpleDescription.Register.TotalStateOfHealth, ModbusScale.Factor100, ModbusDataType.MbUint16));
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
            if (Enum.IsDefined(typeof(BbSimpleDescription.State), state) == false)
            {
                _logger.LogError("{Name} Undefined state reported. State: {State}", Name, State);
            }

            switch (state)
            {
                case (int)BbSimpleDescription.State.Disconnected:
                    State = BatteryBankState.Disconnected;
                    BatteryBankFlowDirection = Domain.EnergyStorages.Batteries.BatteryBankFlowDirection.Idle;
                    break;
                case (int)BbSimpleDescription.State.Connected:
                case (int)BbSimpleDescription.State.Idle:
                    State = BatteryBankState.Connected;
                    BatteryBankFlowDirection = Domain.EnergyStorages.Batteries.BatteryBankFlowDirection.Idle;
                    break;
                case (int)BbSimpleDescription.State.Discharging:
                    State = BatteryBankState.Connected;
                    BatteryBankFlowDirection = Domain.EnergyStorages.Batteries.BatteryBankFlowDirection.Discharging;
                    break;
                case (int)BbSimpleDescription.State.Charging:
                    State = BatteryBankState.Connected;
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
        /// <param name="warning">The warning value.</param>
        /// <remarks>
        /// Keep it simple in simple proxy
        /// </remarks>
        private void SetWarning(int warning)
        {
            if (Enum.IsDefined(typeof(BbSimpleDescription.WarningCode), warning) == false)
            {
                _logger.LogError("{Name} Unknow warning code found: {Warning}", Name, warning);
                WarningStates.Add("Unknown", true);
            }
            else
            {
                BbSimpleDescription.WarningCode code = (BbSimpleDescription.WarningCode)warning;

                if (code != BbSimpleDescription.WarningCode.None)
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
            if (Enum.IsDefined(typeof(BbSimpleDescription.FaultCode), fault) == false)
            {
                _logger.LogError("{Name} Unknow fault code found: {Fault}", Name, fault);
                FaultStates.Add("Unknown", true);
            }
            else
            {
                BbSimpleDescription.FaultCode code = (BbSimpleDescription.FaultCode)fault;

                if (code != BbSimpleDescription.FaultCode.None)
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
            if (Enum.IsDefined(typeof(BbSimpleDescription.VendorEvents), vendorEvent) == false)
            {
                _logger.LogError("{Name} Unknow vendor event found: {VendorEvent}", Name, vendorEvent);
                VendorEvents.Add("Unknown", true);
            }
            else
            {
                BbSimpleDescription.VendorEvents code = (BbSimpleDescription.VendorEvents)vendorEvent;

                if (code != BbSimpleDescription.VendorEvents.None)
                {
                    SetVendorEvent(code.ToString(), true);
                }
            }
        }
    }
}

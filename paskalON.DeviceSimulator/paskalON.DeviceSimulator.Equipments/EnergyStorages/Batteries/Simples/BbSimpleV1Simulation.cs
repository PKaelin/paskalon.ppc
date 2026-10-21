// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples
{
    /// <summary>
    /// Simulates the register behavior of a Battery Bank Simple V1.
    /// </summary>
    public sealed class BbSimpleV1Simulation : BatteryBankSimulationBase
    {
        /// <summary>
        /// Constructor of <see cref="BbSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="device">Simulated device.</param>
        /// <param name="customs">Simulator custom configurations of the device.</param>
        public BbSimpleV1Simulation(SimulationModbusDataStore store, BbSimpleV1Proxy device, IReadOnlyDictionary<string, string> customs)
            : this(store, BatteryBankSimulationSettings.FromDevice(device, customs))
        {
        }


        /// <summary>
        /// Constructor of <see cref="BbSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="settings">Nameplate and simulation settings.</param>
        public BbSimpleV1Simulation(SimulationModbusDataStore store, BatteryBankSimulationSettings settings)
            : base(store, settings, (int)BbSimpleV1Description.Register.CurrentWarning, (int)BbSimpleV1Description.Register.CurrentFault,
                  (int)BbSimpleV1Description.Register.CurrentVendorEvent)
        {
        }


        /// <inheritdoc/>
        protected override bool? ReadConnectCommand()
        {
            ushort selector = RegisterSimulation.Read(Store.Device, (int)BbSimpleV1Description.Register.SelectorState);

            switch (selector)
            {
                case (ushort)BbSimpleV1Description.State.Disconnected:
                    return false;
                case (ushort)BbSimpleV1Description.State.Connected:
                    return true;
                default:
                    return null;
            }
        }


        /// <inheritdoc/>
        protected override void WriteTelemetry(BatteryBankSimulationTelemetry telemetry)
        {
            RegisterSimulation.Write(Store.Device, (int)BbSimpleV1Description.Register.CurrentState, (ushort)GetState(telemetry));
            RegisterSimulation.Write(Store.Device, (int)BbSimpleV1Description.Register.TotalStateOfCharge, telemetry.StateOfCharge,
                ModbusDataType.MbUint16, ModbusScale.Upscale100);
            RegisterSimulation.Write(Store.Device, (int)BbSimpleV1Description.Register.TotalStateOfHealth, telemetry.StateOfHealth,
                ModbusDataType.MbUint16, ModbusScale.Upscale100);
            RegisterSimulation.Write(Store.Device, (int)BbSimpleV1Description.Register.TotalDCVoltage, telemetry.DcVoltage,
                ModbusDataType.MbUint16, ModbusScale.NoScale);
            RegisterSimulation.Write(Store.Device, (int)BbSimpleV1Description.Register.TotalDCCurrent, telemetry.DcCurrent,
                ModbusDataType.MbInt16, ModbusScale.NoScale);
        }


        /// <summary>
        /// Maps the simulated state and power flow to the device state.
        /// </summary>
        private static BbSimpleV1Description.State GetState(BatteryBankSimulationTelemetry telemetry)
        {
            if (telemetry.State != BatteryBankSimulationState.Connected)
            {
                return BbSimpleV1Description.State.Disconnected;
            }

            if (telemetry.ActivePower > 0)
            {
                return BbSimpleV1Description.State.Discharging;
            }

            if (telemetry.ActivePower < 0)
            {
                return BbSimpleV1Description.State.Charging;
            }

            return BbSimpleV1Description.State.Connected;
        }
    }
}

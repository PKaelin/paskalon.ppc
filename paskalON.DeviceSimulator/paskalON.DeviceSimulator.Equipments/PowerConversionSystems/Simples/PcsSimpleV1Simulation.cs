// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples
{
    /// <summary>
    /// Simulates the register behavior of a simple V1 PCS.
    /// </summary>
    /// <remarks>
    /// Power registers are in kilowatts / kilovolt-amperes reactive.
    /// </remarks>
    public sealed class PcsSimpleV1Simulation : PowerConversionSystemSimulationBase
    {
        /// <summary>
        /// Constructor of <see cref="PcsSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="device">Simulated device.</param>
        /// <param name="grid">Shared simulated grid.</param>
        public PcsSimpleV1Simulation(SimulationModbusDataStore store, PcsSimpleV1Proxy device, SimulatedGrid grid)
            : this(store, PowerConversionSystemSimulationSettings.FromDevice(device), grid)
        {
        }


        /// <summary>
        /// Constructor of <see cref="PcsSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="settings">Nameplate settings.</param>
        /// <param name="grid">Shared simulated grid.</param>
        public PcsSimpleV1Simulation(SimulationModbusDataStore store, PowerConversionSystemSimulationSettings settings, SimulatedGrid grid)
            : base(store, settings, grid, (int)PcsSimpleV1Description.Register.CurrentWarning, (int)PcsSimpleV1Description.Register.CurrentFault,
                  (int)PcsSimpleV1Description.Register.CurrentVendorEvent)
        {
        }


        /// <inheritdoc/>
        protected override PowerConversionSystemSimulationState? ReadCommand()
        {
            if (Store.TryTakeWrite((int)PcsSimpleV1Description.Register.SelectorState) is false)
            {
                return null;
            }

            ushort selector = RegisterSimulation.Read(Store.Device, (int)PcsSimpleV1Description.Register.SelectorState);

            switch (selector)
            {
                case (ushort)PcsSimpleV1Description.State.On:
                    return PowerConversionSystemSimulationState.On;
                case (ushort)PcsSimpleV1Description.State.Off:
                case (ushort)PcsSimpleV1Description.State.Stop:
                    return PowerConversionSystemSimulationState.Off;
                case (ushort)PcsSimpleV1Description.State.Standby:
                    return PowerConversionSystemSimulationState.Standby;
                default:
                    return null;
            }
        }


        /// <inheritdoc/>
        protected override (double ActivePower, double ReactivePower) ReadSetpoints()
        {
            double active = RegisterSimulation.Read<double>(Store.Device, (int)PcsSimpleV1Description.Register.PReference,
                ModbusDataType.MbInt16, ModbusScale.Upscale1000);
            double reactive = RegisterSimulation.Read<double>(Store.Device, (int)PcsSimpleV1Description.Register.QReference,
                ModbusDataType.MbInt16, ModbusScale.Upscale1000);

            return (active, reactive);
        }


        /// <inheritdoc/>
        protected override void WriteTelemetry(PowerConversionSystemSimulationTelemetry telemetry)
        {
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.CurrentState, (ushort)GetState(telemetry.State));
            // Power
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.P, telemetry.ActivePower, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.Q, telemetry.ReactivePower, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.PAvailable, telemetry.ActiveAvailablePower,
                ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.QAvailable, telemetry.ReactiveAvailablePower,
                ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            // Breakers and contactors
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.ACBreaker, (ushort)(telemetry.IsACBreakerClosed ? 1 : 0));
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.DcContactor, (ushort)(telemetry.IsDcContactorClosed ? 1 : 0));
            // Frequency, current and voltage
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.Frequency, telemetry.Frequency, ModbusDataType.MbUint16, ModbusScale.Upscale100);
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.DCCurrent, telemetry.DcCurrent, ModbusDataType.MbInt16, ModbusScale.NoScale);
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.DCVoltage, telemetry.DcVoltage, ModbusDataType.MbInt16, ModbusScale.NoScale);
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.ACCurrent, telemetry.AcCurrent, ModbusDataType.MbInt16, ModbusScale.NoScale);
            RegisterSimulation.Write(Store.Device, (int)PcsSimpleV1Description.Register.ACVoltage, telemetry.AcVoltage, ModbusDataType.MbInt16, ModbusScale.NoScale);
        }


        /// <summary>
        /// Maps the simulated state to the device state.
        /// </summary>
        private static PcsSimpleV1Description.State GetState(PowerConversionSystemSimulationState state)
        {
            switch (state)
            {
                case PowerConversionSystemSimulationState.On:
                    return PcsSimpleV1Description.State.On;
                case PowerConversionSystemSimulationState.Standby:
                    return PcsSimpleV1Description.State.Standby;
                case PowerConversionSystemSimulationState.Fault:
                    return PcsSimpleV1Description.State.Fault;
                default:
                    return PcsSimpleV1Description.State.Off;
            }
        }
    }
}

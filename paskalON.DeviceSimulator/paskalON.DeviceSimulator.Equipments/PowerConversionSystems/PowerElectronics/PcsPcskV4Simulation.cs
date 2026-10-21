// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Devices.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems.PowerElectronics
{
    /// <summary>
    /// Simulates the command and telemetry registers of a PCSK V4 PCS.
    /// </summary>
    /// <remarks>
    /// Power registers are in kilowatts / kilovolt-amperes reactive. The device has no vendor event register.
    /// </remarks>
    public sealed class PcsPcskV4Simulation : PowerConversionSystemSimulationBase
    {
        /// <summary>
        /// Selector value of the start command.
        /// </summary>
        private const ushort SelectorStart = 1;


        /// <summary>
        /// Selector value of the stop command.
        /// </summary>
        private const ushort SelectorStop = 0;


        /// <summary>
        /// Selector value of the standby command.
        /// </summary>
        private const ushort SelectorStandby = 3;


        /// <summary>
        /// Constructor of <see cref="PcsPcskV4Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="device">Simulated device.</param>
        /// <param name="grid">Shared simulated grid.</param>
        public PcsPcskV4Simulation(SimulationModbusDataStore store, PcsPcskV4Proxy device, SimulatedGrid grid)
            : base(store, PowerConversionSystemSimulationSettings.FromDevice(device), grid,
                  (int)PcsPcskV4Description.Register.CurrentWarning, (int)PcsPcskV4Description.Register.CurrentFault, null)
        {
        }


        /// <inheritdoc/>
        protected override PowerConversionSystemSimulationState? ReadCommand()
        {
            if (Store.TryTakeWrite((int)PcsPcskV4Description.Register.SelectorState) is false)
            {
                return null;
            }

            ushort selector = RegisterSimulation.Read(Store.Device, (int)PcsPcskV4Description.Register.SelectorState);

            switch (selector)
            {
                case SelectorStart:
                    return PowerConversionSystemSimulationState.On;
                case SelectorStop:
                    return PowerConversionSystemSimulationState.Off;
                case SelectorStandby:
                    return PowerConversionSystemSimulationState.Standby;
                default:
                    return null;
            }
        }


        /// <inheritdoc/>
        protected override (double ActivePower, double ReactivePower) ReadSetpoints()
        {
            double active = RegisterSimulation.Read<double>(Store.Device, (int)PcsPcskV4Description.Register.PReference,
                ModbusDataType.MbInt16, ModbusScale.Upscale1000);
            double reactive = RegisterSimulation.Read<double>(Store.Device, (int)PcsPcskV4Description.Register.QReference,
                ModbusDataType.MbInt16, ModbusScale.Upscale1000);

            return (active, reactive);
        }


        /// <inheritdoc/>
        protected override void WriteTelemetry(PowerConversionSystemSimulationTelemetry telemetry)
        {
            double apparentPower = Math.Sqrt(telemetry.ActivePower * telemetry.ActivePower + telemetry.ReactivePower * telemetry.ReactivePower);

            RegisterSimulation.Write(Store.Device, (int)PcsPcskV4Description.Register.CurrentState, (ushort)GetState(telemetry.State));
            RegisterSimulation.Write(Store.Device, (int)PcsPcskV4Description.Register.P, telemetry.ActivePower, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsPcskV4Description.Register.Q, telemetry.ReactivePower, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsPcskV4Description.Register.S, apparentPower, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsPcskV4Description.Register.PCapability, telemetry.ActiveAvailablePower,
                ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsPcskV4Description.Register.QCapability, telemetry.ReactiveAvailablePower,
                ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsPcskV4Description.Register.DcBreakerState, (ushort)(telemetry.IsDcContactorClosed ? 1 : 0));
        }


        /// <summary>
        /// Maps the simulated state to the device state.
        /// </summary>
        private static PcsPcskV4Description.State GetState(PowerConversionSystemSimulationState state)
        {
            switch (state)
            {
                case PowerConversionSystemSimulationState.On:
                    return PcsPcskV4Description.State.On;
                case PowerConversionSystemSimulationState.Standby:
                    return PcsPcskV4Description.State.Standby;
                case PowerConversionSystemSimulationState.Fault:
                    return PcsPcskV4Description.State.Fault;
                default:
                    return PcsPcskV4Description.State.Off;
            }
        }
    }
}

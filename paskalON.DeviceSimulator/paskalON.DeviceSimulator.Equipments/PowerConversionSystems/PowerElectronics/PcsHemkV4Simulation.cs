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
    /// Simulates the command and telemetry registers of a HEMK V4 PCS.
    /// </summary>
    /// <remarks>
    /// Power registers are in kilowatts / kilovolt-amperes reactive. The device has no vendor event register.
    /// The register addresses are taken from <see cref="PcsHemkV4Description"/> because PcsHemkV4Description is internal
    /// to paskalON.Devices.Equipments. Both descriptions currently define the same register map.
    /// </remarks>
    public sealed class PcsHemkV4Simulation : PowerConversionSystemSimulationBase
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
        /// Constructor of <see cref="PcsHemkV4Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="device">Simulated device.</param>
        /// <param name="grid">Shared simulated grid.</param>
        public PcsHemkV4Simulation(SimulationModbusDataStore store, PcsHemkV4Proxy device, SimulatedGrid grid)
            : base(store, PowerConversionSystemSimulationSettings.FromDevice(device), grid,
                  (int)PcsHemkV4Description.Register.CurrentWarning, (int)PcsHemkV4Description.Register.CurrentFault, null)
        {
        }


        /// <inheritdoc/>
        protected override PowerConversionSystemSimulationState? ReadCommand()
        {
            if (Store.TryTakeWrite((int)PcsHemkV4Description.Register.SelectorState) is false)
            {
                return null;
            }

            ushort selector = RegisterSimulation.Read(Store.Device, (int)PcsHemkV4Description.Register.SelectorState);

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
            double active = RegisterSimulation.Read<double>(Store.Device, (int)PcsHemkV4Description.Register.PReference,
                ModbusDataType.MbInt16, ModbusScale.Upscale1000);
            double reactive = RegisterSimulation.Read<double>(Store.Device, (int)PcsHemkV4Description.Register.QReference,
                ModbusDataType.MbInt16, ModbusScale.Upscale1000);

            return (active, reactive);
        }


        /// <inheritdoc/>
        protected override void WriteTelemetry(PowerConversionSystemSimulationTelemetry telemetry)
        {
            double apparentPower = Math.Sqrt(telemetry.ActivePower * telemetry.ActivePower + telemetry.ReactivePower * telemetry.ReactivePower);

            RegisterSimulation.Write(Store.Device, (int)PcsHemkV4Description.Register.CurrentState, (ushort)GetState(telemetry.State));
            RegisterSimulation.Write(Store.Device, (int)PcsHemkV4Description.Register.P, telemetry.ActivePower, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsHemkV4Description.Register.Q, telemetry.ReactivePower, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsHemkV4Description.Register.S, apparentPower, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsHemkV4Description.Register.PCapability, telemetry.ActiveAvailablePower,
                ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsHemkV4Description.Register.QCapability, telemetry.ReactiveAvailablePower,
                ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(Store.Device, (int)PcsHemkV4Description.Register.DcBreakerState, (ushort)(telemetry.IsDcContactorClosed ? 1 : 0));
        }


        /// <summary>
        /// Maps the simulated state to the device state.
        /// </summary>
        private static PcsHemkV4Description.State GetState(PowerConversionSystemSimulationState state)
        {
            switch (state)
            {
                case PowerConversionSystemSimulationState.On:
                    return PcsHemkV4Description.State.On;
                case PowerConversionSystemSimulationState.Standby:
                    return PcsHemkV4Description.State.Standby;
                case PowerConversionSystemSimulationState.Fault:
                    return PcsHemkV4Description.State.Fault;
                default:
                    return PcsHemkV4Description.State.Off;
            }
        }
    }
}

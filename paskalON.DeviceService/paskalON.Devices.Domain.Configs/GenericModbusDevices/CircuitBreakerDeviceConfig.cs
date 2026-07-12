// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Circuit breaker device configuration.
    /// </summary>
    public class CircuitBreakerDeviceConfig : NameBase
    {
        /// <summary>
        /// Relationship to GenericModbusMapConfigId.
        /// </summary>
        public int? GenericModbusMapConfigId { get; set; }


        /// <summary>
        /// Relationship to GenericModbusMapConfig.
        /// </summary>
        public GenericModbusMapConfig? GenericModbusMapConfig { get; set; }


        /// <summary>
        /// The class name of the.
        /// This uniquely identifies the eventually used type of component. (e.g. a ManufacturerPcs, ManufacturerBattery etc.).
        /// </summary>
        public required string ClassName { get; set; }


        /// <summary>
        /// Describes the operations that can be carried out on the machine from a remote controller.
        /// </summary>
        public CircuitBreakerOperation CircuitBreakerOperation { get; set; }


        /// <summary>
        /// The breaker state register configuration.
        /// </summary>
        public GenericModbusHoldingRegisterConfig? BreakerStateRegister { get; set; }


        /// <summary>
        /// The synchronous voltage register configuration.
        /// </summary>
        public GenericModbusHoldingRegisterConfig? SynchronousVoltage { get; set; }


        /// <summary>
        /// The breaker close command register configuration.
        /// </summary>
        public GenericModbusHoldingRegisterConfig? CloseCommand { get; set; }


        /// <summary>
        /// The breaker close command coil configuration.
        /// </summary>
        public GenericModbusCoilPointConfig? CloseCommandCoil { get; set; }


        /// <summary>
        /// The breaker open command register configuration.
        /// </summary>
        public GenericModbusHoldingRegisterConfig? OpenCommand { get; set; }


        /// <summary>
        /// The breaker open command coil configuration.
        /// </summary>
        public GenericModbusCoilPointConfig? OpenCommandCoil { get; set; }


        /// <summary>
        /// The settings group state register configuration.
        /// </summary>
        public GenericModbusHoldingRegisterConfig? SettingsGroupState { get; set; }

        /// <summary>
        /// The settings group command register configuration.
        /// </summary>
        public GenericModbusHoldingRegisterConfig? SettingsGroupCommand { get; set; }


        /// <summary>
        /// The trip state register configuration.
        /// </summary>
        public GenericModbusHoldingRegisterConfig? TripState { get; set; }


        /// <summary>
        /// The reset trip register configuration.
        /// </summary>
        public GenericModbusHoldingRegisterConfig? ResetTrip { get; set; }


        /// <summary>
        /// The open breaker delay in milliseconds.
        /// </summary>
        public int OpenBreakerDelayMilliseconds { get; set; } = 30000;
    }
}
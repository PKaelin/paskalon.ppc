// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Circuit Breaker generic Modbus device configuration.
    /// </summary>
    public class CircuitBreakerConfig : GenericModbusBaseConfig
    {
        /// <summary>
        /// Parent relationship to DerCircuitConfig Id.
        /// </summary>
        public int DerCircuitConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerCircuitConfig.
        /// </summary>
        public required DerCircuitConfig DerCircuitConfig { get; set; }


        /// <summary>
        /// Relationship to CircuitBreakerDeviceConfig Id.
        /// </summary>
        public int CircuitBreakerDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to CircuitBreakerDeviceConfig.
        /// </summary>
        public required CircuitBreakerDeviceConfig CircuitBreakerDeviceConfig { get; set; }
    }
}
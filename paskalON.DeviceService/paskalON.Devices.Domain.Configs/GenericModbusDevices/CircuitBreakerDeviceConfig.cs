
using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;
using System.ComponentModel.DataAnnotations;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
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
        [Required]
        public string? ClassName { get; set; }

        public CircuitBreakerOperation CircuitBreakerOperation { get; set; }

        public GenericModbusHoldingRegisterMapEntryConfig? BreakerStatusRegister { get; set; }

        public GenericModbusHoldingRegisterMapEntryConfig? SynchronousVoltage { get; set; }

        public GenericModbusHoldingRegisterMapEntryConfig? CloseCommand { get; set; }

        public GenericModbusCoilMapEntryConfig? CloseCommandCoil { get; set; }

        public GenericModbusHoldingRegisterMapEntryConfig? OpenCommand { get; set; }

        public GenericModbusCoilMapEntryConfig? OpenCommandCoil { get; set; }

        public GenericModbusHoldingRegisterMapEntryConfig? SettingsGroupStatus { get; set; }

        public GenericModbusHoldingRegisterMapEntryConfig? SettingsGroupCommand { get; set; }

        public GenericModbusHoldingRegisterMapEntryConfig? TripStatus { get; set; }

        public GenericModbusHoldingRegisterMapEntryConfig? ResetTrip { get; set; }

        public int OpenBreakerDelayMilliseconds { get; set; } = 30000;
    }
}
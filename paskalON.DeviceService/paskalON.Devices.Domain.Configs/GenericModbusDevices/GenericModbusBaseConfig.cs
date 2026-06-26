using System.ComponentModel.DataAnnotations;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    public abstract class GenericModbusBaseConfig : ModbusConfig
    {
        /// <summary>
        /// Id of the generic device.
        /// </summary>
        [Required]
        public required int DeviceId
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
            }
        }


        /// <summary>
        /// Whether this device is active meaning whether it should be loaded into configuration.
        /// </summary>
        [Required]
        public required bool IsActive { get; set; }
    }
}

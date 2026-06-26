using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// Base class for all classes that require a device ID and name.
    /// </summary>
    [Index(nameof(DeviceId), IsUnique = true)]
    public abstract class DeviceIdNameBase : NameBase
    {
        /// <summary>
        /// Id of the device.
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
        /// Gets or sets whether this device is active meaning whether it should be loaded into configuration.
        /// </summary>
        [Required]
        public required bool IsActive { get; set; }
    }
}

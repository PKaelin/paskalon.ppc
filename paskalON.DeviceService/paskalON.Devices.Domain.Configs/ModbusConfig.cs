
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;


namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// TCP addressable device with Modbus connection.
    /// All device properties should be on the device definition itself rather on this interpretation.
    /// </summary>
    public class ModbusConfig : NameBase
    {
        /// <summary>
        /// Relationship to ModbusConnectionConfig Id.
        /// </summary>
        public int ModbusConnectionConfigId { get; set; }


        /// <summary>
        /// Relationship to ModbusConnectionConfig.
        /// </summary>
        [Required]
        public required ModbusConnectionConfig? ModbusConnectionConfig { get; set; }



        /// <summary>
        /// Host address.
        /// </summary>
        [Required]
        public required string Address { get; set; }


        /// <summary>
        /// Port.
        /// </summary>
        [Required]
        public required int Port { get; set; }


        /// <summary>
        /// Address family to connect with. Default is IP4.
        /// </summary>
        [Required]
        public required AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;


        /// <summary>
        /// The station Id.
        /// </summary>
        [Required]
        public required byte StationId { get; set; } = 1;



        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Name} {Address}:{Port}";
        }


    }
}

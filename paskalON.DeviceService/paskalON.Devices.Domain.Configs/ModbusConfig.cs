// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;
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
        public required ModbusConnectionConfig ModbusConnectionConfig { get; set; }


        /// <summary>
        /// Host address.
        /// </summary>
        public required string Address
        {
            get;
            set { ArgumentNullException.ThrowIfNullOrEmpty(value); field = value; }
        }


        /// <summary>
        /// Port.
        /// </summary>
        public required int Port
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value); field = value; }
        }


        /// <summary>
        /// Address family to connect with. Default is IP4.
        /// </summary>
        public required AddressFamily AddressFamily { get; set; } = AddressFamily.InterNetwork;


        /// <summary>
        /// The Modbus unit Id.
        /// </summary>
        public required byte UnitId { get; set; } = 1;



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

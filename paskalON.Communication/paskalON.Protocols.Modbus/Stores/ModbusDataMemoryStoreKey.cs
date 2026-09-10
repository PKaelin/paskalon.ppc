// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.Modbus.Stores
{
    /// <summary>
    /// Modbus data key to identify Modbus data.
    /// </summary>
    public record ModbusDataMemoryStoreKey
    {
        /// <summary>
        /// Address of the Modbus server.
        /// </summary>
        public string Address { get; init; } = string.Empty;


        /// <summary>
        /// Port of the Modbus.
        /// </summary>
        public int Port { get; init; }


        /// <summary>
        /// Unit ID of the Modbus.
        /// </summary>
        public byte UnitId { get; init; } = 1;
    }
}

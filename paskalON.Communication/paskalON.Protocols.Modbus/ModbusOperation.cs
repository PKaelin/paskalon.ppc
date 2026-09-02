// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.Modbus
{
    /// <summary>
    /// Modbus operation type.
    /// </summary>
    internal enum ModbusOperation
    {
        /// <summary>
        /// Write operation.
        /// </summary>
        Write = 0,
        /// <summary>
        /// Read operation.
        /// </summary>
        Read = 1
    }
}

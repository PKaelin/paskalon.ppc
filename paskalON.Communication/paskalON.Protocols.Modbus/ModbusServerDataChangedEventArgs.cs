// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.Protocols.Modbus
{
    /// <summary>
    /// Raised by <see cref="IModbusServer"/> whenever a remote master writes to one of the server's data tables
    /// (e.g. a client called WriteSingleRegister/WriteMultipleCoils against this server).
    /// </summary>
    public class ModbusServerDataChangedEventArgs
    {
        /// <summary>
        /// Modbus register type that was changed.
        /// </summary>
        public required ModbusRegistryType RegisterType { get; init; }

        /// <summary>
        /// Starting address of the changed registers.
        /// </summary>
        public required ushort StartAddress { get; init; }


        /// <summary>
        /// Number of registers that were changed.
        /// </summary>
        public required int Count { get; init; }
    }
}

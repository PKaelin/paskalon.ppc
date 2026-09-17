// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.DeviceSimulator.Dto.Endpoints
{
    /// <summary>
    /// Modbus data endpoint.
    /// </summary>
    public record ModbusEndpointDto
    {
        /// <summary>
        /// Name of the endpoint.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Modbus address.
        /// </summary>
        public required ushort Address { get; init; }


        /// <summary>
        /// Modbus data type of the endpoint.
        /// </summary>
        public required ModbusDataType DataType { get; init; }


        /// <summary>
        /// The parsed endpoint value.
        /// </summary>
        public required object Value { get; init; }


        /// <summary>
        /// Flag whether this endpoint is read only (true) or writeable (false)
        /// </summary>
        public bool IsReadOnly { get; init; } = true;
    }
}

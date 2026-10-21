// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using System.Text.Json.Serialization;

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
        /// <remarks>
        /// Serialized as the enum name (e.g. MbInt16, MbFloatLe) instead of its number.
        /// </remarks>
        [JsonConverter(typeof(JsonStringEnumConverter<ModbusDataType>))]
        public required ModbusDataType DataType { get; init; }


        /// <summary>
        /// The parsed endpoint value.
        /// </summary>
        public required object Value
        {
            get
            {
                return field switch
                {
                    double d => Math.Round(d, 3),
                    float f => Math.Round(f, 3),
                    decimal m => Math.Round(m, 3),
                    _ => field // Return as is if it's an int, string, bool, etc.
                };
            }
            init;
        }


        /// <summary>
        /// Flag whether this endpoint is read only (true) or writeable (false)
        /// </summary>
        public bool IsReadOnly { get; init; } = true;
    }
}

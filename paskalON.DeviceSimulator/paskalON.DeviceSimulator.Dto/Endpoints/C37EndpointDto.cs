// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;

namespace paskalON.DeviceSimulator.Dto.Endpoints
{
    /// <summary>
    /// C37 data endpoint.
    /// </summary>
    public record C37EndpointDto
    {
        /// <summary>
        /// Name of the property.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Name of the endpoint.
        /// </summary>
        public required string Endpoint { get; init; }


        /// <summary>
        /// C37 data type of the endpoint.
        /// </summary>
        public required C37SignalType SignalType { get; init; }


        /// <summary>
        /// The endpoint value.
        /// </summary>
        public required double? Value
        {
            get { return field == null ? null : Math.Round((double)field, 3); }
            init;
        }


        /// <summary>
        /// Flag whether this endpoint is read only (true) or writeable (false)
        /// </summary>
        public bool IsReadOnly { get; init; } = true;
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Communication.Protocols.C37118.Types;
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// TCP addressable device with C37 connection.
    /// </summary>
    public class C37Config : NameBase
    {
        /// <summary>
        /// IP address of the device.
        /// </summary>
        public required string Address
        {
            get;
            set { ArgumentNullException.ThrowIfNullOrEmpty(value); field = value; }
        }


        /// <summary>
        /// Port of the device.
        /// </summary>
        public required ushort Port
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }


        /// <summary>
        /// Transport layer (TCP or UDP) used for C37.118 communications.
        /// </summary>
        public required C37TransportLayer TransportLayer { get; set; }


        /// <summary>
        /// The station name of the C37 data stream which identifies the phasor measurement unit (PMU) or phasor data contractor (PDC).
        /// </summary>
        public required string StationName { get; set; } = "PMU";


        /// <summary>
        /// The stream Id of the data block within the C37 data stream.
        /// </summary>
        /// <remarks>
        /// A device may be acting as a phasor data concentrator (PDC) which means that the C37 data stream will contain
        /// data off of multiple micro PMUs. In this case, the data within the data stream is identified using this value.
        /// </remarks>
        public required ushort StreamId { get; set; } = 1;


        /// <summary>
        /// Timeout duration for receiving config frames in C37.118 stream.
        /// </summary>
        public int ConfigFrameTimeoutMilliseconds { get; set; } = 2000;


        /// <summary>
        /// Timeout duration for receiving data frames in C37.118 stream.
        /// </summary>
        public int DataFrameTimeoutMilliseconds { get; set; } = 500;


        /// <summary>
        /// Number of times to reset <see cref="DataFrameTimeoutMilliseconds"/> before raising a comm error.
        /// </summary>
        public ushort DataFrameRetryCount { get; set; } = 2;


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

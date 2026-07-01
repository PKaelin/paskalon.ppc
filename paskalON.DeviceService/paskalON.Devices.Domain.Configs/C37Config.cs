using paskalON.Communication.Protocols.C37118.Types;

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
        public string? IpAddress
        {
            get;
            set { ArgumentNullException.ThrowIfNullOrEmpty(value); field = value; }
        }


        /// <summary>
        /// Port of the device.
        /// </summary>
        public ushort Port
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }


        /// <summary>
        /// Transport layer (TCP or UDP) used for C37.118 communications.
        /// </summary>
        public C37TransportLayer TransportLayer { get; set; }


        /// <summary>
        /// The ID of the C37 Data Stream.
        /// </summary>
        /// <remarks>
        /// The remote device may have multiple C37 Data Streams.
        /// This ID ensures the DG-IC is looking at the correct data stream. This value is used for verification
        /// purposes, and it is sent to the remote device in various command frames so the remote device can
        /// perform that command on it's appropriate data stream.
        /// </remarks>
        public ushort IdOfDataStream { get; set; } = 1;


        /// <summary>
        /// The ID of the data block within the C37 Data Stream.
        /// </summary>
        /// <remarks>
        /// A remote device may be acting as a "Phasor Data Concentrator," or PDC, which means that the C37 Data Stream will contain
        /// data off of multiple micro PMUs. In this case, the data within the Data Stream is identified using this value.
        /// 
        /// For devices that are not acting as a PDC, it is "usually" the case that this will match the <see cref="IdOfDataStream"/>, 
        /// however this is not enforced by the C37 protocol.
        /// </remarks>
        public ushort IdOfDataBlock { get; set; } = 1;


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
            return $"{Name} {IpAddress}:{Port}";
        }

    }
}

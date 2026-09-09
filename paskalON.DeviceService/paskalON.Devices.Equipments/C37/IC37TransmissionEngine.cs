// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Equipments.C37
{
    public interface IC37TransmissionEngine
    {
        /// <summary>
        /// C37 stream id within the C37 data stream.
        /// </summary>
        /// <remarks>
        /// This identifies the PMU.
        /// </remarks>
        ushort StreamId { get; }


        /// <summary>
        /// C37 destination address (IP or hostname).
        /// </summary>
        string DestinationAddress { get; }


        /// <summary>
        /// C37 destination port.
        /// </summary>
        int DestinationPort { get; }


        /// <summary>
        /// Current registered mappings that were generated via the configuration frame.
        /// </summary>
        List<C37RegisterMapEntry> Mappings { get; }


        /// <summary>
        /// Start streaming.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token.</param>
        /// <returns>Task</returns>
        Task StartStreaming(CancellationToken stoppingToken);
    }
}

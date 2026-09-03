// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Equipments.C37
{
    public interface IC37TransmissionEngine
    {
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

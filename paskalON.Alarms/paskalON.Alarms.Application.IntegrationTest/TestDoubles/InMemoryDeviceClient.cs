// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Client;
using paskalON.Devices.Client.Subscribers;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Alarms.Application.IntegrationTest.TestDoubles
{
    /// <summary>
    /// Device client with devices kept in memory instead of loaded from the device service (external boundary of the integration tests).
    /// </summary>
    public class InMemoryDeviceClient : IDeviceClient
    {
        /// <inheritdoc/>
        /// <remarks>
        /// Never raised, the alarm service does not subscribe the device data with the device client.
        /// </remarks>
        public event EventHandler? CoreDataReceived
        {
            add { }
            remove { }
        }


        /// <inheritdoc/>
        public DerDto Der { get; } = new DerDto { Name = "Test" };


        /// <inheritdoc/>
        public ICollection<PcsDto> PowerConversionSystems { get; } = new List<PcsDto>();


        /// <inheritdoc/>
        public ICollection<BbDto> BatteryBanks { get; } = new List<BbDto>();


        /// <inheritdoc/>
        public ICollection<PvDto> SolarPanels { get; } = new List<PvDto>();


        /// <inheritdoc/>
        public ICollection<PmExternalDto> ExternalPowerMeters { get; } = new List<PmExternalDto>();


        /// <inheritdoc/>
        public ICollection<PmAuxiliaryDto> AuxiliaryPowerMeters { get; } = new List<PmAuxiliaryDto>();


        /// <inheritdoc/>
        public ICollection<PmSystemDto> SystemPowerMeters { get; } = new List<PmSystemDto>();


        /// <inheritdoc/>
        public ICollection<PmCircuitDto> CircuitPowerMeters { get; } = new List<PmCircuitDto>();


        /// <inheritdoc/>
        public Task Initialize(SubscriberTopic subscriberTopic)
        {
            return Task.CompletedTask;
        }
    }
}

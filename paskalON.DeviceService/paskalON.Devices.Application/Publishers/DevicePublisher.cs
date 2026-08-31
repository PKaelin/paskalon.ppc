// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.Messaging;
using System.Text.Json;

namespace paskalON.Devices.Application.Publishers
{
    /// <summary>
    /// Publishes device data to a message publisher interface.
    /// </summary>
    public class DevicePublisher : IDevicePublisher
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Device manager interface that holds the domain objects.
        /// </summary>
        private readonly IDeviceManager _deviceManager;


        /// <summary>
        /// Maps domain device objects to their DTO representations.
        /// </summary>
        private readonly DeviceMapper _mapper;


        /// <summary>
        /// Interface for publishing to message brokers.
        /// </summary>
        private readonly IMessagePublisher _publisher;


        /// <summary>
        /// Message publisher topic.
        /// </summary>        
        private readonly PublisherTopic _publisherTopics;


        /// <summary>
        /// Interval to publish core part of the DTOs.
        /// </summary>
        private readonly int _coreInterval;


        /// <summary>
        /// Interval to publish detail part of the DTOs.
        /// </summary>
        private readonly int _detailInterval;


        /// <summary>
        /// Constructor of <see cref="DevicePublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="deviceManager"> Device manager interface that holds the domain objects.</param>
        /// <param name="mapper">Maps domain device objects to their DTO representations.</param>
        /// <param name="publisher">Interface for publishing to message brokers.</param>
        /// <param name="topics">Message publisher topic.</param>
        /// <param name="coreInterval">Interval to publish core part of the DTOs.</param>
        /// <param name="detailInterval">Interval to publish detail part of the DTOs.</param>
        public DevicePublisher(ILogger<DevicePublisher> logger, IDeviceManager deviceManager, DeviceMapper mapper,
            IMessagePublisher publisher, PublisherTopic topics, int coreInterval, int detailInterval)
        {
            ArgumentNullException.ThrowIfNull(deviceManager);
            ArgumentNullException.ThrowIfNull(mapper);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(topics);
            ArgumentNullException.ThrowIfNull(logger);

            _deviceManager = deviceManager;
            _mapper = mapper;
            _publisher = publisher;
            _publisherTopics = topics;
            _logger = logger;
            _coreInterval = coreInterval;
            _detailInterval = detailInterval;
        }


        /// <summary>
        /// Publishes the DTO parts depending on their interval.
        /// </summary>
        /// <param name="currentInterval">The current interval.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous publish operation.</returns>
        public async Task Publish(int currentInterval)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(currentInterval);

            await PublishHardCoreLoop();

            if (currentInterval % _coreInterval == 0)
            {
                await PublishCoreLoop();
            }

            if (currentInterval % _detailInterval == 0)
            {
                await PublishDetailLoop();
            }
        }



        /// <summary>
        /// Publishes the core part of the DTOs in every loop.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous publish operation.</returns>
        private async Task PublishHardCoreLoop()
        {
            foreach (SystemPowerMeter meter in _deviceManager.SystemPowerMeters)
            {
                PmSystemCoreDto dto = _mapper.MapPowerMeterCore<PmSystemCoreDto>(meter);
                await Publish(_publisherTopics.SystemPowerMeterTopic?.CoreTopic, dto);
            }

            foreach (CircuitPowerMeter meter in _deviceManager.CircuitPowerMeters)
            {
                PmCircuitCoreDto dto = _mapper.MapPowerMeterCore<PmCircuitCoreDto>(meter);
                await Publish(_publisherTopics.CircuitPowerMeterTopic?.CoreTopic, dto);
            }
        }


        /// <summary>
        /// Publishes the core part of the DTOs.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous publish operation.</returns>
        private async Task PublishCoreLoop()
        {
            foreach (PowerConversionSystemBase device in _deviceManager.PowerConversionSystems)
            {
                PcsCoreDto dto = _mapper.MapPowerConversionSystemCore(device);
                await Publish(_publisherTopics.PowerConversionSystemTopic?.CoreTopic, dto);
            }

            foreach (BatteryBankBase device in _deviceManager.BatteryBanks)
            {
                BbCoreDto dto = _mapper.MapBatteryBankCore(device);
                await Publish(_publisherTopics.BatteryBankTopic?.CoreTopic, dto);
            }

            foreach (SolarPanelBase device in _deviceManager.SolarPanels)
            {
                PvCoreDto dto = _mapper.MapSolarPanelCore(device);
                await Publish(_publisherTopics.SolarPanelTopic?.CoreTopic, dto);
            }

            foreach (ExternalPowerMeter meter in _deviceManager.ExternalPowerMeters)
            {
                PmExternalCoreDto dto = _mapper.MapPowerMeterCore<PmExternalCoreDto>(meter);
                await Publish(_publisherTopics.ExternalPowerMeterTopic?.CoreTopic, dto);
            }

            foreach (AuxiliaryPowerMeter meter in _deviceManager.AuxiliaryPowerMeters)
            {
                PmAuxiliaryCoreDto dto = _mapper.MapPowerMeterCore<PmAuxiliaryCoreDto>(meter);
                await Publish(_publisherTopics.AuxiliaryPowerMeterTopic?.CoreTopic, dto);
            }
        }


        /// <summary>
        /// Publishes detail part of the DTOs.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous publish operation.</returns>
        private async Task PublishDetailLoop()
        {
            foreach (SystemPowerMeter meter in _deviceManager.SystemPowerMeters)
            {
                PmSystemDetailDto dto = _mapper.MapPowerMeterDetail<PmSystemDetailDto>(meter);
                await Publish(_publisherTopics.SystemPowerMeterTopic?.DetailTopic, dto);
            }

            foreach (CircuitPowerMeter meter in _deviceManager.CircuitPowerMeters)
            {
                PmCircuitDetailDto dto = _mapper.MapPowerMeterDetail<PmCircuitDetailDto>(meter);
                await Publish(_publisherTopics.CircuitPowerMeterTopic?.DetailTopic, dto);
            }

            foreach (PowerConversionSystemBase device in _deviceManager.PowerConversionSystems)
            {
                PcsDetailDto dto = _mapper.MapPowerConversionSystemDetail(device);
                await Publish(_publisherTopics.PowerConversionSystemTopic?.DetailTopic, dto);
            }

            foreach (BatteryBankBase device in _deviceManager.BatteryBanks)
            {
                BbDetailDto dto = _mapper.MapBatteryBankDetail(device);
                await Publish(_publisherTopics.BatteryBankTopic?.DetailTopic, dto);
            }

            foreach (SolarPanelBase device in _deviceManager.SolarPanels)
            {
                PvDetailDto dto = _mapper.MapSolarPanelDetail(device);
                await Publish(_publisherTopics.SolarPanelTopic?.DetailTopic, dto);
            }

            foreach (ExternalPowerMeter meter in _deviceManager.ExternalPowerMeters)
            {
                PmExternalDetailDto dto = _mapper.MapPowerMeterDetail<PmExternalDetailDto>(meter);
                await Publish(_publisherTopics.ExternalPowerMeterTopic?.DetailTopic, dto);
            }

            foreach (AuxiliaryPowerMeter meter in _deviceManager.AuxiliaryPowerMeters)
            {
                PmAuxiliaryDetailDto dto = _mapper.MapPowerMeterDetail<PmAuxiliaryDetailDto>(meter);
                await Publish(_publisherTopics.AuxiliaryPowerMeterTopic?.DetailTopic, dto);
            }
        }


        /// <summary>
        /// Publish data to the message publisher interface.
        /// </summary>
        /// <typeparam name="T">Type of the message to publish.</typeparam>
        /// <param name="topic">Topic to publish message to.</param>
        /// <param name="message">The message object.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous publish operation.</returns>
        private async Task Publish<T>(string? topic, T message)
        {
            if (string.IsNullOrWhiteSpace(topic) == false)
            {
                try
                {
                    string json = JsonSerializer.Serialize(message);
                    await _publisher.PublishAsync(topic, json);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unexpected error publishing device data. {Error}:", ex.Message);
                }
            }
        }
    }
}

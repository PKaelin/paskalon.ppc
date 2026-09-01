// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Application;
using paskalON.Devices.Client.Registers;
using paskalON.Devices.Client.Subscribers;
using paskalON.Devices.Dto;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.Messaging;

namespace paskalON.Devices.Client
{
    /// <summary>
    /// Device client is a reusable client for different microservices that need to subscribe
    /// to the device services published messages.
    /// </summary>
    public class DeviceClient : IDeviceClient
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Device servicer interface for API calls to the device service.
        /// </summary>
        private readonly IDeviceServer _deviceServer;


        /// <summary>
        /// Message subscriber interface for API calls to the message subscriber.
        /// </summary>
        private readonly IMessageSubscriber _subscriber;


        /// <summary>
        /// Subscriber topic class containing all the subscriber topics used in device client.
        /// </summary>
        private readonly SubscriberTopic _subscriberTopic;


        #region Registers and Subscribers

        /// <summary>
        /// Power Conversion System Registers.
        /// </summary>
        private readonly DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> _pcsRegisters;


        /// <summary>
        /// Power Conversion System Subscriber.
        /// </summary>
        private DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>? _pcsSubscriber;


        /// <summary>
        /// Battery Bank Registers.
        /// </summary>
        private readonly DeviceRegister<BbDto, BbDefinitionDto, BbCoreDto, BbDetailDto> _bbRegisters;


        /// <summary>
        /// Battery Bank Subscriber.
        /// </summary>
        private DeviceSubscriber<BbDto, BbDefinitionDto, BbCoreDto, BbDetailDto>? _bbSubscriber;


        /// <summary>
        /// Solar Registers.
        /// </summary>
        private readonly DeviceRegister<PvDto, PvDefinitionDto, PvCoreDto, PvDetailDto> _pvRegisters;


        /// <summary>
        /// Solar Subscriber.
        /// </summary>
        private DeviceSubscriber<PvDto, PvDefinitionDto, PvCoreDto, PvDetailDto>? _pvSubscriber;


        /// <summary>
        /// External Power Meter Registers.
        /// </summary>
        private readonly DeviceRegister<PmExternalDto, PmExternalDefinitionDto, PmExternalCoreDto, PmExternalDetailDto> _pmExternalRegisters;


        /// <summary>
        /// External Power Meter Subscriber.
        /// </summary>
        private DeviceSubscriber<PmExternalDto, PmExternalDefinitionDto, PmExternalCoreDto, PmExternalDetailDto>? _pmExternalSubscriber;


        /// <summary>
        /// Auxiliary Power Meter Registers.
        /// </summary>
        private readonly DeviceRegister<PmAuxiliaryDto, PmAuxiliaryDefinitionDto, PmAuxiliaryCoreDto, PmAuxiliaryDetailDto> _pmAuxiliaryRegisters;


        /// <summary>
        /// Auxiliary Power Meter Subscriber.
        /// </summary>
        private DeviceSubscriber<PmAuxiliaryDto, PmAuxiliaryDefinitionDto, PmAuxiliaryCoreDto, PmAuxiliaryDetailDto>? _pmAuxiliarySubscriber;


        /// <summary>
        /// Circuit Power Meter Registers.
        /// </summary>
        private readonly DeviceRegister<PmCircuitDto, PmCircuitDefinitionDto, PmCircuitCoreDto, PmCircuitDetailDto> _pmCircuitRegisters;


        /// <summary>
        /// Circuit Power Meter Subscriber.
        /// </summary>
        private DeviceSubscriber<PmCircuitDto, PmCircuitDefinitionDto, PmCircuitCoreDto, PmCircuitDetailDto>? _pmCircuitSubscriber;


        /// <summary>
        /// System Power Meter Registers.
        /// </summary>
        private readonly DeviceRegister<PmSystemDto, PmSystemDefinitionDto, PmSystemCoreDto, PmSystemDetailDto> _pmSystemRegisters;


        /// <summary>
        /// System Power Meter Subscriber.
        /// </summary>
        private DeviceSubscriber<PmSystemDto, PmSystemDefinitionDto, PmSystemCoreDto, PmSystemDetailDto>? _pmSystemSubscriber;

        #endregion



        public DerDto Der { get; private set; } = new DerDto();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<PcsDto> PowerConversionSystems { get => _pcsRegisters.Devices; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<BbDto> BatteryBanks { get => _bbRegisters.Devices; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<PvDto> SolarPanels { get => _pvRegisters.Devices; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<PmExternalDto> ExternalPowerMeters { get => _pmExternalRegisters.Devices; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<PmAuxiliaryDto> AuxiliaryPowerMeters { get => _pmAuxiliaryRegisters.Devices; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<PmSystemDto> SystemPowerMeters { get => _pmSystemRegisters.Devices; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<PmCircuitDto> CircuitPowerMeters { get => _pmCircuitRegisters.Devices; }


        /// <summary>
        /// Constructor of <see cref="DeviceClient"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="subscriber">Message subscriber interface for API calls to the message subscriber.</param>
        /// <param name="deviceServer">Device servicer interface for API calls to the device service.</param>
        /// <param name="subscriberTopic">Subscriber topic class containing all the subscriber topics used in device client.</param>
        public DeviceClient(ILogger logger, IMessageSubscriber subscriber, IDeviceServer deviceServer, SubscriberTopic subscriberTopic)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(subscriber);
            ArgumentNullException.ThrowIfNull(deviceServer);
            ArgumentNullException.ThrowIfNull(subscriberTopic);

            _logger = logger;
            _subscriber = subscriber;
            _deviceServer = deviceServer;
            _subscriberTopic = subscriberTopic;
            // PCS, BB, PV registers    
            _pcsRegisters = new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(_logger);
            _bbRegisters = new DeviceRegister<BbDto, BbDefinitionDto, BbCoreDto, BbDetailDto>(_logger);
            _pvRegisters = new DeviceRegister<PvDto, PvDefinitionDto, PvCoreDto, PvDetailDto>(_logger);
            // Meter registers
            _pmExternalRegisters = new DeviceRegister<PmExternalDto, PmExternalDefinitionDto, PmExternalCoreDto, PmExternalDetailDto>(_logger);
            _pmAuxiliaryRegisters = new DeviceRegister<PmAuxiliaryDto, PmAuxiliaryDefinitionDto, PmAuxiliaryCoreDto, PmAuxiliaryDetailDto>(_logger);
            _pmCircuitRegisters = new DeviceRegister<PmCircuitDto, PmCircuitDefinitionDto, PmCircuitCoreDto, PmCircuitDetailDto>(_logger);
            _pmSystemRegisters = new DeviceRegister<PmSystemDto, PmSystemDefinitionDto, PmSystemCoreDto, PmSystemDetailDto>(_logger);
        }


        /// <summary>
        /// Initializes the device client.
        /// </summary>
        public async Task Initialize()
        {
            // Handle exception where client is created and initialized.
            Der = await _deviceServer.GetDer();

            IEnumerable<DerCircuitDto> circuits = Der.DerGroups.SelectMany(g => g.DerCircuits);
            IEnumerable<DerUnitDto> units = circuits.SelectMany(c => c.DerUnits);

            // Power meters
            Der.ExternalPowerMeters.ForEach((d) => { _pmExternalRegisters.Add(d); });
            Der.AuxiliaryPowerMeters.ForEach((d) => { _pmAuxiliaryRegisters.Add(d); });
            Der.SystemPowerMeters.ForEach((d) => { _pmSystemRegisters.Add(d); });
            circuits.ToList().ForEach((d) => { if (d.CircuitPowerMeter != null) _pmCircuitRegisters.Add(d.CircuitPowerMeter); });
            // PCS
            units.OfType<DerBatteryStorageUnitDto>().ToList().ForEach(d => { _pcsRegisters.Add(d.PowerConversionSystem); });
            units.OfType<DerSolarUnitDto>().ToList().ForEach(d => { _pcsRegisters.Add(d.PowerConversionSystem); });
            // Battery Banks
            units.OfType<DerBatteryStorageUnitDto>().SelectMany(b => b.BatteryBanks).ToList().ForEach(d => { _bbRegisters.Add(d); });
            // Solars
            units.OfType<DerSolarUnitDto>().SelectMany(s => s.SolarPanels).ToList().ForEach(d => { _pvRegisters.Add(d); });

            Subscribe();
        }


        /// <summary>
        /// Create subscribers for all devices.
        /// </summary>
        private void Subscribe()
        {
            _pcsSubscriber = CreateSubscriber(_logger, _subscriber, _pcsRegisters, _subscriberTopic.PowerConversionSystemTopic);
            _bbSubscriber = CreateSubscriber(_logger, _subscriber, _bbRegisters, _subscriberTopic.BatteryBankTopic);
            _pvSubscriber = CreateSubscriber(_logger, _subscriber, _pvRegisters, _subscriberTopic.SolarPanelTopic);
            _pmExternalSubscriber = CreateSubscriber(_logger, _subscriber, _pmExternalRegisters, _subscriberTopic.ExternalPowerMeterTopic);
            _pmAuxiliarySubscriber = CreateSubscriber(_logger, _subscriber, _pmAuxiliaryRegisters, _subscriberTopic.AuxiliaryPowerMeterTopic);
            _pmCircuitSubscriber = CreateSubscriber(_logger, _subscriber, _pmCircuitRegisters, _subscriberTopic.CircuitPowerMeterTopic);
            _pmSystemSubscriber = CreateSubscriber(_logger, _subscriber, _pmSystemRegisters, _subscriberTopic.SystemPowerMeterTopic);
        }


        /// <summary>
        /// Create device subscriber.
        /// </summary>
        /// <typeparam name="TDevice">The device type.</typeparam>
        /// <typeparam name="TDefinition">The definition type of the device.</typeparam>
        /// <typeparam name="TCore">The core type of the device.</typeparam>
        /// <typeparam name="TDetail">The detail type of the device.</typeparam>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="subscriber">Message subscriber interface to subscribe to messages with callbacks to this instance.</param>
        /// <param name="register">Device register interface holding the registered devices with device Id.</param>        
        /// <param name="entry">Subscriber topic entry.</param>
        /// <returns>Return the device subscriber instance or null if there was nothing configured.</returns>
        private DeviceSubscriber<TDevice, TDefinition, TCore, TDetail>? CreateSubscriber<TDevice, TDefinition, TCore, TDetail>(ILogger logger,
            IMessageSubscriber subscriber, DeviceRegister<TDevice, TDefinition, TCore, TDetail> register, SubscriberTopicEntry? entry)
                where TDevice : DeviceBase<TDefinition, TCore, TDetail>
                where TDefinition : class, IDeviceDefinition
                where TCore : class, IDevice
                where TDetail : class, IDevice
        {
            if (entry == null)
            {
                logger.LogInformation("Device subscriber for device {DeviceTopic} was not registered", typeof(TDevice).Name);
                return null;
            }

            logger.LogInformation("Device subscriber for device {DeviceTopic} was registered", typeof(TDevice).Name);
            return new DeviceSubscriber<TDevice, TDefinition, TCore, TDetail>
                (logger, subscriber, register, entry.DefinitionTopic, entry.CoreTopic, entry.DetailTopic);
        }
    }
}

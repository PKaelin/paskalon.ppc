// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
using paskalON.Telemetry;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Base class for generic Modbus devices.
    /// </summary>
    public abstract class GenericModbusDeviceBase : DerDeviceBase, IGenericModbusDevice, INotifyPropertyChanged
    {
        /// <summary>
        /// Generic Modbus base configuration.
        /// </summary>
        /// <remarks>
        /// Inherits from ModbusConfig.
        /// </remarks>
        private readonly GenericModbusBaseConfig _config;


        /// <summary>
        /// Event when the Generic Modbus Device state <see cref="GenericModbusDeviceState"/> changes.
        /// </summary>
        public event EventHandler<GenericModbusDeviceStateChangedEventArgs>? StateChanged;


        /// <summary>
        /// Event when the communication error state changed.
        /// </summary>
        public event EventHandler<CommunicationErrorChangedEventArgs>? CommunicationErrorChanged;


        /// <summary>
        /// Event when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;


        /// <summary>
        /// State of the Generic Modbus Device.
        /// Specialized Generic Modbus Device has to map its states to the these states.
        /// </summary>
        public GenericModbusDeviceState State
        {
            get;
            set { if (field != value) { field = value; SetState(value); } else field = value; }
        }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError
        {
            get;
            set { if (field != value) { field = value; SetCommunicationError(value); } else field = value; }
        }


        /// <summary>
        /// List of generic Modbus entries that represent the data points and registers of the device.
        /// </summary>
        public required List<GenericModbusEntryBase> GenericModbusEntries { get; init; }


        /// <summary>
        /// Constructor of <see cref="GenericModbusDeviceBase"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The generic Modbus configuration.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        public GenericModbusDeviceBase(ILogger logger, GenericModbusBaseConfig config, List<GenericModbusEntryBase> genericModbusEntries,
            IMetricsPublisher publisher, IDataface dataface) : base(logger, config, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(genericModbusEntries);
            ArgumentNullException.ThrowIfNull(publisher);

            _config = config;
            GenericModbusEntries = genericModbusEntries;

            RegisterMetrics();
            RegisterDataface();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task ConnectAsync()
        {
            _logger.LogInformation("{Name} connect requested.", Name);
            State = GenericModbusDeviceState.Connecting;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task DisconnectAsync()
        {
            _logger.LogInformation("{Name} disconnect requested.", Name);
            State = GenericModbusDeviceState.Disconnecting;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task ResetLatchedAlarmsAsync()
        {
            _logger.LogInformation("{Name} reset latched alarms requested.", Name);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async virtual Task CheckHealthAsync()
        {
            // TODO: Implement state check, data received check and com error update if necessary.
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterMetrics()
        {
            IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>
            {
                { "Name", _config.Name },
                { "DeviceId", _config.DeviceId }
            };

            // Initialize metrics
            MetricsPublisher.Initialize("GMD", tags);

            foreach (GenericModbusPointEntry entry in GenericModbusEntries.OfType<GenericModbusPointEntry>())
            {
                MetricsPublisher.Register<GenericModbusDeviceBase, byte>(this, entry.Name, MetricType.Gauge, x => entry.Value, _config.MetricsFactorClass1);
            }

            foreach (GenericModbusRegisterEntry entry in GenericModbusEntries.OfType<GenericModbusRegisterEntry>())
            {
                MetricsPublisher.Register<GenericModbusDeviceBase, Int16>(this, entry.Name, MetricType.Gauge, x => entry.Value, _config.MetricsFactorClass1);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterDataface()
        {
            // TODO: register registers
        }


        /// <summary>
        /// Trigger GenericModbusDeviceState change events.
        /// </summary>
        /// <param name="state">The GenericModbusDeviceState state.</param>
        private void SetState(GenericModbusDeviceState state)
        {
            _logger.LogInformation("{Name} - GenericModbusDeviceState state changed to: {State}", Name, State);
            StateChanged?.Invoke(this, new GenericModbusDeviceStateChangedEventArgs(state));
        }


        /// <summary>
        /// Trigger CommunicationError change events.
        /// </summary>
        /// <param name="state">The communication error state.</param>
        private void SetCommunicationError(bool state)
        {
            if (state == true)
            {
                _logger.LogError("{Name} - CommunicationError state changed to: {State}", Name, CommunicationError);
            }
            else
            {
                _logger.LogInformation("{Name} - CommunicationError state changed to: {State}", Name, CommunicationError);
            }

            CommunicationErrorChanged?.Invoke(this, new CommunicationErrorChangedEventArgs(state));
        }


        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that changed. An empty value or null indicates that all of the
        /// properties have changed.
        /// </param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

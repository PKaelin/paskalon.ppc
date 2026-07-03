using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
using paskalON.Domains.Contracts;
using paskalON.Domains.Telemetry;
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
    public abstract class GenericModbusDeviceBase : DerDeviceBase<GenericModbusDeviceBase>, IGenericModbusDevice<GenericModbusDeviceBase>, INotifyPropertyChanged
    {
        /// <summary>
        /// Generic Modbus base configuration.
        /// </summary>
        /// <remarks>
        /// Inherits from ModbusConfig.
        /// </remarks>
        private readonly GenericModbusBaseConfig _config;


        /// <summary>
        /// Generic Modbus device instance that communicates with the device.
        /// </summary>
        private readonly IGenericModbusDevice<GenericModbusDeviceBase> _device;


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
        /// <param name="device">The device interface.</param>
        public GenericModbusDeviceBase(ILogger logger, GenericModbusBaseConfig config, List<GenericModbusEntryBase> genericModbusEntries, IMetricsPublisher<GenericModbusDeviceBase> publisher,
            IGenericModbusDevice<GenericModbusDeviceBase> device) : base(logger, config, publisher, device)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(genericModbusEntries);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(device);

            _config = config;
            _device = device;
            GenericModbusEntries = genericModbusEntries;
            RegisterMetrics(publisher);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Connect()
        {
            _logger.LogInformation("{Name} connect requested.", Name);
            _device.Connect();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Disconnect()
        {
            _logger.LogInformation("{Name} disconnect requested.", Name);
            _device.Disconnect();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void ResetLatchedAlarms()
        {
            _logger.LogInformation("{Name} reset latched alarms requested.", Name);
            _device.ResetLatchedAlarms();
        }



        /// <summary>
        /// Trigger GenericModbusDeviceState change events.
        /// </summary>
        /// <param name="state">The GenericModbusDeviceState state.</param>
        protected void SetState(GenericModbusDeviceState state)
        {
            _logger.LogInformation("{Name} - GenericModbusDeviceState state changed to: {State}", Name, State);
            StateChanged?.Invoke(this, new GenericModbusDeviceStateChangedEventArgs(state));
        }


        /// <summary>
        /// Trigger CommunicationError change events.
        /// </summary>
        /// <param name="state">The communication error state.</param>
        protected void SetCommunicationError(bool state)
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


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterMetrics(IMetricsPublisher<GenericModbusDeviceBase> metricsPublisher)
        {
            foreach (GenericModbusPointEntry entry in GenericModbusEntries.OfType<GenericModbusPointEntry>())
            {
                metricsPublisher.Register<byte>(entry.Name, x => entry.Value, _config.MetricsFactorClass1);
            }

            foreach (GenericModbusRegisterEntry entry in GenericModbusEntries.OfType<GenericModbusRegisterEntry>())
            {
                metricsPublisher.Register<Int16>(entry.Name, x => entry.Value, _config.MetricsFactorClass1);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterDataface(IDataface<GenericModbusDeviceBase> dataface)
        {
            // TODO:
        }

    }
}

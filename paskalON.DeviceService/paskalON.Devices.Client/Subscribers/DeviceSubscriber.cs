// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client.Registers;
using paskalON.Devices.Dto;
using paskalON.Messaging;
using System.Text.Json;

namespace paskalON.Devices.Client.Subscribers
{
    /// <summary>
    /// Device subscriber subscribes to a message subscriber and receives messages of the subscribed types.
    /// </summary>
    /// <typeparam name="TDevice">The device type.</typeparam>
    /// <typeparam name="TDefinition">The definition type of the device.</typeparam>
    /// <typeparam name="TCore">The core type of the device.</typeparam>
    /// <typeparam name="TDetail">The detail type of the device.</typeparam>
    public sealed class DeviceSubscriber<TDevice, TDefinition, TCore, TDetail> : IDeviceSubscriber
        where TDevice : DeviceBase<TDefinition, TCore, TDetail>
        where TDefinition : class, IDeviceDefinition
        where TCore : class, IDevice
        where TDetail : class, IDevice
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Device register interface holding the registered devices with device Id.
        /// </summary>
        private readonly IDeviceRegister<TDevice, TDefinition, TCore, TDetail> _register;


        /// <summary>
        /// Message subscriber interface to subscribe to messages with callbacks to this instance.
        /// </summary>
        private readonly IMessageSubscriber _subscriber;


        /// <summary>
        /// The definition message topic name to subscribe to.
        /// </summary>
        private readonly string _definitionTopic;


        /// <summary>
        /// The core message topic name to subscribe to.
        /// </summary>
        private readonly string _coreTopic;


        /// <summary>
        /// The detail message topic name to subscribe to.
        /// </summary>
        private readonly string _detailTopic;


        /// <summary>
        /// Constructor of <see cref="DeviceSubscriber"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="subscriber">Message subscriber interface to subscribe to messages with callbacks to this instance.</param>
        /// <param name="register">Device register interface holding the registered devices with device Id.</param>
        /// <param name="definitionTopic">The definition message topic name to subscribe to.</param>
        /// <param name="coreTopic">The core message topic name to subscribe to.</param>
        /// <param name="detailTopic">The detail message topic name to subscribe to.</param>
        public DeviceSubscriber(ILogger logger, IMessageSubscriber subscriber, IDeviceRegister<TDevice, TDefinition, TCore, TDetail> register,
            string definitionTopic, string coreTopic, string detailTopic)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(subscriber);
            ArgumentNullException.ThrowIfNull(register);

            _logger = logger;
            _register = register;
            _subscriber = subscriber;
            _definitionTopic = definitionTopic;
            _coreTopic = coreTopic;
            _detailTopic = detailTopic;
            Subscribe();
        }


        /// <summary>
        /// Callback for the update definition message.
        /// </summary>
        /// <param name="json">The definition message.</param>
        public void UpdateDefinition(string json)
        {
            TDefinition? message = null;

            try
            {
                message = JsonSerializer.Deserialize<TDefinition>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deserialize device definition with message threw an unexpected error: {Error}", ex.Message);
            }

            if (message is null)
                return;

            try
            {
                _register.UpdateDefinition(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Update device definition threw an unexpected error: {Error}", ex.Message);
            }
        }


        /// <summary>
        /// Callback for the update core message.
        /// </summary>
        /// <param name="json">The core message.</param>
        public void UpdateCore(string json)
        {
            TCore? message = null;

            try
            {
                message = JsonSerializer.Deserialize<TCore>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deserialize device core with message threw an unexpected error: {Error}", ex.Message);
            }

            if (message is null)
                return;

            try
            {
                _register.UpdateCore(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Update device core threw an unexpected error: {Error}", ex.Message);
            }
        }


        /// <summary>
        /// Callback for the update detail message.
        /// </summary>
        /// <param name="json">The detail message.</param>
        public void UpdateDetail(string json)
        {
            TDetail? message = null;

            try
            {
                message = JsonSerializer.Deserialize<TDetail>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError("Deserialize device detail with message threw an unexpected error: {Error}", ex.Message);
            }

            if (message is null)
                return;

            try
            {
                _register.UpdateDetail(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Update device detail threw an unexpected error: {Error}", ex.Message);
            }
        }


        /// <summary>
        /// Subscribe the message callbacks to the message subscriber.
        /// </summary>
        private void Subscribe()
        {
            try
            {
                if (string.IsNullOrEmpty(_definitionTopic) == false)
                {
                    _subscriber.Subscribe(_definitionTopic, UpdateDefinition);
                }

                if (string.IsNullOrEmpty(_coreTopic) == false)
                {
                    _subscriber.Subscribe(_coreTopic, UpdateCore);
                }

                if (string.IsNullOrEmpty(_detailTopic) == false)
                {
                    _subscriber.Subscribe(_detailTopic, UpdateDetail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Subscribing device message to message subscriber threw an unexpected error: {Error}", ex.Message);
            }
        }
    }
}
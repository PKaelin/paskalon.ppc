// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text.Json;
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Devices.Dto;
using paskalON.Messaging;

namespace paskalON.Alarms.Application.Subscribers
{
    /// <summary>
    /// Subscribes to the published data of the other services and queues it for the alarm evaluation.
    /// </summary>
    /// <remarks>
    /// The callbacks only deserialize and queue the data, so that the message subscriber is never blocked by the database.
    /// </remarks>
    public class PpcAlarmSubscriber : IPpcAlarmSubscriber
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<PpcAlarmSubscriber> _logger;


        /// <summary>
        /// Message subscriber of the published data.
        /// </summary>
        private readonly IMessageSubscriber _subscriber;


        /// <summary>
        /// Queue of the received data.
        /// </summary>
        private readonly IPpcAlarmMessageQueue _queue;


        /// <summary>
        /// Time provider of the receive time.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Constructor of <see cref="PpcAlarmSubscriber"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="subscriber">Message subscriber of the published data.</param>
        /// <param name="queue">Queue of the received data.</param>
        /// <param name="timeProvider">Time provider of the receive time.</param>
        public PpcAlarmSubscriber(ILogger<PpcAlarmSubscriber> logger, IMessageSubscriber subscriber, IPpcAlarmMessageQueue queue, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(subscriber);
            ArgumentNullException.ThrowIfNull(queue);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _logger = logger;
            _subscriber = subscriber;
            _queue = queue;
            _timeProvider = timeProvider;
        }


        /// <inheritdoc/>
        public void SubscribeDevice<TDevice>(string topic, PpcAlarmType ppcAlarmType) where TDevice : class, IDevice
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(topic);

            _subscriber.Subscribe(topic, json => EnqueueDevice<TDevice>(json, ppcAlarmType));
            _logger.LogInformation("Subscribed {PpcAlarmType} data {DataType} to topic {Topic}.", ppcAlarmType, typeof(TDevice).Name, topic);
        }


        /// <summary>
        /// Deserializes the published device data and queues it with the device as publisher.
        /// </summary>
        private void EnqueueDevice<TDevice>(string json, PpcAlarmType ppcAlarmType) where TDevice : class, IDevice
        {
            TDevice? device;

            try
            {
                device = JsonSerializer.Deserialize<TDevice>(json);
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Deserializing {DataType} of {PpcAlarmType} failed.", typeof(TDevice).Name, ppcAlarmType);

                return;
            }

            if (device is null)
            {
                return;
            }

            _queue.Enqueue(new PpcAlarmMessage
            {
                Key = new PpcAlarmKey { PpcAlarmType = ppcAlarmType, DeviceId = device.DeviceId },
                Data = device,
                ReceivedOn = _timeProvider.GetUtcNow()
            });
        }
    }
}

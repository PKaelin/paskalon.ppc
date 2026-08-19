// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Client.Registers;
using paskalON.Devices.Dto;
using paskalON.Messaging;
using System.Text.Json;

namespace paskalON.Devices.Client.Subscribers
{
    public sealed class DeviceSubscriber<TDevice, TDefinition, TCore, TDetail> : IDeviceSubscriber
        where TDevice : DeviceBase<TDefinition, TCore, TDetail>
        where TDefinition : class, IDeviceDefinition
        where TCore : class, IDevice
        where TDetail : class, IDevice
    {
        private readonly DeviceRegister<TDevice, TDefinition, TCore, TDetail> _register;
        private readonly IMessageSubscriber _subscriber;
        private readonly string _definitionTopic;
        private readonly string _coreTopic;
        private readonly string _detailTopic;


        public DeviceSubscriber(IMessageSubscriber subscriber, DeviceRegister<TDevice, TDefinition, TCore, TDetail> register,
            string definitionTopic, string coreTopic, string detailTopic)
        {
            ArgumentNullException.ThrowIfNull(subscriber);
            ArgumentNullException.ThrowIfNull(register);

            _register = register;
            _subscriber = subscriber;
            _definitionTopic = definitionTopic;
            _coreTopic = coreTopic;
            _detailTopic = detailTopic;
            Subscribe();
        }

        public void UpdateDefinition(string json)
        {
            var message = JsonSerializer.Deserialize<TDefinition>(json);

            if (message is null)
                return;

            _register.UpdateDefinition(message);
        }


        public void UpdateCore(string json)
        {
            var message = JsonSerializer.Deserialize<TCore>(json);

            if (message is null)
                return;

            _register.UpdateCore(message);
        }

        public void UpdateDetail(string json)
        {
            var message = JsonSerializer.Deserialize<TDetail>(json);

            if (message is null)
                return;

            _register.UpdateDetail(message);
        }

        private void Subscribe()
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
    }
}
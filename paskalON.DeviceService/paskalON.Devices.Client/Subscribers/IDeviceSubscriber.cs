// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Client.Subscribers
{
    public interface IDeviceSubscriber
    {
        void UpdateDefinition(string json);

        void UpdateCore(string json);

        void UpdateDetail(string json);
    }
}

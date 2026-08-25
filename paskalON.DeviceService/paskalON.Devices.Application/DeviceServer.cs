// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;

namespace paskalON.Devices.Application
{
    public class DeviceServer : IDeviceServer
    {
        public Task<DerDto> GetDer()
        {
            throw new NotImplementedException();
        }
    }
}

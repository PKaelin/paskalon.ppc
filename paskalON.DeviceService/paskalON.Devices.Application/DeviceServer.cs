// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;

namespace paskalON.Devices.Application
{
    public class DeviceServer : IDeviceServer
    {
        private readonly IDeviceManager _deviceManager;

        private readonly DeviceMapper _mapper;


        public DeviceServer(IDeviceManager deviceManager, DeviceMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(deviceManager);
            ArgumentNullException.ThrowIfNull(mapper);

            _deviceManager = deviceManager;
            _mapper = mapper;
        }


        public Task<DerDto> GetDer()
        {
            return Task.FromResult(_mapper.MapDer(_deviceManager.Der));
        }
    }
}

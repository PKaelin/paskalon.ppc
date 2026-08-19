// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client.Registers;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Devices.Client
{
    public class DeviceClient : IDeviceClient
    {
        private ILogger _logger;

        public DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> PcsRegisters { get; }
        public DeviceRegister<BbDto, BbDefinitionDto, BbCoreDto, BbDetailDto> BbRegisters { get; }
        public ICollection<PcsDto> Pcs { get => PcsRegisters.Devices; }
        public ICollection<BbDto> Bbs { get => BbRegisters.Devices; }


        public DeviceClient(ILogger logger, IEnumerable<PcsDefinitionDto> pcsDefinitions, IEnumerable<BbDefinitionDto> bbDefinitions)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;

            PcsRegisters = new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(_logger);

            foreach (var definition in pcsDefinitions)
            {
                PcsRegisters.Add(new PcsDto(definition));
            }

            BbRegisters = new DeviceRegister<BbDto, BbDefinitionDto, BbCoreDto, BbDetailDto>(_logger);

            foreach (var definition in bbDefinitions)
            {
                BbRegisters.Add(new BbDto(definition));
            }
        }
    }
}

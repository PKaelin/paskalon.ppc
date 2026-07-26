// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;

namespace paskalON.OperatingModes.Domain.IntegrationTest.OpenModes.FrequencyActives
{
    [TestClass]
    public class ActivePowerFixedModeTest
    {
        [TestMethod]
        public void PowerFixedModeTest()
        {
            Mock<ActivePowerFixedModeMap> map = new Mock<ActivePowerFixedModeMap>();
            Mock<SystemConfig> systemConfig = new Mock<SystemConfig>();

            RampRateConfig rampConfig = new RampRateConfig
            {
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpRatePerSecond = 10,
                RampDownRatePerSecond = 20
            };


            ActivePowerFixedModeConfig config = new ActivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerFixedModeConfig",
                Type = OperatingModeType.Bess,
                IsActive = true,
                TimeoutSeconds = 0,
                RampConfig = rampConfig
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            RampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, rampConfig);
            ActivePowerFixedMode mode = new ActivePowerFixedMode(NullLogger.Instance, timeProvider, systemConfig.Object, config, map.Object, ramp, null);

        }
    }
}

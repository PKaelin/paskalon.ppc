// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs.Ramps;

namespace paskalON.OperatingModes.Domain.Configs.UnitTest.Ramps
{
    [TestClass]
    public class RampRatePercentageConfigTest
    {
        [TestMethod]
        public void RampRatePercentageConfigConstructorTest()
        {
            RampRatePercentageConfig config = new RampRatePercentageConfig
            {
                Id = 1,
                RampTimeSeconds = 0,
                RampUpRatePercentPerSecond = 0,
                RampDownRatePercentPerSecond = 0,
                ChangedBy = "Test",
                ChangedDate = DateTimeOffset.UtcNow
            };

            Assert.IsNotNull(config);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampRatePercentageConfig { Id = 1, RampTimeSeconds = -1, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampRatePercentageConfig { Id = 1, RampUpRatePercentPerSecond = -1, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampRatePercentageConfig { Id = 1, RampDownRatePercentPerSecond = -1, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampRatePercentageConfig { Id = 1, RampUpRatePercentPerSecond = 101, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampRatePercentageConfig { Id = 1, RampDownRatePercentPerSecond = 101, ChangedBy = "Test" });
        }
    }
}
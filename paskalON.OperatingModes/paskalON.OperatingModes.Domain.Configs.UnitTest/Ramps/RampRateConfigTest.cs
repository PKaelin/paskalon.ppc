// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs.Ramps;

namespace paskalON.OperatingModes.Domain.Configs.UnitTest.Ramps
{
    [TestClass]
    public class RampRateConfigTest
    {
        [TestMethod]
        public void RampRateConfigConstructorTest()
        {
            RampRateConfig config = new RampRateConfig
            {
                Id = 1,
                RampTimeSeconds = 0,
                RampUpRatePerSecond = 0,
                RampDownRatePerSecond = 0,
                ChangedBy = "Test",
                ChangedDate = DateTimeOffset.UtcNow
            };

            Assert.IsNotNull(config);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampRateConfig { Id = 1, RampTimeSeconds = -1, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampRateConfig { Id = 1, RampUpRatePerSecond = -1, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampRateConfig { Id = 1, RampUpRatePerSecond = -1, ChangedBy = "Test" });
        }
    }
}
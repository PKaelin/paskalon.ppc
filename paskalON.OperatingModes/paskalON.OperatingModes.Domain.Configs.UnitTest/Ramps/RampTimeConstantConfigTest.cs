// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs.Ramps;

namespace paskalON.OperatingModes.Domain.Configs.UnitTest.Ramps
{
    [TestClass]
    public class RampTimeConstantConfigTest
    {
        [TestMethod]
        public void RampTimeConstantConfigConstructorTest()
        {
            RampTimeConstantConfig config = new RampTimeConstantConfig
            {
                Id = 1,
                RampTimeSeconds = 0,
                RampUpTimeConstantSeconds = 0,
                RampDownTimeConstantSeconds = 0,
                ChangedBy = "Test",
                ChangedDate = DateTimeOffset.UtcNow
            };

            Assert.IsNotNull(config);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampTimeConstantConfig { Id = 1, RampTimeSeconds = -1, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampTimeConstantConfig { Id = 1, RampUpTimeConstantSeconds = -1, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampTimeConstantConfig { Id = 1, RampDownTimeConstantSeconds = -1, ChangedBy = "Test" });
        }
    }
}

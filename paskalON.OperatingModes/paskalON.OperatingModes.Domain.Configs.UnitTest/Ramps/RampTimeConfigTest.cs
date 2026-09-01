// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs.Ramps;

namespace paskalON.OperatingModes.Domain.Configs.UnitTest.Ramps
{
    [TestClass]
    public class RampTimeConfigTest
    {
        [TestMethod]
        public void RampTimeConfigConstructorTest()
        {
            RampTimeConfig config = new RampTimeConfig
            {
                Id = 1,
                RampTimeSeconds = 0,
                RampUpTimeSeconds = 0,
                RampDownTimeSeconds = 0,
                ChangedBy = "Test",
                ChangedDate = DateTimeOffset.UtcNow
            };

            Assert.IsNotNull(config);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampTimeConfig { Id = 1, RampTimeSeconds = -1, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampTimeConfig { Id = 1, RampUpTimeSeconds = -1, ChangedBy = "Test" });
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RampTimeConfig { Id = 1, RampDownTimeSeconds = -1, ChangedBy = "Test" });
        }
    }
}
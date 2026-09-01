// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.UnitTest.Ders
{
    [TestClass]
    public class DerGroupTest
    {
        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Der der = new Der(NullLogger<Der>.Instance, new DerConfig { ChangedBy = "Test", Name = "DerConfig" });
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerGroup(NullLogger<DerGroup>.Instance, null!, der));
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = derConfig };
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerGroup(NullLogger<DerGroup>.Instance, groupConfig, null!));
        }


        [TestMethod]
        public void CreateDerGroupTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            Der der = new Der(NullLogger<Der>.Instance, derConfig);
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = derConfig };

            DerGroup derGroup = new DerGroup(NullLogger<DerGroup>.Instance, groupConfig, der);

            Assert.IsNotNull(derGroup.DerCircuits);
            Assert.IsNotNull(derGroup.Der);
        }

    }
}

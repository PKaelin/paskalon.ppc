// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.UnitTest.Ders
{
    [TestClass]
    public class DerCircuitTest
    {
        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            Der der = new Der(NullLogger<Der>.Instance, derConfig);
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = derConfig };
            DerGroup group = new DerGroup(NullLogger<DerGroup>.Instance, groupConfig, der);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerCircuit(NullLogger<DerGroup>.Instance, null, group));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = derConfig };
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerGroup(NullLogger<DerGroup>.Instance, groupConfig, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.UnitTest.Ders
{
    [TestClass]
    public class DerBatteryStorageUnitTest
    {
        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            Der der = new Der(NullLogger.Instance, derConfig);
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = derConfig };
            DerGroup group = new DerGroup(NullLogger.Instance, groupConfig, der);
            DerCircuitConfig circuitConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "DerCircuitConfig", DerGroupConfig = groupConfig };
            DerCircuit circuit = new DerCircuit(NullLogger.Instance, circuitConfig, group);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerBatteryStorageUnit(NullLogger.Instance, null, circuit));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = derConfig };
            DerCircuitConfig circuitConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "DerCircuitConfig", DerGroupConfig = groupConfig };
            DerBatteryStorageUnitConfig unitConfig = new DerBatteryStorageUnitConfig { ChangedBy = "Test", Name = "DerUnit", DerCircuitConfig = circuitConfig };

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerBatteryStorageUnit(NullLogger.Instance, unitConfig, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}

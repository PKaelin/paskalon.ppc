// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.UnitTest.Ders
{
    [TestClass]
    public class DerBatteryStorageUnitTest
    {
        private DerCircuit? _circuit;
        private DerBatteryStorageUnitConfig? _unitConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            // Der
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");
            Mock<Der> der = new Mock<Der>(NullLogger.Instance, derConfig.Object);
            // Group
            Mock<DerGroupConfig> groupConfig = new Mock<DerGroupConfig>();
            groupConfig.SetupGet(x => x.Name).Returns("DerGroupConfig");
            Mock<DerGroup> group = new Mock<DerGroup>(NullLogger.Instance, groupConfig.Object, der.Object);
            // Circuit
            Mock<DerCircuitConfig> circuitConfig = new Mock<DerCircuitConfig>();
            circuitConfig.SetupGet(x => x.Name).Returns("DerCircuitConfig");
            _circuit = new DerCircuit(NullLogger.Instance, circuitConfig.Object, group.Object);
            // Unit
            _unitConfig = new DerBatteryStorageUnitConfig { ChangedBy = "Test", Name = "DerUnit", DerCircuitConfig = circuitConfig!.Object };
        }



        [TestMethod]
        public void CreateWithoutConfigTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerBatteryStorageUnit(NullLogger.Instance, null, _circuit!));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerBatteryStorageUnit(NullLogger.Instance, _unitConfig!, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [TestMethod]
        public void CreateDerBatteryStorageUnit()
        {
            DerBatteryStorageUnit unit = new DerBatteryStorageUnit(NullLogger.Instance, _unitConfig!, _circuit!);

            Assert.IsNotNull(unit.DerCircuit);
            Assert.IsNotNull(unit.BatteryBanks);
            Assert.HasCount(0, unit.BatteryBanks);
            Assert.IsNull(unit.PowerConversionSystem);
        }
    }
}

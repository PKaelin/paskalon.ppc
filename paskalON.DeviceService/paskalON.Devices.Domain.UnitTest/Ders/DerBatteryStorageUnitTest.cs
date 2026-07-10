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
        private DerConfig? _derConfig;
        private Der? _der;
        private DerGroupConfig? _groupConfig;
        private DerGroup? _group;
        private DerCircuitConfig? _circuitConfig;
        private DerCircuit? _circuit;
        private DerBatteryStorageUnitConfig? _unitConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            _derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            _der = new Der(NullLogger.Instance, _derConfig);
            _groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = _derConfig };
            _group = new DerGroup(NullLogger.Instance, _groupConfig, _der);
            _circuitConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "DerCircuitConfig", DerGroupConfig = _groupConfig };
            _circuit = new DerCircuit(NullLogger.Instance, _circuitConfig, _group);
            _unitConfig = new DerBatteryStorageUnitConfig { ChangedBy = "Test", Name = "DerUnit", DerCircuitConfig = _circuitConfig! };
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

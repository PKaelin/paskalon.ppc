// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.UnitTest.Ders
{
    [TestClass]
    public class DerSolarUnitTest
    {
        private DerCircuit? _circuit;
        private DerSolarUnitConfig? _unitConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            Der der = new Der(NullLogger.Instance, derConfig);
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = derConfig };
            DerGroup group = new DerGroup(NullLogger.Instance, groupConfig, der);
            DerCircuitConfig circuitConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "DerCircuitConfig", DerGroupConfig = groupConfig };
            _circuit = new DerCircuit(NullLogger.Instance, circuitConfig, group);
            _unitConfig = new DerSolarUnitConfig { ChangedBy = "Test", Name = "DerUnit", DerCircuitConfig = circuitConfig! };
        }


        [TestMethod]
        public void CreateWithoutConfigTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerSolarUnit(NullLogger.Instance, null, _circuit!));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerSolarUnit(NullLogger.Instance, _unitConfig!, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateDerSolaUnitTest()
        {
            DerSolarUnit unit = new DerSolarUnit(NullLogger.Instance, _unitConfig!, _circuit!);

            Assert.IsNotNull(unit.DerCircuit);
            Assert.IsNull(unit.PowerConversionSystem);
        }
    }
}

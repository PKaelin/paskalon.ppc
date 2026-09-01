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
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerSolarUnit(NullLogger.Instance, null!, _circuit!));
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerSolarUnit(NullLogger.Instance, _unitConfig!, null!));
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

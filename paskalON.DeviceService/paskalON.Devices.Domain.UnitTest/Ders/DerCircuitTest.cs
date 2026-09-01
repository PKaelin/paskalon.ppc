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
    public class DerCircuitTest
    {
        private DerConfig? _derConfig;
        private DerGroupConfig? _groupConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            _derConfig = new DerConfig { ChangedBy = "Test", Name = "DerConfig" };
            _groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "DerGroupConfig", DerConfig = _derConfig };
        }



        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Der der = new Der(NullLogger<Der>.Instance, _derConfig!);
            DerGroup group = new DerGroup(NullLogger<DerGroup>.Instance, _groupConfig!, der);
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerCircuit(NullLogger<DerGroup>.Instance, null!, group));
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerGroup(NullLogger<DerGroup>.Instance, _groupConfig!, null!));
        }


        [TestMethod]
        public void CreateDerCircuitTest()
        {
            Der der = new Der(NullLogger<Der>.Instance, _derConfig!);
            DerGroup group = new DerGroup(NullLogger<DerGroup>.Instance, _groupConfig!, der);
            DerCircuitConfig circuitConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "DerCircuitConfig", DerGroupConfig = _groupConfig! };

            DerCircuit derCircuit = new DerCircuit(NullLogger<DerGroup>.Instance, circuitConfig, group);

            Assert.IsNotNull(derCircuit.DerUnits);
            Assert.IsNotNull(derCircuit.DerGroup);
            Assert.IsNull(derCircuit.CircuitBreaker);
            Assert.IsNull(derCircuit.CircuitPowerMeter);
        }
    }
}

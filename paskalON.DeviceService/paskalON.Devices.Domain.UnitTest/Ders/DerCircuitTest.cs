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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerCircuit(NullLogger<DerGroup>.Instance, null, group));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithoutParentTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerGroup(NullLogger<DerGroup>.Instance, _groupConfig!, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
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

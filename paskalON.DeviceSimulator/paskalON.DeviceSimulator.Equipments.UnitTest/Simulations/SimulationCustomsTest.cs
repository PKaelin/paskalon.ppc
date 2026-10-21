// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Equipments.UnitTest.Simulations
{
    [TestClass]
    public sealed class SimulationCustomsTest
    {
        [TestMethod]
        [DataRow("5", 5d)]
        [DataRow("2.5", 2.5d)]
        [DataRow("34500", 34500d)]
        public void GetDoubleReturnsConfiguredValueTest(string value, double expected)
        {
            Dictionary<string, string> customs = new Dictionary<string, string> { { "SimPreChargeDelaySeconds", value } };

            double result = SimulationCustoms.GetDouble(customs, "SimPreChargeDelaySeconds", 0);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void GetDoubleReturnsDefaultWhenMissingTest()
        {
            Dictionary<string, string> customs = new Dictionary<string, string> { { "BbSimulationKey", "Value" } };

            double result = SimulationCustoms.GetDouble(customs, "SimPreChargeDelaySeconds", 7);

            Assert.AreEqual(7, result);
        }


        [TestMethod]
        public void GetDoubleInvalidNumberThrowsTest()
        {
            Dictionary<string, string> customs = new Dictionary<string, string> { { "SimPreChargeDelaySeconds", "five" } };

            Assert.ThrowsExactly<FormatException>(() => SimulationCustoms.GetDouble(customs, "SimPreChargeDelaySeconds", 0));
        }
    }
}

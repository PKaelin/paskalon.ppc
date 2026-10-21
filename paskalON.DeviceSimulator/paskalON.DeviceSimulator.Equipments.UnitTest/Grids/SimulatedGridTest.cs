// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Equipments.Grids;

namespace paskalON.DeviceSimulator.Equipments.UnitTest.Grids
{
    [TestClass]
    public sealed class SimulatedGridTest
    {
        [TestMethod]
        public void SimulatedGridStartsWithDefaultsTest()
        {
            SimulatedGrid grid = new SimulatedGrid();

            Assert.AreEqual(SimulatedGrid.DefaultNominalFrequency, grid.NominalFrequency);
            Assert.AreEqual(SimulatedGrid.DefaultNominalVoltageLineToLine, grid.NominalVoltageLineToLine);
            Assert.AreEqual(SimulatedGrid.DefaultNominalFrequency, grid.Frequency);
        }


        [TestMethod]
        [DataRow(50d, 20000d)]
        [DataRow(60d, 34500d)]
        public void ConfigureSetsNominalValuesTest(double frequency, double voltage)
        {
            SimulatedGrid grid = new SimulatedGrid();

            grid.Configure(frequency, voltage);

            Assert.AreEqual(frequency, grid.NominalFrequency);
            Assert.AreEqual(voltage, grid.NominalVoltageLineToLine);
            Assert.AreEqual(frequency, grid.Frequency);
        }


        [TestMethod]
        [DataRow(0d, 34500d)]
        [DataRow(-50d, 34500d)]
        [DataRow(60d, 0d)]
        public void ConfigureInvalidValuesThrowsTest(double frequency, double voltage)
        {
            SimulatedGrid grid = new SimulatedGrid();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => grid.Configure(frequency, voltage));
        }


        [TestMethod]
        public void TickKeepsFrequencyCloseToNominalTest()
        {
            SimulatedGrid grid = new SimulatedGrid();
            grid.Configure(50, 20000);
            List<double> frequencies = new List<double>();

            for (int tick = 0; tick < 200; tick++)
            {
                grid.Tick();
                frequencies.Add(grid.Frequency);
            }

            Assert.IsTrue(frequencies.All(frequency => frequency >= 49.97 && frequency <= 50.03));
        }
    }
}

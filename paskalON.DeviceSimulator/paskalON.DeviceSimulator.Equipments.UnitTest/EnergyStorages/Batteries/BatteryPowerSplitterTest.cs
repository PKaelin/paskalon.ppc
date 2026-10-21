// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;

namespace paskalON.DeviceSimulator.Equipments.UnitTest.EnergyStorages.Batteries
{
    [TestClass]
    public sealed class BatteryPowerSplitterTest
    {
        [TestMethod]
        [DataRow(1500000d, 4000000d, 2500000d, 2000000d, 2500000d, 1000000d, 500000d, DisplayName = "Discharge proportional to usable capacity")]
        [DataRow(-1500000d, 4000000d, 2500000d, 2000000d, 2500000d, -1000000d, -500000d, DisplayName = "Charge proportional to usable capacity")]
        [DataRow(1500000d, 4000000d, 600000d, 2000000d, 2500000d, 600000d, 900000d, DisplayName = "Limited bank shares the remaining power")]
        [DataRow(1500000d, 0d, 2500000d, 2000000d, 2500000d, 0d, 1500000d, DisplayName = "Bank without capacity takes no power")]
        [DataRow(1500000d, 4000000d, 0d, 2000000d, 2500000d, 0d, 1500000d, DisplayName = "Bank without power limit takes no power")]
        [DataRow(9000000d, 4000000d, 1000000d, 2000000d, 1000000d, 1000000d, 1000000d, DisplayName = "Power above all limits allocates the limits")]
        [DataRow(0d, 4000000d, 2500000d, 2000000d, 2500000d, 0d, 0d, DisplayName = "No power allocates nothing")]
        public void SplitAllocatesPowerTest(double activePower, double weight1, double limit1, double weight2, double limit2, double expected1, double expected2)
        {
            List<(double Weight, double Limit)> candidates = new List<(double Weight, double Limit)>
            {
                (weight1, limit1),
                (weight2, limit2)
            };

            double[] allocations = BatteryPowerSplitter.Split(activePower, candidates);

            Assert.HasCount(2, allocations);
            Assert.AreEqual(expected1, allocations[0], 0.001);
            Assert.AreEqual(expected2, allocations[1], 0.001);
        }


        [TestMethod]
        public void SplitWithoutCandidatesReturnsEmptyTest()
        {
            List<(double Weight, double Limit)> candidates = new List<(double Weight, double Limit)>();

            double[] allocations = BatteryPowerSplitter.Split(1500000, candidates);

            Assert.IsEmpty(allocations);
        }


        [TestMethod]
        public void SplitNullCandidatesThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => BatteryPowerSplitter.Split(1500000, null!));
        }
    }
}

using paskalON.PhysicalUnits.Electricals.Energies;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Energies
{
    [TestClass]
    public class EnergyTest
    {
        [TestMethod]
        public void EnergyConstructorTest()
        {
            Energy energy = new Energy(TimeSpan.FromHours(1).TotalHours * 50);
            Assert.AreEqual(50, energy.WattHours);

            energy = new Energy(TimeSpan.FromMinutes(30).TotalHours * 50);
            Assert.AreEqual(25, energy.WattHours);
        }


        [TestMethod]
        public void EnergyStaticConstructorWattHoursTest()
        {
            Energy energy = Energy.FromWattHours(TimeSpan.FromHours(1).TotalHours * 50);
            Assert.AreEqual(50, energy.WattHours);

            energy = Energy.FromWattHours(TimeSpan.FromMinutes(30).TotalHours * 50);
            Assert.AreEqual(25, energy.WattHours);
        }


        [TestMethod]
        public void EnergyStaticConstructorKiloWattHoursTest()
        {
            Energy energy = Energy.FromKiloWattHours(TimeSpan.FromHours(1).TotalHours * 50);
            Assert.AreEqual(50, energy.KiloWattHours);

            energy = Energy.FromKiloWattHours(TimeSpan.FromMinutes(30).TotalHours * 50);
            Assert.AreEqual(25, energy.KiloWattHours);
        }


        [TestMethod]
        public void EnergyStaticConstructorMegaWattHoursTest()
        {
            Energy energy = Energy.FromMegaWattHours(TimeSpan.FromHours(1).TotalHours * 50);
            Assert.AreEqual(50, energy.MegaWattHours);

            energy = Energy.FromMegaWattHours(TimeSpan.FromMinutes(30).TotalHours * 50);
            Assert.AreEqual(25, energy.MegaWattHours);
        }


        [TestMethod]
        public void EnergyWattHoursKiloWattHoursMegaWattHours()
        {
            Energy energy = new Energy(TimeSpan.FromHours(1).TotalHours * 111222333);
            Assert.AreEqual(111222333, energy.WattHours);
            Assert.AreEqual(111222.333, energy.KiloWattHours);
            Assert.AreEqual(111.222333, energy.MegaWattHours);
            Assert.AreEqual(111222333d * 3600d, energy.WattSeconds);
        }

    }
}

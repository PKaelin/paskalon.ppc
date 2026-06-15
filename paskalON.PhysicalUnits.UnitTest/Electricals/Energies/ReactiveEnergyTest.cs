using paskalON.PhysicalUnits.Electricals.Energies;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Energies
{
    [TestClass]
    public class ReactiveEnergyTest
    {
        [TestMethod]
        public void ReactiveEnergyConstructorTest()
        {
            ReactiveEnergy energy = new ReactiveEnergy(TimeSpan.FromHours(1).TotalHours * 50);
            Assert.AreEqual(50, energy.VoltAmperesReactiveHours);

            energy = new ReactiveEnergy(TimeSpan.FromMinutes(30).TotalHours * 50);
            Assert.AreEqual(25, energy.VoltAmperesReactiveHours);
        }


        [TestMethod]
        public void ReactiveEnergyStaticConstructorVoltAmperesReactiveHoursTest()
        {
            ReactiveEnergy energy = ReactiveEnergy.FromVoltAmperesReactiveHours(TimeSpan.FromHours(1).TotalHours * 50);
            Assert.AreEqual(50, energy.VoltAmperesReactiveHours);

            energy = ReactiveEnergy.FromVoltAmperesReactiveHours(TimeSpan.FromMinutes(30).TotalHours * 50);
            Assert.AreEqual(25, energy.VoltAmperesReactiveHours);
        }


        [TestMethod]
        public void ReactiveEnergyStaticConstructorKiloVoltAmperesReactiveHoursTest()
        {
            ReactiveEnergy energy = ReactiveEnergy.FromKiloVoltAmperesReactiveHours(TimeSpan.FromHours(1).TotalHours * 50);
            Assert.AreEqual(50, energy.KiloVoltAmperesReactiveHours);

            energy = ReactiveEnergy.FromKiloVoltAmperesReactiveHours(TimeSpan.FromMinutes(30).TotalHours * 50);
            Assert.AreEqual(25, energy.KiloVoltAmperesReactiveHours);
        }


        [TestMethod]
        public void ReactiveEnergyStaticConstructorMegaVoltAmperesReactiveHoursTest()
        {
            ReactiveEnergy energy = ReactiveEnergy.FromMegaVoltAmperesReactiveHours(TimeSpan.FromHours(1).TotalHours * 50);
            Assert.AreEqual(50, energy.MegaVoltAmperesReactiveHours);

            energy = ReactiveEnergy.FromMegaVoltAmperesReactiveHours(TimeSpan.FromMinutes(30).TotalHours * 50);
            Assert.AreEqual(25, energy.MegaVoltAmperesReactiveHours);
        }


        [TestMethod]
        public void ReactiveEnergyVoltAmperesReactiveHoursKiloVoltAmperesReactiveHoursMegaVoltAmperesReactiveHours()
        {
            ReactiveEnergy energy = new ReactiveEnergy(TimeSpan.FromHours(1).TotalHours * 111222333);
            Assert.AreEqual(111222333, energy.VoltAmperesReactiveHours);
            Assert.AreEqual(111222.333, energy.KiloVoltAmperesReactiveHours);
            Assert.AreEqual(111.222333, energy.MegaVoltAmperesReactiveHours);
            Assert.AreEqual(111222333d * 3600d, energy.VoltAmperesReactiveSeconds);
        }

    }
}

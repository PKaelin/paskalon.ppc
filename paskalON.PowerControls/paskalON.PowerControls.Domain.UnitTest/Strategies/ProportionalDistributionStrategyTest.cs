// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Domain.Strategies;
using paskalON.Telemetry;
using System.Text.RegularExpressions;

namespace paskalON.PowerControls.Domain.UnitTest.Strategies
{
    [TestClass]
    public class ProportionalDistributionStrategyTest
    {
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private FakeLogger _logger = new FakeLogger();
        private ProportionalDistributionStrategy _distribution = null!;


        [TestInitialize]
        public void Initialize()
        {
            _distribution = new ProportionalDistributionStrategy(_logger);
        }


        [TestMethod]
        public void ProportionalDistributionEmptyUnitsTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            _distribution!.Distribute(active, reactive, new List<DerUnitPowerControl>());

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 20.*achieved: 0.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 10.*achieved: 0.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
        }


        [TestMethod]
        public void DistributeZeroActivePowerSetsAllUnitsToZeroTest()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: -1000, maximumActive: 5000, minimumReactive: -2000, maximumReactive: 4000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: -2000, maximumActive: 10000, minimumReactive: -1000, maximumReactive: 6000);

            _distribution.Distribute(new ActivePower(0), new ReactivePower(0), new[] { unit1.Object, unit2.Object });

            VerifyUpdatePower(unit1, 0, 0);
            VerifyUpdatePower(unit2, 0, 0);
        }


        [TestMethod]
        public void DistributePositiveActivePowerDistributesProportionallyToMaximumTest()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: -1000, maximumActive: 5000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: -1000, maximumActive: 10000);

            _distribution.Distribute(new ActivePower(9000), new ReactivePower(0), new[] { unit1.Object, unit2.Object });

            VerifyUpdatePower(unit1, 3000, 0);
            VerifyUpdatePower(unit2, 6000, 0);
        }


        [TestMethod]
        public void DistributePositiveActivePowerDistributesAcrossThreeUnitsTest()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: -1000, maximumActive: 2000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: -1000, maximumActive: 3000);
            Mock<IDerUnitPowerControl> unit3 = CreateUnit(minimumActive: -1000, maximumActive: 5000);

            _distribution.Distribute(new ActivePower(5000), new ReactivePower(0), new[] { unit1.Object, unit2.Object, unit3.Object });

            VerifyUpdatePower(unit1, 1000, 0);
            VerifyUpdatePower(unit2, 1500, 0);
            VerifyUpdatePower(unit3, 2500, 0);
        }


        [TestMethod]
        public void DistributeNegativeActivePowerTestDistributesProportionallyToMinimum()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: -2000, maximumActive: 5000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: -4000, maximumActive: 10000);

            _distribution.Distribute(new ActivePower(-3000), new ReactivePower(0), new[] { unit1.Object, unit2.Object });

            // Total minimum = -6,000
            // Unit 1 = (-2,000 / -6,000) * -3,000 = -1,000
            // Unit 2 = (-4,000 / -6,000) * -3,000 = -2,000
            VerifyUpdatePower(unit1, -1000, 0);
            VerifyUpdatePower(unit2, -2000, 0);
        }


        [TestMethod]
        public void DistributePositiveReactivePowerDistributesProportionallyToMaximumTest()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumReactive: -1000, maximumReactive: 2000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumReactive: -1000, maximumReactive: 4000);

            _distribution.Distribute(new ActivePower(0), new ReactivePower(3000), new[] { unit1.Object, unit2.Object });

            VerifyUpdatePower(unit1, 0, 1000);
            VerifyUpdatePower(unit2, 0, 2000);
        }


        [TestMethod]
        public void DistributeNegativeReactivePowerTestDistributesProportionallyToMinimum()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumReactive: -2000, maximumReactive: 2000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumReactive: -4000, maximumReactive: 4000);

            _distribution.Distribute(new ActivePower(0), new ReactivePower(-3000), new[] { unit1.Object, unit2.Object });

            VerifyUpdatePower(unit1, 0, -1000);
            VerifyUpdatePower(unit2, 0, -2000);
        }


        [TestMethod]
        public void DistributeOnlyEnabledUnitsAreUpdatedTest()
        {
            Mock<IDerUnitPowerControl> enabledUnit = CreateUnit(enabled: true, minimumActive: -1000, maximumActive: 5000);
            Mock<IDerUnitPowerControl> disabledUnit = CreateUnit(enabled: false, minimumActive: -10000, maximumActive: 10000);

            _distribution.Distribute(new ActivePower(2500), new ReactivePower(0), new[] { enabledUnit.Object, disabledUnit.Object });

            VerifyUpdatePower(enabledUnit, 2500, 0);
            disabledUnit.Verify(x => x.UpdatePower(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
        }


        [TestMethod]
        public void DistributeAllUnitsDisabledDoesNotUpdateAnyUnitTest()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(enabled: false);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(enabled: false);

            _distribution.Distribute(new ActivePower(1000), new ReactivePower(1000), new[] { unit1.Object, unit2.Object });

            unit1.Verify(x => x.UpdatePower(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
            unit2.Verify(x => x.UpdatePower(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
        }


        [TestMethod]
        public void DistributePositiveActivePowerWhenTotalMaximumIsZeroReturnsZero()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: 0, maximumActive: 0);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: 0, maximumActive: 0);

            _distribution.Distribute(new ActivePower(5000), new ReactivePower(0), new[] { unit1.Object, unit2.Object });

            VerifyUpdatePower(unit1, 0, 0);
            VerifyUpdatePower(unit2, 0, 0);
        }


        [TestMethod]
        public void DistributeNegativeActivePowerWhenTotalMinimumIsZeroReturnsZero()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: 0, maximumActive: 5000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: 0, maximumActive: 10000);

            _distribution.Distribute(new ActivePower(-5000), new ReactivePower(0), new[] { unit1.Object, unit2.Object });

            VerifyUpdatePower(unit1, 0, 0);
            VerifyUpdatePower(unit2, 0, 0);
        }

        [TestMethod]
        public void DistributePositiveActivePowerWhenTotalMaximumIsDoubleMaxReturnsZero()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: 0, maximumActive: double.MaxValue);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: 0, maximumActive: double.MaxValue);

            _distribution.Distribute(new ActivePower(5000), new ReactivePower(0), new[] { unit1.Object, unit2.Object });

            VerifyUpdatePower(unit1, 2500, 0);
            VerifyUpdatePower(unit2, 2500, 0);
        }


        [TestMethod]
        public void DistributeMixedActiveLimitsDistributesPositiveTargetUsingMaximumsTest()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: -2000, maximumActive: 3000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: -8000, maximumActive: 7000);

            _distribution.Distribute(new ActivePower(5000), new ReactivePower(0), new[] { unit1.Object, unit2.Object });

            // 3 / 10 * 5,000 = 1,500
            // 7 / 10 * 5,000 = 3,500
            VerifyUpdatePower(unit1, 1500, 0);
            VerifyUpdatePower(unit2, 3500, 0);
        }


        [TestMethod]
        public void DistributeMixedActiveLimitsDistributesNegativeTargetUsingMinimumsTest()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: -2000, maximumActive: 3000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: -8000, maximumActive: 7000);

            _distribution.Distribute(new ActivePower(-5000), new ReactivePower(0), new[] { unit1.Object, unit2.Object });

            // -2 / -10 * -5,000 = -1,000
            // -8 / -10 * -5,000 = -4,000
            VerifyUpdatePower(unit1, -1000, 0);
            VerifyUpdatePower(unit2, -4000, 0);
        }


        [TestMethod]
        public void DistributeActiveAndReactivePowerDistributesBothIndependentlyTest()
        {
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(minimumActive: -2000, maximumActive: 6000, minimumReactive: -1000, maximumReactive: 2000);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(minimumActive: -4000, maximumActive: 12000, minimumReactive: -3000, maximumReactive: 4000);

            _distribution.Distribute(new ActivePower(6000), new ReactivePower(-2000), new[] { unit1.Object, unit2.Object });

            // Active: 6 / 18 * 6,000 = 2,000
            // Active: 12 / 18 * 6,000 = 4,000
            // Reactive: -1 / -4 * -2,000 = -500
            // Reactive: -3 / -4 * -2,000 = -1,500
            VerifyUpdatePower(unit1, 2000, -500);
            VerifyUpdatePower(unit2, 4000, -1500);
        }


        private Mock<IDerUnitPowerControl> CreateUnit(bool enabled = true, double minimumActive = -10000, double maximumActive = 10000,
            double minimumReactive = -10000, double maximumReactive = 10000)
        {
            Mock<IDerUnitPowerControl> mock = new Mock<IDerUnitPowerControl>();
            mock.SetupGet(x => x.IsEnabled).Returns(enabled);
            mock.SetupGet(x => x.MinimumActivePower).Returns(new ActivePower(minimumActive));
            mock.SetupGet(x => x.MaximumActivePower).Returns(new ActivePower(maximumActive));
            mock.SetupGet(x => x.MinimumReactivePower).Returns(new ReactivePower(minimumReactive));
            mock.SetupGet(x => x.MaximumReactivePower).Returns(new ReactivePower(maximumReactive));

            return mock;
        }


        private void VerifyUpdatePower(Mock<IDerUnitPowerControl> unit, double expectedActive, double expectedReactive)
        {
            unit.Verify(x => x.UpdatePower(It.Is<ActivePower>(p => p.Watts == expectedActive),
                It.Is<ReactivePower>(p => p.VoltAmperesReactive == expectedReactive)), Times.Once);
        }
    }
}

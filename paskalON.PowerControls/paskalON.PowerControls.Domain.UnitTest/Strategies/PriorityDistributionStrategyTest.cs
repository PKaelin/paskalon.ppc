// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.ConstraintEngine.Domain;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Ders;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Strategies;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Domain.Strategies;
using paskalON.Telemetry;
using System.Text.RegularExpressions;

namespace paskalON.PowerControls.Domain.UnitTest.Strategies
{
    [TestClass]
    public class PriorityDistributionStrategyTest
    {
        private DerUnitPowerControlConfig? _derUnitConfig1;
        private DerUnitPowerControlConfig? _derUnitConfig2;
        private DerUnitPowerControlConfig? _derUnitConfig3;
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private FakeLogger _logger = new FakeLogger();
        private PriorityDistributionStrategy? _distribution;


        [TestInitialize]
        public void Initialize()
        {
            _distribution = new PriorityDistributionStrategy(_logger);

            _derUnitConfig1 = new DerUnitPowerControlConfig
            {
                ChangedBy = "T",
                Name = "PCU1",
                DerUnitName = "U1",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal,
                Priority = 1
            };

            _derUnitConfig2 = new DerUnitPowerControlConfig
            {
                ChangedBy = "T",
                Name = "PCU2",
                DerUnitName = "U1",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal,
                Priority = 2
            };

            _derUnitConfig3 = new DerUnitPowerControlConfig
            {
                ChangedBy = "T",
                Name = "PCU2",
                DerUnitName = "U1",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal,
                Priority = 3
            };
        }


        [TestMethod]
        public void PriorityDistributionEmptyUnitsTest()
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
        public void PriorityDistributionOneUnitNotStartedNoConstraintTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            DerUnitPowerControlMap map = new DerUnitPowerControlMap { State = () => DerState.Stopped };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map, _publisher.Object, new List<IDerUnitConstraint>());
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit1 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 20.*achieved: 0.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 10.*achieved: 0.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
        }


        [TestMethod]
        public void PriorityDistributionOneUnitStartedNoConstraintTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map1, _publisher.Object, new List<IDerUnitConstraint>());
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit1 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 20.*achieved: 20.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 10.*achieved: 10.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
            Assert.AreEqual(20, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(10, unit1.TargetReactivePower.KiloVoltAmperesReactive);
        }


        [TestMethod]
        public void PriorityDistributionTwoUnitStartedNoConstraintTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map1, _publisher.Object, new List<IDerUnitConstraint>());
            DerUnitPowerControl unit2 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig2!, map2, _publisher.Object, new List<IDerUnitConstraint>());
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit1, unit2 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 20.*achieved: 20.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 10.*achieved: 10.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
            Assert.AreEqual(20, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(10, unit1.TargetReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, unit2.TargetActivePower.KiloWatts);
            Assert.AreEqual(0, unit2.TargetReactivePower.KiloVoltAmperesReactive);
        }


        [TestMethod]
        public void PriorityDistributionTwoUnitStartedNoConstraintZeroSystemTargetTest()
        {
            ActivePower active = ActivePower.FromKilo(0);
            ReactivePower reactive = ReactivePower.FromKilo(0);

            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map1, _publisher.Object, new List<IDerUnitConstraint>());
            DerUnitPowerControl unit2 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig2!, map2, _publisher.Object, new List<IDerUnitConstraint>());
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit1, unit2 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 0.*achieved: 0.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 0.*achieved: 0.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
            Assert.AreEqual(0, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(0, unit1.TargetReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(0, unit2.TargetActivePower.KiloWatts);
            Assert.AreEqual(0, unit2.TargetReactivePower.KiloVoltAmperesReactive);
        }


        [TestMethod]
        public void PriorityDistributionThreeUnitStartedOneStoppedOneMaintenanceNoConstraintTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Stopped };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Maintenance };
            DerUnitPowerControlMap map3 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map1, _publisher.Object, new List<IDerUnitConstraint>());
            DerUnitPowerControl unit2 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig2!, map2, _publisher.Object, new List<IDerUnitConstraint>());
            DerUnitPowerControl unit3 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig3!, map3, _publisher.Object, new List<IDerUnitConstraint>());
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit1, unit2, unit3 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 20.*achieved: 20.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 10.*achieved: 10.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
            Assert.AreEqual(0, unit1.TargetActivePower.KiloWattsPrecision);
            Assert.AreEqual(0, unit1.TargetReactivePower.KiloVoltAmperesReactivePrecision);
            Assert.AreEqual(0, unit2.TargetActivePower.KiloWattsPrecision);
            Assert.AreEqual(0, unit2.TargetReactivePower.KiloVoltAmperesReactivePrecision);
            Assert.AreEqual(20, unit3.TargetActivePower.KiloWattsPrecision);
            Assert.AreEqual(10, unit3.TargetReactivePower.KiloVoltAmperesReactivePrecision);
        }


        [TestMethod]
        public void PriorityDistributionTwoUnitStartedConstraintBelowTargetTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            DerUnitPowerConstraintConfig unitConstraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerConstraintConfig",
                MaximumActivePowerKiloWatt = 15,
                MinimumActivePowerKiloWatt = -15,
                MaximumReactivePowerKiloVars = 5,
                MinimumReactivePowerKiloVars = -5
            };

            DerUnitPowerConstraint unitConstraint = new DerUnitPowerConstraint(NullLogger.Instance, unitConstraintConfig, new DerUnitPowerConstraintMap());
            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map1, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            DerUnitPowerControl unit2 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig2!, map2, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit2, unit1 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 20.*achieved: 20.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 10.*achieved: 10.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
            Assert.AreEqual(15, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(5, unit1.TargetReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(5, unit2.TargetActivePower.KiloWatts);
            Assert.AreEqual(5, unit2.TargetReactivePower.KiloVoltAmperesReactive);
        }


        [TestMethod]
        public void PriorityDistributionTwoUnitStartedConstraintBelowTargetNegativeTest()
        {
            ActivePower active = ActivePower.FromKilo(-20);
            ReactivePower reactive = ReactivePower.FromKilo(-10);

            DerUnitPowerConstraintConfig unitConstraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerConstraintConfig",
                MaximumActivePowerKiloWatt = 15,
                MinimumActivePowerKiloWatt = -15,
                MaximumReactivePowerKiloVars = 5,
                MinimumReactivePowerKiloVars = -5
            };

            DerUnitPowerConstraint unitConstraint = new DerUnitPowerConstraint(NullLogger.Instance, unitConstraintConfig, new DerUnitPowerConstraintMap());
            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map1, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            DerUnitPowerControl unit2 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig2!, map2, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit1, unit2 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: -20.*achieved: -20.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: -10.*achieved: -10.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
            Assert.AreEqual(-15, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(-5, unit1.TargetReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(-5, unit2.TargetActivePower.KiloWatts);
            Assert.AreEqual(-5, unit2.TargetReactivePower.KiloVoltAmperesReactive);
        }
    }
}

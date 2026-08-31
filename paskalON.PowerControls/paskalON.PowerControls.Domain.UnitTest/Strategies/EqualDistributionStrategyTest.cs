// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
    public class EqualDistributionStrategyTest
    {
        private DerUnitPowerControlConfig? _derUnitConfig1;
        private DerUnitPowerControlConfig? _derUnitConfig2;
        private DerUnitPowerControlConfig? _derUnitConfig3;
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private FakeLogger _logger = new FakeLogger();
        private EqualDistributionStrategy? _distribution;


        [TestInitialize]
        public void Initialize()
        {
            _distribution = new EqualDistributionStrategy(_logger);

            _derUnitConfig1 = new DerUnitPowerControlConfig
            {
                ChangedBy = "T",
                Name = "PCU1",
                DerUnitName = "U1",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal
            };

            _derUnitConfig2 = new DerUnitPowerControlConfig
            {
                ChangedBy = "T",
                Name = "PCU2",
                DerUnitName = "U1",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal
            };

            _derUnitConfig3 = new DerUnitPowerControlConfig
            {
                ChangedBy = "T",
                Name = "PCU2",
                DerUnitName = "U1",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal
            };
        }


        [TestMethod]
        public void EqualDistributionEmptyUnitsTest()
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
        public void EqualDistributionOneUnitNotStartedNoConstraintTest()
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
        public void EqualDistributionOneUnitStartedNoConstraintTest()
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
        public void EqualDistributionTwoUnitStartedNoConstraintTest()
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
            Assert.AreEqual(10, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(5, unit1.TargetReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(10, unit2.TargetActivePower.KiloWatts);
            Assert.AreEqual(5, unit2.TargetReactivePower.KiloVoltAmperesReactive);
        }


        [TestMethod]
        public void EqualDistributionTwoUnitStartedNoConstraintZeroSystemTargetTest()
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
        public void EqualDistributionThreeUnitStartedNoConstraintTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Started };
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
            Assert.AreEqual(6.66667, unit1.TargetActivePower.KiloWattsPrecision);
            Assert.AreEqual(3.33333, unit1.TargetReactivePower.KiloVoltAmperesReactivePrecision);
            Assert.AreEqual(6.66667, unit2.TargetActivePower.KiloWattsPrecision);
            Assert.AreEqual(3.33333, unit2.TargetReactivePower.KiloVoltAmperesReactivePrecision);
            Assert.AreEqual(6.66667, unit3.TargetActivePower.KiloWattsPrecision);
            Assert.AreEqual(3.33333, unit3.TargetReactivePower.KiloVoltAmperesReactivePrecision);
        }


        [TestMethod]
        public void EqualDistributionThreeUnitStartedOneStoppedOneMaintenanceNoConstraintTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Stopped };
            DerUnitPowerControlMap map3 = new DerUnitPowerControlMap { State = () => DerState.Maintenance };
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
            Assert.AreEqual(20, unit1.TargetActivePower.KiloWattsPrecision);
            Assert.AreEqual(10, unit1.TargetReactivePower.KiloVoltAmperesReactivePrecision);
            Assert.AreEqual(0, unit2.TargetActivePower.KiloWattsPrecision);
            Assert.AreEqual(0, unit2.TargetReactivePower.KiloVoltAmperesReactivePrecision);
            Assert.AreEqual(0, unit3.TargetActivePower.KiloWattsPrecision);
            Assert.AreEqual(0, unit3.TargetReactivePower.KiloVoltAmperesReactivePrecision);
        }


        [TestMethod]
        public void EqualDistributionTwoUnitStartedConstraintAboveTargetTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            DerUnitPowerConstraintConfig unitConstraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerConstraintConfig",
                MaximumActivePowerKiloWatt = 20,
                MinimumActivePowerKiloWatt = -20,
                MaximumReactivePowerKiloVars = 10,
                MinimumReactivePowerKiloVars = -10
            };

            DerUnitPowerConstraint unitConstraint = new DerUnitPowerConstraint(NullLogger.Instance, unitConstraintConfig, new DerUnitPowerConstraintMap());
            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map1, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            DerUnitPowerControl unit2 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig2!, map2, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit1, unit2 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 20.*achieved: 20.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 10.*achieved: 10.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
            Assert.AreEqual(10, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(5, unit1.TargetReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(10, unit2.TargetActivePower.KiloWatts);
            Assert.AreEqual(5, unit2.TargetReactivePower.KiloVoltAmperesReactive);
        }



        [TestMethod]
        public void EqualDistributionTwoUnitStartedConstraintAboveTargetNegativeTest()
        {
            ActivePower active = ActivePower.FromKilo(-20);
            ReactivePower reactive = ReactivePower.FromKilo(-10);

            DerUnitPowerConstraintConfig unitConstraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerConstraintConfig",
                MaximumActivePowerKiloWatt = 20,
                MinimumActivePowerKiloWatt = -20,
                MaximumReactivePowerKiloVars = 10,
                MinimumReactivePowerKiloVars = -10
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
            Assert.AreEqual(-10, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(-5, unit1.TargetReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(-10, unit2.TargetActivePower.KiloWatts);
            Assert.AreEqual(-5, unit2.TargetReactivePower.KiloVoltAmperesReactive);
        }


        [TestMethod]
        public void EqualDistributionTwoUnitStartedConstraintBelowTargetTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            DerUnitPowerConstraintConfig unitConstraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerConstraintConfig",
                MaximumActivePowerKiloWatt = 8,
                MinimumActivePowerKiloWatt = -8,
                MaximumReactivePowerKiloVars = 4,
                MinimumReactivePowerKiloVars = -4
            };

            DerUnitPowerConstraint unitConstraint = new DerUnitPowerConstraint(NullLogger.Instance, unitConstraintConfig, new DerUnitPowerConstraintMap());
            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map1, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            DerUnitPowerControl unit2 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig2!, map2, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit1, unit2 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 20.*achieved: 16.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 10.*achieved: 8.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
            Assert.AreEqual(8, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(4, unit1.TargetReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(8, unit2.TargetActivePower.KiloWatts);
            Assert.AreEqual(4, unit2.TargetReactivePower.KiloVoltAmperesReactive);
        }


        [TestMethod]
        public void EqualDistributionTwoUnitStartedConstraintBelowTargetNegativeTest()
        {
            ActivePower active = ActivePower.FromKilo(-20);
            ReactivePower reactive = ReactivePower.FromKilo(-10);

            DerUnitPowerConstraintConfig unitConstraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerConstraintConfig",
                MaximumActivePowerKiloWatt = 8,
                MinimumActivePowerKiloWatt = -8,
                MaximumReactivePowerKiloVars = 4,
                MinimumReactivePowerKiloVars = -4
            };

            DerUnitPowerConstraint unitConstraint = new DerUnitPowerConstraint(NullLogger.Instance, unitConstraintConfig, new DerUnitPowerConstraintMap());
            DerUnitPowerControlMap map1 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControlMap map2 = new DerUnitPowerControlMap { State = () => DerState.Started };
            DerUnitPowerControl unit1 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig1!, map1, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            DerUnitPowerControl unit2 = new DerUnitPowerControl(NullLogger.Instance, _derUnitConfig2!, map2, _publisher.Object, new List<IDerUnitConstraint> { unitConstraint });
            List<DerUnitPowerControl> units = new List<DerUnitPowerControl> { unit1, unit2 };

            _distribution!.Distribute(active, reactive, units);

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: -20.*achieved: -16.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: -10.*achieved: -8.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
            Assert.AreEqual(-8, unit1.TargetActivePower.KiloWatts);
            Assert.AreEqual(-4, unit1.TargetReactivePower.KiloVoltAmperesReactive);
            Assert.AreEqual(-8, unit2.TargetActivePower.KiloWatts);
            Assert.AreEqual(-4, unit2.TargetReactivePower.KiloVoltAmperesReactive);
        }

    }
}


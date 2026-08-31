// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Moq;
using paskalON.ConstraintEngine.Domain;
using paskalON.PowerControls.Domain.Configs.Systems;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Domain.Strategies;
using paskalON.PowerControls.Domain.Systems;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Domain.UnitTest.System
{
    [TestClass]
    public sealed class SystemPowerControlTest
    {
        [TestMethod]
        public void SystemPowerControlNullLoggerTest()
        {
            Mock<SystemPowerControlConfig> config = new Mock<SystemPowerControlConfig>();
            Mock<SystemPowerControlMap> map = new Mock<SystemPowerControlMap>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<ISystemConstraint> constraints = new List<ISystemConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(null, config.Object, map.Object, publisher.Object, constraints, units, distributions.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }



        [TestMethod]
        public void SystemPowerControlNullConfigTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<SystemPowerControlMap> map = new Mock<SystemPowerControlMap>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<ISystemConstraint> constraints = new List<ISystemConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(logger.Object, null, map.Object, publisher.Object, constraints, units, distributions.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }



        [TestMethod]
        public void SystemPowerControlNullMapTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<SystemPowerControlConfig> config = new Mock<SystemPowerControlConfig>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<ISystemConstraint> constraints = new List<ISystemConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(logger.Object, config.Object, null, publisher.Object, constraints, units, distributions.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void SystemPowerControlNullPublisherTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<SystemPowerControlConfig> config = new Mock<SystemPowerControlConfig>();
            Mock<SystemPowerControlMap> map = new Mock<SystemPowerControlMap>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<ISystemConstraint> constraints = new List<ISystemConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(logger.Object, config.Object, map.Object, null, constraints, units, distributions.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void SystemPowerControlNullConstraintsTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<SystemPowerControlConfig> config = new Mock<SystemPowerControlConfig>();
            Mock<SystemPowerControlMap> map = new Mock<SystemPowerControlMap>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(logger.Object, config.Object, map.Object, publisher.Object, null, units, distributions.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void SystemPowerControlNullUnitsTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<SystemPowerControlConfig> config = new Mock<SystemPowerControlConfig>();
            Mock<SystemPowerControlMap> map = new Mock<SystemPowerControlMap>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<ISystemConstraint> constraints = new List<ISystemConstraint>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(logger.Object, config.Object, map.Object, publisher.Object, constraints, null, distributions.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void SystemPowerControlNullDistributionsTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<SystemPowerControlConfig> config = new Mock<SystemPowerControlConfig>();
            Mock<SystemPowerControlMap> map = new Mock<SystemPowerControlMap>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            IEnumerable<ISystemConstraint> constraints = new List<ISystemConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerControl(logger.Object, config.Object, map.Object, publisher.Object, constraints, units, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void SystemPowerControlConstructorTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<SystemPowerControlConfig> config = new Mock<SystemPowerControlConfig>();
            Mock<SystemPowerControlMap> map = new Mock<SystemPowerControlMap>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<ISystemConstraint> constraints = new List<ISystemConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

            SystemPowerControl control = new SystemPowerControl(logger.Object, config.Object, map.Object, publisher.Object, constraints, units, distributions.Object);

            Assert.IsNotNull(control.MetricsPublisher);
        }
    }
}

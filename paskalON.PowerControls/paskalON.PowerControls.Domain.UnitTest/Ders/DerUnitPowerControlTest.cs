// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Moq;
using paskalON.ConstraintEngine.Domain;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Domain.Strategies;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Domain.UnitTest.Ders
{
    [TestClass]
    public class DerUnitPowerControlTest
    {
        [TestMethod]
        public void DerUnitPowerControlNullLoggerTest()
        {
            Mock<DerUnitPowerControlConfig> config = new Mock<DerUnitPowerControlConfig>();
            Mock<DerUnitPowerControlMap> map = new Mock<DerUnitPowerControlMap>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<IDerUnitConstraint> constraints = new List<IDerUnitConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(null, config.Object, map.Object, publisher.Object, constraints));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void DerUnitPowerControlNullConfigTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<DerUnitPowerControlMap> map = new Mock<DerUnitPowerControlMap>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<IDerUnitConstraint> constraints = new List<IDerUnitConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(logger.Object, null, map.Object, publisher.Object, constraints));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void DerUnitPowerControlNullMapConfigTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<DerUnitPowerControlConfig> config = new Mock<DerUnitPowerControlConfig>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<IDerUnitConstraint> constraints = new List<IDerUnitConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(logger.Object, config.Object, null, publisher.Object, constraints));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void DerUnitPowerControlNullPublisherConfigTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<DerUnitPowerControlConfig> config = new Mock<DerUnitPowerControlConfig>();
            Mock<DerUnitPowerControlMap> map = new Mock<DerUnitPowerControlMap>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<IDerUnitConstraint> constraints = new List<IDerUnitConstraint>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(logger.Object, config.Object, null, null, constraints));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void DerUnitPowerControlNullConstraintsConfigTest()
        {
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<DerUnitPowerControlConfig> config = new Mock<DerUnitPowerControlConfig>();
            Mock<DerUnitPowerControlMap> map = new Mock<DerUnitPowerControlMap>();
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<DistributionStrategyProfile> distributions = new Mock<DistributionStrategyProfile>();
            IEnumerable<DerUnitPowerControl> units = new List<DerUnitPowerControl>();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(logger.Object, config.Object, null, publisher.Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


    }
}

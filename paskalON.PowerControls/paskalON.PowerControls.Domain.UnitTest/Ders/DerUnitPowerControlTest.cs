// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.ConstraintEngine.Domain;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Ders;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Strategies;
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

            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(null!, config.Object, map.Object, publisher.Object, constraints));
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

            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(logger.Object, null!, map.Object, publisher.Object, constraints));
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

            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(logger.Object, config.Object, null!, publisher.Object, constraints));
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

            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(logger.Object, config.Object, null!, null!, constraints));
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

            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitPowerControl(logger.Object, config.Object, null!, publisher.Object, null));
        }


        [TestMethod]
        [DataRow(DistributionStrategyType.Proportional)]
        [DataRow(DistributionStrategyType.WaterFilling)]
        public void DerUnitPowerControlCapacityDistributionRequiresAllLimitsTest(DistributionStrategyType strategy)
        {
            DerUnitPowerControlConfig config = new DerUnitPowerControlConfig
            {
                ChangedBy = "Test",
                Name = "UnitControl",
                DerUnitName = "Unit",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = strategy
            };

            DerUnitPowerConstraintConfig constraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerLimits",
                MaximumActivePowerWatt = 1000,
                MinimumActivePowerWatt = -1000
            };

            DerUnitPowerConstraint constraint = new DerUnitPowerConstraint(NullLogger.Instance, constraintConfig, new DerUnitPowerConstraintMap());

            Assert.ThrowsExactly<InvalidOperationException>(() => new DerUnitPowerControl(NullLogger.Instance, config,
                new DerUnitPowerControlMap { PcsDeviceId = 1, State = () => DerState.Started }, Mock.Of<IMetricsPublisher>(), [constraint]));
        }


        [TestMethod]
        [DataRow(DistributionStrategyType.Proportional)]
        [DataRow(DistributionStrategyType.WaterFilling)]
        public void DerUnitPowerEnergyStorageControlCapacityDistributionRequiresAllLimitsTest(DistributionStrategyType strategy)
        {
            DerUnitEnergyStoragePowerControlConfig config = new DerUnitEnergyStoragePowerControlConfig
            {
                ChangedBy = "Test",
                Name = "StorageControl",
                DerUnitName = "Storage",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = strategy
            };

            DerUnitPowerConstraintConfig constraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerLimits",
                MaximumActivePowerWatt = 1000,
                MinimumActivePowerWatt = -1000
            };

            DerUnitPowerConstraint constraint = new DerUnitPowerConstraint(NullLogger.Instance, constraintConfig, new DerUnitPowerConstraintMap());

            Assert.ThrowsExactly<InvalidOperationException>(() => new DerUnitPowerEnergyStorageControl(NullLogger.Instance, config,
                new DerUnitPowerEnergyStorageControlMap
                {
                    PcsDeviceId = 1,
                    State = () => DerState.Started,
                    StateOfCharge = () => 50,
                    StateOfChargeMaximum = () => 100,
                    StateOfChargeMinimum = () => 0
                }, Mock.Of<IMetricsPublisher>(), [constraint]));
        }
    }
}

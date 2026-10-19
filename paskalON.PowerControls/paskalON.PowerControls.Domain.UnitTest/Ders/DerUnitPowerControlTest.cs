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
using paskalON.PhysicalUnits.Electricals.Powers;
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


        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void SetMaintenanceTargetSetsTargetAndRaisesEventTest(bool isEnergyStorage)
        {
            IDerUnitPowerControl unit = CreateUnit(isEnergyStorage);
            int systemTargetChanges = 0;
            int maintenanceTargetChanges = 0;
            unit.TargetPowerChanged += (sender, args) => systemTargetChanges++;
            unit.MaintenanceTargetChanged += (sender, args) => maintenanceTargetChanges++;

            unit.SetMaintenanceTarget(new ActivePower(-30000), new ReactivePower(5000));

            Assert.IsTrue(unit.IsMaintenanceTargetSet);
            Assert.AreEqual(new ActivePower(-30000), unit.MaintenanceTargetActivePower);
            Assert.AreEqual(new ReactivePower(5000), unit.MaintenanceTargetReactivePower);
            Assert.AreEqual(1, maintenanceTargetChanges);
            Assert.AreEqual(0, systemTargetChanges);
            Assert.AreEqual(new ActivePower(0), unit.TargetActivePower);
            Assert.AreEqual(new ReactivePower(0), unit.TargetReactivePower);
        }


        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void UpdatePowerClearsMaintenanceTargetTest(bool isEnergyStorage)
        {
            IDerUnitPowerControl unit = CreateUnit(isEnergyStorage);
            unit.SetMaintenanceTarget(new ActivePower(-30000), new ReactivePower(5000));

            unit.UpdatePower(new ActivePower(20000), new ReactivePower(1000));

            Assert.IsFalse(unit.IsMaintenanceTargetSet);
            Assert.AreEqual(new ActivePower(20000), unit.TargetActivePower);
            Assert.AreEqual(new ReactivePower(1000), unit.TargetReactivePower);
        }


        private IDerUnitPowerControl CreateUnit(bool isEnergyStorage)
        {
            DerUnitPowerConstraintConfig constraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "PowerLimits",
                MaximumActivePowerWatt = 50000,
                MinimumActivePowerWatt = -50000,
                MaximumReactivePowerVars = 10000,
                MinimumReactivePowerVars = -10000
            };
            DerUnitPowerConstraint constraint = new DerUnitPowerConstraint(NullLogger.Instance, constraintConfig, new DerUnitPowerConstraintMap());

            if (isEnergyStorage)
            {
                DerUnitEnergyStoragePowerControlConfig storageConfig = new DerUnitEnergyStoragePowerControlConfig
                {
                    ChangedBy = "Test",
                    Name = "StorageControl",
                    DerUnitName = "BESS-Unit 1",
                    IsActive = true,
                    IsEnabled = true,
                    DistributionStrategyType = DistributionStrategyType.Equal
                };
                DerUnitPowerEnergyStorageControlMap storageMap = new DerUnitPowerEnergyStorageControlMap
                {
                    PcsDeviceId = 1,
                    State = () => DerState.Maintenance,
                    StateOfCharge = () => 50,
                    StateOfChargeMaximum = () => 90,
                    StateOfChargeMinimum = () => 10
                };

                return new DerUnitPowerEnergyStorageControl(NullLogger.Instance, storageConfig, storageMap, Mock.Of<IMetricsPublisher>(), [constraint]);
            }

            DerUnitPowerControlConfig config = new DerUnitPowerControlConfig
            {
                ChangedBy = "Test",
                Name = "UnitControl",
                DerUnitName = "PV-Unit 1",
                IsActive = true,
                IsEnabled = true,
                DistributionStrategyType = DistributionStrategyType.Equal
            };

            return new DerUnitPowerControl(NullLogger.Instance, config, new DerUnitPowerControlMap { PcsDeviceId = 1, State = () => DerState.Maintenance },
                Mock.Of<IMetricsPublisher>(), [constraint]);
        }
    }
}

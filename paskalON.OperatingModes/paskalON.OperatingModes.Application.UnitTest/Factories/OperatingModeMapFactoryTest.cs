// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Application.Factories;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Application.UnitTest.Factories
{
    /// <summary>
    /// Tests the mapping of the device data in <see cref="OperatingModeMapFactory"/>.
    /// </summary>
    /// <remarks>
    /// Test plant: Bess1 (2 MW / 1 MVar available), Bess2 (3 MW / 1.5 MVar available), Solar1 (4 MW available, no reactive power),
    /// Bess3 (5 MW available but communication error) and a system power meter measuring 6 MW / 0.5 MVar.
    /// The measured output of all units differs from the available power to make sure the available power is used.
    /// </remarks>
    [TestClass]
    public class OperatingModeMapFactoryTest
    {
        /// <summary>
        /// The map factory under test.
        /// </summary>
        private readonly OperatingModeMapFactory _factory = new OperatingModeMapFactory();


        /// <summary>
        /// The DER of the test plant.
        /// </summary>
        private DerDto? _der;


        /// <summary>
        /// Names of the overridden DER units.
        /// </summary>
        private HashSet<string>? _overriddenUnits;


        /// <summary>
        /// Lookup of overridden DER units backed by <see cref="_overriddenUnits"/>.
        /// </summary>
        private IUnitOverrideLookup? _overrides;


        /// <summary>
        /// Creates the DER of the test plant without overridden units.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _der = CreateDer();
            _overriddenUnits = new HashSet<string>(StringComparer.Ordinal);
            Mock<IUnitOverrideLookup> overrides = new Mock<IUnitOverrideLookup>();
            overrides.Setup(lookup => lookup.IsOverridden(It.IsAny<string>())).Returns<string>(name => _overriddenUnits.Contains(name));
            _overrides = overrides.Object;
        }


        /// <summary>
        /// Confirms the map type created for each supported configuration.
        /// </summary>
        /// <param name="configKind">The kind of configuration.</param>
        /// <param name="expectedMapType">The expected map type.</param>
        [TestMethod]
        [DataRow("Maintenance", typeof(MaintenanceModeMap))]
        [DataRow("ActivePowerFixed", typeof(ActivePowerFixedModeMap))]
        [DataRow("ReactivePowerFixed", typeof(ReactivePowerFixedModeMap))]
        [DataRow("MaximumPowerPointTracking", typeof(MaximumPowerPointTrackingModeMap))]
        [DataRow("ActivePower", typeof(ActivePowerModeMap))]
        [DataRow("ReactivePower", typeof(ReactivePowerModeMap))]
        [DataRow("ActivePowerLimit", typeof(ActivePowerLimitModeMap))]
        [DataRow("ReactivePowerLimit", typeof(ReactivePowerLimitModeMap))]
        [DataRow("Shutdown", typeof(OperatingModeBaseMap))]
        public void OperatingModeMapFactoryCreateMapTypeTest(string configKind, Type expectedMapType)
        {
            OperatingModeBaseMap? map = _factory.Create(CreateConfig(configKind), _der!, _overrides!);

            Assert.IsNotNull(map);
            Assert.AreEqual(expectedMapType, map.GetType());
        }


        /// <summary>
        /// Confirms unsupported configurations return no map.
        /// </summary>
        [TestMethod]
        public void OperatingModeMapFactoryCreateUnsupportedReturnsNullTest()
        {
            OperatingModeBaseMap? map = _factory.Create(CreateConfig("EmergencyShutdown"), _der!, _overrides!);

            Assert.IsNull(map);
        }


        /// <summary>
        /// Confirms the plant-level available power sums the available power of the reachable units.
        /// </summary>
        [TestMethod]
        public void OperatingModeMapFactoryPlantAvailablePowerTest()
        {
            OperatingModeBaseMap map = _factory.Create(CreateConfig("ActivePowerFixed"), _der!, _overrides!)!;

            ActivePower? availableActive = map.AvailableActivePower.Invoke();
            ReactivePower? availableReactive = map.AvailableReactivePower.Invoke();

            Assert.AreEqual(9000000, availableActive!.Value.Watts);
            Assert.AreEqual(2500000, availableReactive!.Value.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms overridden units are excluded from the plant-level available power at runtime.
        /// </summary>
        [TestMethod]
        public void OperatingModeMapFactoryPlantAvailableExcludesOverriddenUnitsTest()
        {
            OperatingModeBaseMap map = _factory.Create(CreateConfig("ActivePowerFixed"), _der!, _overrides!)!;
            double availableBeforeOverride = map.AvailableActivePower.Invoke()!.Value.Watts;

            _overriddenUnits!.Add("Bess1");

            Assert.AreEqual(9000000, availableBeforeOverride);
            Assert.AreEqual(7000000, map.AvailableActivePower.Invoke()!.Value.Watts);
            Assert.AreEqual(1500000, map.AvailableReactivePower.Invoke()!.Value.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms the maintenance mode only uses the available power of its DER unit, also while it is overridden.
        /// </summary>
        [TestMethod]
        public void OperatingModeMapFactoryMaintenanceUsesOnlyItsUnitTest()
        {
            _overriddenUnits!.Add("Bess2");

            MaintenanceModeMap map = (MaintenanceModeMap)_factory.Create(CreateConfig("Maintenance"), _der!, _overrides!)!;

            Assert.AreEqual("Bess2", map.DerUnitName);
            Assert.AreEqual(3000000, map.AvailableActivePower.Invoke()!.Value.Watts);
            Assert.AreEqual(1500000, map.AvailableReactivePower.Invoke()!.Value.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms an unknown maintenance DER unit throws with the DER unit name.
        /// </summary>
        [TestMethod]
        public void OperatingModeMapFactoryMaintenanceUnknownUnitThrowsTest()
        {
            MaintenanceModeConfig config = (MaintenanceModeConfig)CreateConfig("Maintenance");
            config.DerUnitName = "Bess9";

            InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() => _factory.Create(config, _der!, _overrides!));
            Assert.Contains("Bess9", exception.Message);
        }


        /// <summary>
        /// Confirms the available power is null when no unit reports it.
        /// </summary>
        [TestMethod]
        public void OperatingModeMapFactoryAvailableNullWithoutReportingUnitsTest()
        {
            _overriddenUnits!.UnionWith(new[] { "Bess1", "Bess2", "Solar1" });

            OperatingModeBaseMap map = _factory.Create(CreateConfig("ActivePowerFixed"), _der!, _overrides!)!;

            Assert.IsNull(map.AvailableActivePower.Invoke());
            Assert.IsNull(map.AvailableReactivePower.Invoke());
        }


        /// <summary>
        /// Confirms the closed loop maps measure the power at the POI with the system power meter.
        /// </summary>
        [TestMethod]
        public void OperatingModeMapFactoryPowerAtPoiTest()
        {
            ActivePowerModeMap activeMap = (ActivePowerModeMap)_factory.Create(CreateConfig("ActivePower"), _der!, _overrides!)!;
            ReactivePowerModeMap reactiveMap = (ReactivePowerModeMap)_factory.Create(CreateConfig("ReactivePower"), _der!, _overrides!)!;
            ActivePowerLimitModeMap limitMap = (ActivePowerLimitModeMap)_factory.Create(CreateConfig("ActivePowerLimit"), _der!, _overrides!)!;
            ReactivePowerLimitModeMap reactiveLimitMap = (ReactivePowerLimitModeMap)_factory.Create(CreateConfig("ReactivePowerLimit"), _der!, _overrides!)!;

            ActivePower? activePowerAtPoi = activeMap.ActivePowerAtPoi.Invoke();
            ReactivePower? reactivePowerAtPoi = reactiveMap.ReactivePowerAtPoi.Invoke();

            Assert.AreEqual(6000000, activePowerAtPoi!.Value.Watts);
            Assert.AreEqual(500000, reactivePowerAtPoi!.Value.VoltAmperesReactive);
            Assert.AreEqual(6000000, limitMap.ActivePowerAtPoi.Invoke()!.Value.Watts);
            Assert.AreEqual(500000, reactiveLimitMap.ReactivePowerAtPoi.Invoke()!.Value.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms the system map measures the power at the POI with the system power meter and gets the hold reason from the given function.
        /// </summary>
        [TestMethod]
        public void OperatingModeMapFactoryCreateSystemMapTest()
        {
            SystemOperatingModeControlMap map = _factory.CreateSystemMap(_der!, () => "Lost dependencies: PowerControl.");

            ActivePower? activePowerAtPoi = map.ActivePowerAtPoi.Invoke();
            ReactivePower? reactivePowerAtPoi = map.ReactivePowerAtPoi.Invoke();
            string? holdReason = map.HoldReason.Invoke();

            Assert.AreEqual(6000000, activePowerAtPoi!.Value.Watts);
            Assert.AreEqual(500000, reactivePowerAtPoi!.Value.VoltAmperesReactive);
            Assert.AreEqual("Lost dependencies: PowerControl.", holdReason);
            Assert.ThrowsExactly<ArgumentNullException>(() => _factory.CreateSystemMap(null!, () => null));
            Assert.ThrowsExactly<ArgumentNullException>(() => _factory.CreateSystemMap(_der!, null!));
        }


        /// <summary>
        /// Confirms null arguments are rejected.
        /// </summary>
        [TestMethod]
        public void OperatingModeMapFactoryCreateNullArgumentsThrowTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _factory.Create(null!, _der!, _overrides!));
            Assert.ThrowsExactly<ArgumentNullException>(() => _factory.Create(CreateConfig("ActivePowerFixed"), null!, _overrides!));
            Assert.ThrowsExactly<ArgumentNullException>(() => _factory.Create(CreateConfig("ActivePowerFixed"), _der!, null!));
        }


        /// <summary>
        /// Creates the configuration of the specified kind.
        /// </summary>
        /// <param name="configKind">The kind of configuration.</param>
        /// <returns>The operating mode configuration.</returns>
        private OperatingModeBaseConfig CreateConfig(string configKind)
        {
            return configKind switch
            {
                "Maintenance" => new MaintenanceModeConfig { ChangedBy = "Test", Name = "MaintenanceBess2", IsActive = true, Type = PowerControlType.Bess, DerUnitName = "Bess2", RampConfig = ApplicationTestData.CreateRampConfig() },
                "ActivePowerFixed" => new ActivePowerFixedModeConfig { ChangedBy = "Test", Name = "ActivePowerFixedMode", IsActive = true, Type = PowerControlType.Bess, RampConfig = ApplicationTestData.CreateRampConfig() },
                "ReactivePowerFixed" => new ReactivePowerFixedModeConfig { ChangedBy = "Test", Name = "ReactivePowerFixedMode", IsActive = true, Type = PowerControlType.Bess, RampConfig = ApplicationTestData.CreateRampConfig() },
                "MaximumPowerPointTracking" => new MaximumPowerPointTrackingModeConfig { ChangedBy = "Test", Name = "MaximumPowerPointTrackingMode", IsActive = true, Type = PowerControlType.Bess, RampConfig = ApplicationTestData.CreateRampConfig() },
                "ActivePower" => new ActivePowerModeConfig { ChangedBy = "Test", Name = "ActivePowerMode", IsActive = true, Type = PowerControlType.Bess, RampConfig = ApplicationTestData.CreateRampConfig() },
                "ReactivePower" => new ReactivePowerModeConfig { ChangedBy = "Test", Name = "ReactivePowerMode", IsActive = true, Type = PowerControlType.Bess, RampConfig = ApplicationTestData.CreateRampConfig() },
                "ActivePowerLimit" => new ActivePowerLimitModeConfig { ChangedBy = "Test", Name = "ActivePowerLimitMode", IsActive = true, Type = PowerControlType.Bess, MaximumActivePowerLimitWatt = 5000000, RampConfig = ApplicationTestData.CreateRampConfig() },
                "ReactivePowerLimit" => new ReactivePowerLimitModeConfig { ChangedBy = "Test", Name = "ReactivePowerLimitMode", IsActive = true, Type = PowerControlType.Bess, MaximumReactivePowerLimitVars = 2000000, RampConfig = ApplicationTestData.CreateRampConfig() },
                "Shutdown" => new ShutdownModeConfig { ChangedBy = "Test", Name = "ShutdownMode", IsActive = true, Type = PowerControlType.Bess, RampConfig = ApplicationTestData.CreateRampConfig() },
                _ => new EmergencyShutdownModeConfig { ChangedBy = "Test", Name = "EmergencyShutdownMode", IsActive = true, Type = PowerControlType.Bess, RampConfig = ApplicationTestData.CreateRampConfig() },
            };
        }


        /// <summary>
        /// Creates the DER of the test plant.
        /// </summary>
        /// <returns>The DER.</returns>
        private DerDto CreateDer()
        {
            DerCircuitDto circuit = new DerCircuitDto
            {
                Name = "Circuit1",
                DerUnits = new List<DerUnitDto>
                {
                    new DerBatteryStorageUnitDto { Name = "Bess1", PowerConversionSystem = CreatePcs(1, 2000000, 1000000, false) },
                    new DerBatteryStorageUnitDto { Name = "Bess2", PowerConversionSystem = CreatePcs(2, 3000000, 1500000, false) },
                    new DerSolarUnitDto { Name = "Solar1", PowerConversionSystem = CreatePcs(3, 4000000, null, false) },
                    new DerBatteryStorageUnitDto { Name = "Bess3", PowerConversionSystem = CreatePcs(4, 5000000, 2000000, true) },
                }
            };
            PmSystemDto systemPowerMeter = new PmSystemDto(new PmSystemDefinitionDto { DeviceId = 10, Name = "SystemPowerMeter" });
            systemPowerMeter.UpdateCore(new PmSystemCoreDto { DeviceId = 10, ActivePower = new ActivePower(6000000), ReactivePower = new ReactivePower(500000) });

            return new DerDto
            {
                Name = "Plant",
                DerGroups = new List<DerGroupDto> { new DerGroupDto { Name = "Group1", DerCircuits = new List<DerCircuitDto> { circuit } } },
                SystemPowerMeters = new List<PmSystemDto> { systemPowerMeter }
            };
        }


        /// <summary>
        /// Creates a power conversion system that measures 100 kW less than its available active power.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <param name="availableActive">The available active power in Watts.</param>
        /// <param name="availableReactive">The available reactive power in Vars or null if not reported.</param>
        /// <param name="communicationError">Whether the device has a communication error.</param>
        /// <returns>The power conversion system.</returns>
        private PcsDto CreatePcs(int deviceId, double availableActive, double? availableReactive, bool communicationError)
        {
            PcsDto pcs = new PcsDto(new PcsDefinitionDto { DeviceId = deviceId, Name = $"Pcs{deviceId}", TargetAddress = $"10.0.0.{deviceId}", TargetPort = 502 });
            pcs.UpdateCore(new PcsCoreDto
            {
                DeviceId = deviceId,
                CommunicationError = communicationError,
                ActivePower = new ActivePower(availableActive - 100000),
                ActiveAvailablePower = new ActivePower(availableActive),
                ReactivePower = new ReactivePower(0),
                ReactiveAvailablePower = availableReactive.HasValue ? new ReactivePower(availableReactive.Value) : null
            });

            return pcs;
        }
    }
}

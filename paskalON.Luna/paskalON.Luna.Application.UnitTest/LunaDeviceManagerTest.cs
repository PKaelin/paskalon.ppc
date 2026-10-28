// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Devices.Client;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.Luna.Dto.Devices;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Luna.Application.UnitTest
{
    /// <summary>
    /// Tests <see cref="LunaDeviceManager"/> with a mocked device client.
    /// </summary>
    [TestClass]
    public sealed class LunaDeviceManagerTest
    {
        /// <summary>
        /// The mocked device client.
        /// </summary>
        private Mock<IDeviceClient> _deviceClient = null!;


        /// <summary>
        /// The device manager under test.
        /// </summary>
        private LunaDeviceManager _manager = null!;


        /// <summary>
        /// Creates the device manager with a mocked device client.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _deviceClient = new Mock<IDeviceClient>();
            _manager = new LunaDeviceManager(NullLogger<LunaDeviceManager>.Instance, _deviceClient.Object);
        }


        /// <summary>
        /// Confirms the summary is taken from the first system power meter that is connected and has no communication error.
        /// </summary>
        [TestMethod]
        public void GetSystemMeterSummaryUsesFirstConnectedMeterWithoutCommunicationErrorTest()
        {
            List<PmSystemDto> meters = new List<PmSystemDto>
            {
                CreateSystemMeter(1, true, PowerMeterState.Connected, 1000000),
                CreateSystemMeter(2, false, PowerMeterState.Disconnected, 2000000),
                CreateSystemMeter(3, false, PowerMeterState.Connected, 3000000),
                CreateSystemMeter(4, false, PowerMeterState.Connected, 4000000)
            };
            _deviceClient.Setup(client => client.Der).Returns(CreateDer(meters, new List<BbDto>()));

            SystemMeterSummaryDto? summary = _manager.GetSystemMeterSummary();

            Assert.IsNotNull(summary);
            Assert.AreEqual(3000000, summary.ActivePower.Watts);
            Assert.AreEqual(-1500000, summary.ReactivePower.VoltAmperesReactive);
            Assert.AreEqual(-0.89, summary.PowerFactor);
            Assert.AreEqual(50.01, summary.Frequency);
        }


        /// <summary>
        /// Confirms no summary is returned when no system power meter is connected without communication error.
        /// </summary>
        /// <param name="communicationError">The communication error of the system power meter.</param>
        /// <param name="state">The state of the system power meter.</param>
        [TestMethod]
        [DataRow(true, PowerMeterState.Connected)]
        [DataRow(false, PowerMeterState.Unknown)]
        [DataRow(false, PowerMeterState.Disconnected)]
        [DataRow(false, PowerMeterState.Connecting)]
        [DataRow(false, PowerMeterState.Disconnecting)]
        public void GetSystemMeterSummaryWithoutAvailableMeterReturnsNullTest(bool communicationError, PowerMeterState state)
        {
            List<PmSystemDto> meters = new List<PmSystemDto>
            {
                CreateSystemMeter(1, communicationError, state, 1000000),
                new PmSystemDto(CreateSystemMeterDefinition(2))
            };
            _deviceClient.Setup(client => client.Der).Returns(CreateDer(meters, new List<BbDto>()));

            SystemMeterSummaryDto? summary = _manager.GetSystemMeterSummary();

            Assert.IsNull(summary);
        }


        /// <summary>
        /// Confirms no summary is returned when the DER has no system power meter.
        /// </summary>
        [TestMethod]
        public void GetSystemMeterSummaryWithoutMetersReturnsNullTest()
        {
            _deviceClient.Setup(client => client.Der).Returns(CreateDer(new List<PmSystemDto>(), new List<BbDto>()));

            SystemMeterSummaryDto? summary = _manager.GetSystemMeterSummary();

            Assert.IsNull(summary);
        }


        /// <summary>
        /// Confirms missing values of the system power meter are summarized as 0.
        /// </summary>
        [TestMethod]
        public void GetSystemMeterSummaryMissingMeterValuesAreZeroTest()
        {
            PmSystemDto meter = new PmSystemDto(CreateSystemMeterDefinition(1));
            meter.UpdateCore(new PmSystemCoreDto { DeviceId = 1, CommunicationError = false, State = PowerMeterState.Connected });
            _deviceClient.Setup(client => client.Der).Returns(CreateDer(new List<PmSystemDto> { meter }, new List<BbDto>()));

            SystemMeterSummaryDto? summary = _manager.GetSystemMeterSummary();

            Assert.IsNotNull(summary);
            Assert.AreEqual(0, summary.ActivePower.Watts);
            Assert.AreEqual(0, summary.ReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, summary.PowerFactor);
            Assert.AreEqual(0, summary.Frequency);
        }


        /// <summary>
        /// Confirms the state of charge is the average of the battery banks with a state of charge and without communication error.
        /// </summary>
        [TestMethod]
        public void GetSystemMeterSummaryAveragesStateOfChargeTest()
        {
            List<BbDto> batteryBanks = new List<BbDto>
            {
                CreateBatteryBank(1, false, 40),
                CreateBatteryBank(2, false, 70),
                CreateBatteryBank(3, true, 10),
                CreateBatteryBank(4, false, null),
                new BbDto(CreateBatteryBankDefinition(5))
            };
            List<PmSystemDto> meters = new List<PmSystemDto> { CreateSystemMeter(1, false, PowerMeterState.Connected, 1000000) };
            _deviceClient.Setup(client => client.Der).Returns(CreateDer(meters, batteryBanks));

            SystemMeterSummaryDto? summary = _manager.GetSystemMeterSummary();

            Assert.IsNotNull(summary);
            Assert.AreEqual(55, summary.StateOfCharge);
        }


        /// <summary>
        /// Confirms the state of charge is null when no battery bank delivers a state of charge.
        /// </summary>
        [TestMethod]
        public void GetSystemMeterSummaryWithoutBatteryStateOfChargeReturnsNullStateOfChargeTest()
        {
            List<BbDto> batteryBanks = new List<BbDto>
            {
                CreateBatteryBank(1, true, 80),
                CreateBatteryBank(2, false, null)
            };
            List<PmSystemDto> meters = new List<PmSystemDto> { CreateSystemMeter(1, false, PowerMeterState.Connected, 1000000) };
            _deviceClient.Setup(client => client.Der).Returns(CreateDer(meters, batteryBanks));

            SystemMeterSummaryDto? summary = _manager.GetSystemMeterSummary();

            Assert.IsNotNull(summary);
            Assert.IsNull(summary.StateOfCharge);
        }


        /// <summary>
        /// Confirms the system power meter with the device ID is returned.
        /// </summary>
        /// <param name="deviceId">The device ID of the requested system power meter.</param>
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void GetSystemPowerMeterReturnsMeterWithDeviceIdTest(int deviceId)
        {
            List<PmSystemDto> meters = new List<PmSystemDto>
            {
                CreateSystemMeter(1, false, PowerMeterState.Connected, 1000000),
                CreateSystemMeter(2, true, PowerMeterState.Disconnected, 2000000)
            };
            _deviceClient.Setup(client => client.Der).Returns(CreateDer(meters, new List<BbDto>()));

            PmSystemDto? meter = _manager.GetSystemPowerMeter(deviceId);

            Assert.IsNotNull(meter);
            Assert.AreSame(meters[deviceId - 1], meter);
        }


        /// <summary>
        /// Confirms null is returned when the DER has no system power meter with the device ID.
        /// </summary>
        [TestMethod]
        public void GetSystemPowerMeterWithUnknownDeviceIdReturnsNullTest()
        {
            List<PmSystemDto> meters = new List<PmSystemDto> { CreateSystemMeter(1, false, PowerMeterState.Connected, 1000000) };
            _deviceClient.Setup(client => client.Der).Returns(CreateDer(meters, new List<BbDto>()));

            PmSystemDto? meter = _manager.GetSystemPowerMeter(5);

            Assert.IsNull(meter);
        }


        /// <summary>
        /// Confirms the auxiliary power meter with the device ID is returned, or null for an unknown device ID.
        /// </summary>
        /// <param name="deviceId">The device ID of the requested auxiliary power meter.</param>
        /// <param name="isFound">True if the DER has an auxiliary power meter with the device ID.</param>
        [TestMethod]
        [DataRow(11, true)]
        [DataRow(12, true)]
        [DataRow(5, false)]
        public void GetAuxiliaryPowerMeterReturnsMeterWithDeviceIdTest(int deviceId, bool isFound)
        {
            List<PmAuxiliaryDto> meters = new List<PmAuxiliaryDto>
            {
                new PmAuxiliaryDto(new PmAuxiliaryDefinitionDto { DeviceId = 11, Name = "AuxPm11", TargetAddress = "localhost", TargetPort = 502 }),
                new PmAuxiliaryDto(new PmAuxiliaryDefinitionDto { DeviceId = 12, Name = "AuxPm12", TargetAddress = "localhost", TargetPort = 502 })
            };
            DerDto der = CreateDer(new List<PmSystemDto>(), new List<BbDto>()) with { AuxiliaryPowerMeters = meters };
            _deviceClient.Setup(client => client.Der).Returns(der);

            PmAuxiliaryDto? meter = _manager.GetAuxiliaryPowerMeter(deviceId);

            Assert.AreEqual(isFound, meter is not null);
            Assert.AreSame(meters.FirstOrDefault(expected => expected.DeviceId == deviceId), meter);
        }


        /// <summary>
        /// Confirms the external power meter with the device ID is returned, or null for an unknown device ID.
        /// </summary>
        /// <param name="deviceId">The device ID of the requested external power meter.</param>
        /// <param name="isFound">True if the DER has an external power meter with the device ID.</param>
        [TestMethod]
        [DataRow(21, true)]
        [DataRow(22, true)]
        [DataRow(5, false)]
        public void GetExternalPowerMeterReturnsMeterWithDeviceIdTest(int deviceId, bool isFound)
        {
            List<PmExternalDto> meters = new List<PmExternalDto>
            {
                new PmExternalDto(new PmExternalDefinitionDto { DeviceId = 21, Name = "ExtPm21", TargetAddress = "localhost", TargetPort = 502 }),
                new PmExternalDto(new PmExternalDefinitionDto { DeviceId = 22, Name = "ExtPm22", TargetAddress = "localhost", TargetPort = 502 })
            };
            DerDto der = CreateDer(new List<PmSystemDto>(), new List<BbDto>()) with { ExternalPowerMeters = meters };
            _deviceClient.Setup(client => client.Der).Returns(der);

            PmExternalDto? meter = _manager.GetExternalPowerMeter(deviceId);

            Assert.AreEqual(isFound, meter is not null);
            Assert.AreSame(meters.FirstOrDefault(expected => expected.DeviceId == deviceId), meter);
        }


        /// <summary>
        /// Confirms the PCS of a battery storage or solar unit with the device ID is returned, or null for an unknown device ID.
        /// </summary>
        /// <param name="deviceId">The device ID of the requested PCS.</param>
        /// <param name="expectedName">The name of the expected PCS, null if no PCS has the device ID.</param>
        [TestMethod]
        [DataRow(31, "BatteryPcs31")]
        [DataRow(32, "SolarPcs32")]
        [DataRow(5, null)]
        public void GetPcsReturnsPcsOfDerUnitWithDeviceIdTest(int deviceId, string? expectedName)
        {
            PcsDto batteryPcs = new PcsDto(new PcsDefinitionDto { DeviceId = 31, Name = "BatteryPcs31", TargetAddress = "localhost", TargetPort = 502 });
            PcsDto solarPcs = new PcsDto(new PcsDefinitionDto { DeviceId = 32, Name = "SolarPcs32", TargetAddress = "localhost", TargetPort = 502 });
            DerBatteryStorageUnitDto batteryUnit = new DerBatteryStorageUnitDto { Name = "BatteryUnit1", PowerConversionSystem = batteryPcs };
            DerSolarUnitDto solarUnit = new DerSolarUnitDto { Name = "SolarUnit1", PowerConversionSystem = solarPcs };
            DerCircuitDto circuit = new DerCircuitDto { Name = "Circuit1", DerUnits = new List<DerUnitDto> { batteryUnit, solarUnit } };
            DerGroupDto group = new DerGroupDto { Name = "Group1", DerCircuits = new List<DerCircuitDto> { circuit } };
            _deviceClient.Setup(client => client.Der).Returns(new DerDto { Name = "Der", DerGroups = new List<DerGroupDto> { group } });

            PcsDto? pcs = _manager.GetPcs(deviceId);

            Assert.AreEqual(expectedName, pcs?.Name);
        }


        /// <summary>
        /// Confirms the battery bank of a battery storage unit with the device ID is returned, or null for an unknown device ID.
        /// </summary>
        /// <param name="deviceId">The device ID of the requested battery bank.</param>
        /// <param name="isFound">True if the DER has a battery bank with the device ID.</param>
        [TestMethod]
        [DataRow(41, true)]
        [DataRow(42, true)]
        [DataRow(5, false)]
        public void GetBatteryBankReturnsBatteryBankWithDeviceIdTest(int deviceId, bool isFound)
        {
            List<BbDto> batteryBanks = new List<BbDto>
            {
                CreateBatteryBank(41, false, 80),
                CreateBatteryBank(42, true, null)
            };
            _deviceClient.Setup(client => client.Der).Returns(CreateDer(new List<PmSystemDto>(), batteryBanks));

            BbDto? batteryBank = _manager.GetBatteryBank(deviceId);

            Assert.AreEqual(isFound, batteryBank is not null);
            Assert.AreSame(batteryBanks.FirstOrDefault(expected => expected.DeviceId == deviceId), batteryBank);
        }


        /// <summary>
        /// Creates a DER with the system power meters and one battery storage unit with the battery banks.
        /// </summary>
        /// <param name="systemMeters">The system power meters.</param>
        /// <param name="batteryBanks">The battery banks of the battery storage unit.</param>
        /// <returns>The DER DTO.</returns>
        private DerDto CreateDer(List<PmSystemDto> systemMeters, List<BbDto> batteryBanks)
        {
            PcsDto pcs = new PcsDto(new PcsDefinitionDto { DeviceId = 1, Name = "PCS1", TargetAddress = "localhost", TargetPort = 502 });
            DerBatteryStorageUnitDto unit = new DerBatteryStorageUnitDto { Name = "Unit1", PowerConversionSystem = pcs, BatteryBanks = batteryBanks };
            DerCircuitDto circuit = new DerCircuitDto { Name = "Circuit1", DerUnits = new List<DerUnitDto> { unit } };
            DerGroupDto group = new DerGroupDto { Name = "Group1", DerCircuits = new List<DerCircuitDto> { circuit } };

            return new DerDto { Name = "Der", DerGroups = new List<DerGroupDto> { group }, SystemPowerMeters = systemMeters };
        }


        /// <summary>
        /// Creates a system power meter with a core.
        /// </summary>
        /// <param name="deviceId">The device ID.</param>
        /// <param name="communicationError">The communication error.</param>
        /// <param name="state">The state.</param>
        /// <param name="activePowerWatts">The active power in watts, the reactive power is the negative half of it.</param>
        /// <returns>The system power meter DTO.</returns>
        private PmSystemDto CreateSystemMeter(int deviceId, bool communicationError, PowerMeterState state, double activePowerWatts)
        {
            PmSystemDto meter = new PmSystemDto(CreateSystemMeterDefinition(deviceId));
            meter.UpdateCore(new PmSystemCoreDto
            {
                DeviceId = deviceId,
                CommunicationError = communicationError,
                State = state,
                ActivePower = new ActivePower(activePowerWatts),
                ReactivePower = new ReactivePower(-activePowerWatts / 2),
                PowerFactor = -0.89,
                Frequency = 50.01
            });

            return meter;
        }


        /// <summary>
        /// Creates the definition of a system power meter.
        /// </summary>
        /// <param name="deviceId">The device ID.</param>
        /// <returns>The system power meter definition DTO.</returns>
        private PmSystemDefinitionDto CreateSystemMeterDefinition(int deviceId)
        {
            return new PmSystemDefinitionDto { DeviceId = deviceId, Name = $"SysPm{deviceId}", TargetAddress = "localhost", TargetPort = 502 };
        }


        /// <summary>
        /// Creates a battery bank with a core.
        /// </summary>
        /// <param name="deviceId">The device ID.</param>
        /// <param name="communicationError">The communication error.</param>
        /// <param name="stateOfCharge">The state of charge in percent.</param>
        /// <returns>The battery bank DTO.</returns>
        private BbDto CreateBatteryBank(int deviceId, bool communicationError, double? stateOfCharge)
        {
            BbDto batteryBank = new BbDto(CreateBatteryBankDefinition(deviceId));
            batteryBank.UpdateCore(new BbCoreDto { DeviceId = deviceId, CommunicationError = communicationError, StateOfCharge = stateOfCharge });

            return batteryBank;
        }


        /// <summary>
        /// Creates the definition of a battery bank.
        /// </summary>
        /// <param name="deviceId">The device ID.</param>
        /// <returns>The battery bank definition DTO.</returns>
        private BbDefinitionDto CreateBatteryBankDefinition(int deviceId)
        {
            return new BbDefinitionDto { DeviceId = deviceId, Name = $"BB{deviceId}", TargetAddress = "localhost", TargetPort = 502 };
        }
    }
}

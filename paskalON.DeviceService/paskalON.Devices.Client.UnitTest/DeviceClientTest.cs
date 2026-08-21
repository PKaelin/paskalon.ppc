// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Devices.Application;
using paskalON.Devices.Client.Subscribers;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.Messaging;

namespace paskalON.Devices.Client.UnitTest
{
    [TestClass]
    public class DeviceClientTest
    {
        private Mock<IMessageSubscriber> _subscriberMock = null!;
        private Mock<IDeviceServer> _deviceServerMock = null!;
        private SubscriberTopic? _subscriberTopic = null!;


        [TestInitialize]
        public void Initialize()
        {
            _subscriberMock = new Mock<IMessageSubscriber>();
            _deviceServerMock = new Mock<IDeviceServer>();
            _subscriberTopic = new SubscriberTopic();
        }


        [TestMethod]
        public void DeviceClientConstructorNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                { new DeviceClient(null!, _subscriberMock.Object, _deviceServerMock.Object, _subscriberTopic!); });
        }


        [TestMethod]
        public void DeviceClientConstructorNullSubscriberTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                { new DeviceClient(NullLogger.Instance, null!, _deviceServerMock.Object, _subscriberTopic!); });
        }


        [TestMethod]
        public void DeviceClientConstructorNullDeviceServerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                { new DeviceClient(NullLogger.Instance, _subscriberMock.Object, null!, _subscriberTopic!); });
        }


        [TestMethod]
        public void DeviceClientConstructorNullSubscriberTopicTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                { new DeviceClient(NullLogger.Instance, _subscriberMock.Object, _deviceServerMock.Object, null!); });
        }


        [TestMethod]
        public void DeviceClientConstructorTest()
        {
            DeviceClient client = new DeviceClient(NullLogger.Instance, _subscriberMock.Object, _deviceServerMock.Object, _subscriberTopic!);

            Assert.IsNotNull(client);
            Assert.IsNotNull(client.Der);
            Assert.IsNotNull(client.PowerConversionSystems);
            Assert.IsNotNull(client.BatteryBanks);
            Assert.IsNotNull(client.SolarPanels);
            Assert.IsNotNull(client.ExternalPowerMeters);
            Assert.IsNotNull(client.AuxiliaryPowerMeters);
            Assert.IsNotNull(client.CircuitPowerMeters);
        }


        [TestMethod]
        public async Task DeviceClientInitializeEmptyDerTest()
        {
            DerDto der = new DerDto();
            _deviceServerMock.Setup(x => x.GetDer()).ReturnsAsync(der);
            DeviceClient client = new DeviceClient(NullLogger.Instance, _subscriberMock.Object, _deviceServerMock.Object, _subscriberTopic!);

            await client.Initialize();

            _deviceServerMock.Verify(x => x.GetDer(), Times.Once);
            Assert.AreSame(der, client.Der);
            Assert.HasCount(0, client.PowerConversionSystems);
            Assert.HasCount(0, client.BatteryBanks);
            Assert.HasCount(0, client.SolarPanels);
            Assert.HasCount(0, client.ExternalPowerMeters);
            Assert.HasCount(0, client.AuxiliaryPowerMeters);
            Assert.HasCount(0, client.CircuitPowerMeters);
        }


        [TestMethod]
        public async Task DeviceClientInitializeDeviceServerThrowsExceptionTest()
        {
            _deviceServerMock.Setup(x => x.GetDer()).ThrowsAsync(new InvalidOperationException("Test exception"));
            DeviceClient client = new DeviceClient(NullLogger.Instance, _subscriberMock.Object, _deviceServerMock.Object, _subscriberTopic!);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => { await client.Initialize(); });
        }


        [TestMethod]
        public async Task DeviceClientInitializeSubscribesToConfiguredTopicsTest()
        {
            DerDto der = new DerDto();
            _deviceServerMock.Setup(x => x.GetDer()).ReturnsAsync(der);
            _subscriberTopic = CreateSubscriberTopicWithTopics();
            DeviceClient client = new DeviceClient(NullLogger.Instance, _subscriberMock.Object, _deviceServerMock.Object, _subscriberTopic!);

            await client.Initialize();

            _subscriberMock.Verify(x => x.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.AtLeastOnce);
        }


        [TestMethod]
        public async Task DeviceClientInitializeWithoutConfiguredTopicsTest()
        {
            DerDto der = new DerDto();
            _deviceServerMock.Setup(x => x.GetDer()).ReturnsAsync(der);
            DeviceClient client = new DeviceClient(NullLogger.Instance, _subscriberMock.Object, _deviceServerMock.Object, _subscriberTopic!);

            await client.Initialize();

            _subscriberMock.Verify(x => x.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
        }


        [TestMethod]
        public async Task DeviceClientInitializeDerBatteriesTest()
        {
            DerDto der = CreateBatteryDevicesDto();
            _deviceServerMock.Setup(x => x.GetDer()).ReturnsAsync(der);
            DeviceClient client = new DeviceClient(NullLogger.Instance, _subscriberMock.Object, _deviceServerMock.Object, _subscriberTopic!);

            await client.Initialize();

            _deviceServerMock.Verify(x => x.GetDer(), Times.Once);
            Assert.AreSame(der, client.Der);
            Assert.HasCount(1, der.DerGroups);
            Assert.HasCount(1, der.DerGroups.First().DerCircuits);
            Assert.HasCount(1, der.DerGroups.First().DerCircuits.First().DerUnits);
            Assert.HasCount(1, client.PowerConversionSystems);
            Assert.HasCount(2, client.BatteryBanks);
            Assert.HasCount(0, client.SolarPanels);
            Assert.HasCount(0, client.SystemPowerMeters);
            Assert.HasCount(0, client.ExternalPowerMeters);
            Assert.HasCount(0, client.AuxiliaryPowerMeters);
            Assert.HasCount(0, client.CircuitPowerMeters);
        }


        [TestMethod]
        public async Task DeviceClientInitializeDerSolarsTest()
        {
            DerDto der = CreateSolarDevicesDto();
            _deviceServerMock.Setup(x => x.GetDer()).ReturnsAsync(der);
            DeviceClient client = new DeviceClient(NullLogger.Instance, _subscriberMock.Object, _deviceServerMock.Object, _subscriberTopic!);

            await client.Initialize();

            _deviceServerMock.Verify(x => x.GetDer(), Times.Once);
            Assert.AreSame(der, client.Der);
            Assert.HasCount(1, der.DerGroups);
            Assert.HasCount(1, der.DerGroups.First().DerCircuits);
            Assert.HasCount(2, der.DerGroups.First().DerCircuits.First().DerUnits);
            Assert.HasCount(2, client.PowerConversionSystems);
            Assert.HasCount(0, client.BatteryBanks);
            Assert.HasCount(3, client.SolarPanels);
            Assert.HasCount(0, client.SystemPowerMeters);
            Assert.HasCount(0, client.ExternalPowerMeters);
            Assert.HasCount(0, client.AuxiliaryPowerMeters);
            Assert.HasCount(0, client.CircuitPowerMeters);
        }


        [TestMethod]
        public async Task DeviceClientInitializeMeterTest()
        {
            DerDto der = CreateMeterDevicesDto();
            _deviceServerMock.Setup(x => x.GetDer()).ReturnsAsync(der);
            DeviceClient client = new DeviceClient(NullLogger.Instance, _subscriberMock.Object, _deviceServerMock.Object, _subscriberTopic!);

            await client.Initialize();

            _deviceServerMock.Verify(x => x.GetDer(), Times.Once);
            Assert.AreSame(der, client.Der);
            Assert.HasCount(1, der.DerGroups);
            Assert.HasCount(1, der.DerGroups.First().DerCircuits);
            Assert.IsNotNull(der.DerGroups.First().DerCircuits.First().CircuitPowerMeter);
            Assert.HasCount(0, client.PowerConversionSystems);
            Assert.HasCount(0, client.BatteryBanks);
            Assert.HasCount(0, client.SolarPanels);
            Assert.HasCount(1, client.SystemPowerMeters);
            Assert.HasCount(2, client.ExternalPowerMeters);
            Assert.HasCount(1, client.AuxiliaryPowerMeters);
            Assert.HasCount(1, client.CircuitPowerMeters);
        }


        private DerDto CreateBatteryDevicesDto()
        {
            // Devices
            BbDefinitionDto bbDefinition1 = new BbDefinitionDto { DeviceId = 1, Name = "BB1" };
            BbDto bb1 = new BbDto(bbDefinition1);
            BbDefinitionDto bbDefinition2 = new BbDefinitionDto { DeviceId = 2, Name = "BB2" };
            BbDto bb2 = new BbDto(bbDefinition2);
            PcsDefinitionDto pcsDef1 = new PcsDefinitionDto { DeviceId = 1, Name = "PCS1" };
            PcsDto pcs1 = new PcsDto(pcsDef1);
            // Structure
            DerBatteryStorageUnitDto unit1 = new DerBatteryStorageUnitDto { PowerConversionSystem = pcs1, BatteryBanks = new List<BbDto> { bb1, bb2 } };
            DerCircuitDto circuit1 = new DerCircuitDto { DerUnits = new List<DerUnitDto> { unit1 } };
            DerGroupDto group1 = new DerGroupDto { DerCircuits = new List<DerCircuitDto> { circuit1 } };
            DerDto der = new DerDto { DerGroups = new List<DerGroupDto> { group1 } };

            return der;
        }


        private DerDto CreateSolarDevicesDto()
        {
            // Devices
            PvDefinitionDto pvDef1 = new PvDefinitionDto { DeviceId = 1, Name = "PV1" };
            PvDto pv1 = new PvDto(pvDef1);
            PvDefinitionDto pvDef2 = new PvDefinitionDto { DeviceId = 2, Name = "PV2" };
            PvDto pv2 = new PvDto(pvDef2);
            PvDefinitionDto pvDef3 = new PvDefinitionDto { DeviceId = 3, Name = "PV3" };
            PvDto pv3 = new PvDto(pvDef3);
            PcsDefinitionDto pcsDef1 = new PcsDefinitionDto { DeviceId = 1, Name = "PCS1" };
            PcsDto pcs1 = new PcsDto(pcsDef1);
            PcsDefinitionDto pcsDef2 = new PcsDefinitionDto { DeviceId = 2, Name = "PCS2" };
            PcsDto pcs2 = new PcsDto(pcsDef2);
            // Structure
            DerSolarUnitDto unit1 = new DerSolarUnitDto { PowerConversionSystem = pcs1, SolarPanels = new List<PvDto> { pv1, pv2 } };
            DerSolarUnitDto unit2 = new DerSolarUnitDto { PowerConversionSystem = pcs2, SolarPanels = new List<PvDto> { pv3 } };
            DerCircuitDto circuit1 = new DerCircuitDto { DerUnits = new List<DerUnitDto> { unit1, unit2 } };
            DerGroupDto group1 = new DerGroupDto { DerCircuits = new List<DerCircuitDto> { circuit1 } };
            DerDto der = new DerDto { DerGroups = new List<DerGroupDto> { group1 } };

            return der;
        }


        private DerDto CreateMeterDevicesDto()
        {
            // Devices
            PmSystemDefinitionDto pmSysDef1 = new PmSystemDefinitionDto { DeviceId = 1, Name = "SysPm1" };
            PmSystemDto pmSys1 = new PmSystemDto(pmSysDef1);
            PmExternalDefinitionDto pmExDef1 = new PmExternalDefinitionDto { DeviceId = 1, Name = "ExPm1" };
            PmExternalDto pmEx1 = new PmExternalDto(pmExDef1);
            PmExternalDefinitionDto pmExDef2 = new PmExternalDefinitionDto { DeviceId = 2, Name = "ExPm2" };
            PmExternalDto pmEx2 = new PmExternalDto(pmExDef2);
            PmAuxiliaryDefinitionDto pmAuxDef1 = new PmAuxiliaryDefinitionDto { DeviceId = 1, Name = "AuxPm1" };
            PmAuxiliaryDto pmAux1 = new PmAuxiliaryDto(pmAuxDef1);
            PmCircuitDefinitionDto pmCircDef1 = new PmCircuitDefinitionDto { DeviceId = 1, Name = "CircPm1" };
            PmCircuitDto pmCirc1 = new PmCircuitDto(pmCircDef1);
            // Structure
            DerCircuitDto circuit1 = new DerCircuitDto { CircuitPowerMeter = pmCirc1 };
            DerGroupDto group1 = new DerGroupDto { DerCircuits = new List<DerCircuitDto> { circuit1 } };
            DerDto der = new DerDto
            {
                DerGroups = new List<DerGroupDto> { group1 },
                SystemPowerMeters = new List<PmSystemDto> { pmSys1 },
                ExternalPowerMeters = new List<PmExternalDto> { pmEx1, pmEx2 },
                AuxiliaryPowerMeters = new List<PmAuxiliaryDto> { pmAux1 },
            };

            return der;
        }


        private SubscriberTopic CreateSubscriberTopicWithTopics()
        {
            SubscriberTopic topic = new SubscriberTopic();

            topic.PowerConversionSystemTopic = new SubscriberTopicEntry
            {
                DefinitionTopic = "pcs/definition",
                CoreTopic = "pcs/core",
                DetailTopic = "pcs/detail"
            };

            topic.BatteryBankTopic = new SubscriberTopicEntry
            {
                DefinitionTopic = "bb/definition",
                CoreTopic = "bb/core",
                DetailTopic = "bb/detail"
            };

            topic.SolarPanelTopic = new SubscriberTopicEntry
            {
                DefinitionTopic = "pv/definition",
                CoreTopic = "pv/core",
                DetailTopic = "pv/detail"
            };

            return topic;
        }

    }
}

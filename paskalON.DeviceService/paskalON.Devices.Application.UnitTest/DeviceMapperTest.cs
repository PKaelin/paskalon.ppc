// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;

namespace paskalON.Devices.Application.UnitTest
{
    [TestClass]
    public class DeviceMapperTest
    {
        private readonly DeviceMapper _mapper = new DeviceMapper();


        [TestMethod]
        public void DeviceMapperMapDerEmptyTest()
        {
            DerConfig config = new DerConfig { ChangedBy = "Test", Name = "DER" };
            Der der = new Der(NullLogger.Instance, config);

            DerDto result = _mapper.MapDer(der);

            Assert.IsNotNull(result);
            Assert.HasCount(0, result.DerGroups);
            Assert.HasCount(0, result.SystemPowerMeters);
            Assert.HasCount(0, result.AuxiliaryPowerMeters);
            Assert.HasCount(0, result.ExternalPowerMeters);
        }


        [TestMethod]
        public void DeviceMapperMapDerNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapDer(null!));
        }


        [TestMethod]
        public void DeviceMapperMapDerGroupTest()
        {
            Der der = new Der(NullLogger.Instance, new DerConfig { ChangedBy = "Test", Name = "DER" });
            DerGroupConfig config = new DerGroupConfig { ChangedBy = "Test", Name = "Group", DerConfig = new DerConfig { ChangedBy = "Test", Name = "DER" } };
            DerGroup group = new DerGroup(NullLogger.Instance, config, der);

            DerGroupDto result = _mapper.MapDerGroup(group);

            Assert.IsNotNull(result);
            Assert.HasCount(0, result.DerCircuits);
        }


        [TestMethod]
        public void DeviceMapperMapDerGroupNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapDerGroup(null!));
        }


        [TestMethod]
        public void DeviceMapperMapDerCircuitTest()
        {
            Der der = new Der(NullLogger.Instance, new DerConfig { ChangedBy = "Test", Name = "DER" });
            DerGroupConfig groupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "Group", DerConfig = new DerConfig { ChangedBy = "Test", Name = "DER" } };
            DerGroup group = new DerGroup(NullLogger.Instance, groupConfig, der);
            DerCircuitConfig config = new DerCircuitConfig { ChangedBy = "Test", Name = "Circuit", DerGroupConfig = groupConfig };
            DerCircuit circuit = new DerCircuit(NullLogger.Instance, config, group);

            DerCircuitDto result = _mapper.MapDerCircuit(circuit);

            Assert.IsNotNull(result);
            Assert.HasCount(0, result.DerUnits);
            Assert.IsNull(result.CircuitPowerMeter);
        }


        [TestMethod]
        public void DeviceMapperMapDerCircuitNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapDerCircuit(null!));
        }


        [TestMethod]
        public void DeviceMapperMapDerUnitNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapDerUnit(null!));
        }


        [TestMethod]
        public void DeviceMapperMapDerBatteryStorageUnitNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapDerBatteryStorageUnit(null!));
        }


        [TestMethod]
        public void DeviceMapperMapDerSolarUnitNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapDerSolarUnit(null!));
        }


        [TestMethod]
        public void DeviceMapperMapPowerConversionSystemNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapPowerConversionSystem(null!));
        }


        [TestMethod]
        public void DeviceMapperMapPowerConversionSystemDefinitionNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapPowerConversionSystemDefinition(null!));
        }


        [TestMethod]
        public void DeviceMapperMapPowerConversionSystemCoreNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapPowerConversionSystemCore(null!));
        }


        [TestMethod]
        public void DeviceMapperMapPowerConversionSystemDetailNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapPowerConversionSystemDetail(null!));
        }


        [TestMethod]
        public void DeviceMapperMapBatteryBankNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapBatteryBank(null!));
        }


        [TestMethod]
        public void DeviceMapperMapBatteryBankDefinitionNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapBatteryBankDefinition(null!));
        }


        [TestMethod]
        public void DeviceMapperMapBatteryBankCoreNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapBatteryBankCore(null!));
        }


        [TestMethod]
        public void DeviceMapperMapBatteryBankDetailNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapBatteryBankDetail(null!));
        }


        [TestMethod]
        public void DeviceMapperMapSolarPanelNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapSolarPanel(null!));
        }


        [TestMethod]
        public void DeviceMapperMapSolarPanelDefinitionNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapSolarPanelDefinition(null!));
        }


        [TestMethod]
        public void DeviceMapperMapSolarPanelCoreNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapSolarPanelCore(null!));
        }


        [TestMethod]
        public void DeviceMapperMapSolarPanelDetailNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapSolarPanelDetail(null!));
        }


        [TestMethod]
        public void DeviceMapperMapSystemPowerMeterNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapSystemPowerMeter(null!));
        }


        [TestMethod]
        public void DeviceMapperMapAuxiliaryPowerMeterNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapAuxiliaryPowerMeter(null!));
        }


        [TestMethod]
        public void DeviceMapperMapExternalPowerMeterNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapExternalPowerMeter(null!));
        }


        [TestMethod]
        public void DeviceMapperMapCircuitPowerMeterNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapCircuitPowerMeter(null!));
        }


        [TestMethod]
        public void DeviceMapperMapPowerMeterDefinitionNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapPowerMeterDefinition<PmSystemDefinitionDto>(null!));
        }


        [TestMethod]
        public void DeviceMapperMapPowerMeterCoreNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapPowerMeterCore<PmSystemCoreDto>(null!));
        }


        [TestMethod]
        public void DeviceMapperMapPowerMeterDetailNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mapper.MapPowerMeterDetail<PmSystemDetailDto>(null!));
        }
    }
}

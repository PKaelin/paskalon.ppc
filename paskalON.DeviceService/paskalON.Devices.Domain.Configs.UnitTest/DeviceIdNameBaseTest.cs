// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.GenericModbusDevices;

namespace paskalON.Devices.Domain.Configs.UnitTest
{
    [TestClass]
    public class DeviceIdNameBaseTest
    {
        public static IEnumerable<object[]> Configs => TestConfigFactory.CreateDeviceIdNameBaseConfigs();


        [TestMethod]
        [DynamicData(nameof(Configs))]
        public void DeviceIdBoundaryTest(DeviceIdNameBase config)
        {
            config.DeviceId = 0;
            int lowest = config.DeviceId;

            config.DeviceId = int.MaxValue;

            Assert.AreEqual(0, lowest);
            Assert.AreEqual(int.MaxValue, config.DeviceId);
        }


        [TestMethod]
        [DynamicData(nameof(Configs))]
        public void DeviceIdNegativeTest(DeviceIdNameBase config)
        {
            Action set = () => config.DeviceId = -1;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
            Assert.AreEqual(1, config.DeviceId);
        }


        [TestMethod]
        [DynamicData(nameof(Configs))]
        public void MetricsFactorNotPositiveTest(DeviceIdNameBase config)
        {
            int[] defaults = [config.MetricsFactorClass1, config.MetricsFactorClass2, config.MetricsFactorClass3, config.MetricsFactorClass4];

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MetricsFactorClass1 = 0);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MetricsFactorClass2 = 0);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MetricsFactorClass3 = -1);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => config.MetricsFactorClass4 = int.MinValue);
            CollectionAssert.AreEqual(defaults, new int[] { config.MetricsFactorClass1, config.MetricsFactorClass2, config.MetricsFactorClass3, config.MetricsFactorClass4 });
        }


        [TestMethod]
        [DynamicData(nameof(Configs))]
        public void MetricsFactorMinimumTest(DeviceIdNameBase config)
        {
            config.MetricsFactorClass4 = 1;

            Assert.AreEqual(1, config.MetricsFactorClass4);
        }



        [TestMethod]
        public void GenericModbusDeviceIdNegativeTest()
        {
            ModbusConfig modbus = TestConfigFactory.CreateModbusConfig();
            GenericModbusConfig config = new GenericModbusConfig
            {
                ChangedBy = TestConfigFactory.ChangedBy,
                Name = "GMD 1",
                DeviceId = 1,
                IsActive = true,
                ModbusConnectionConfig = modbus.ModbusConnectionConfig,
                Address = modbus.Address,
                Port = Constants.PortStartGmd,
                AddressFamily = modbus.AddressFamily,
                UnitId = 1,
                DerConfig = TestConfigFactory.CreateDerConfig(),
                GenericModbusDeviceConfig = new GenericModbusDeviceConfig
                {
                    ChangedBy = TestConfigFactory.ChangedBy,
                    Name = "Device GMD 1",
                    ClassName = "GenericModbusDevice",
                    GenericModbusMapConfig = TestConfigFactory.CreateGenericModbusMapConfig()
                }
            };

            Action set = () => config.DeviceId = -1;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
            Assert.AreEqual(1, config.DeviceId);
        }
    }
}

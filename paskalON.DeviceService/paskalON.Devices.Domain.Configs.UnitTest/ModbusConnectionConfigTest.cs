// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Configs.UnitTest
{
    [TestClass]
    public class ModbusConnectionConfigTest
    {
        private const string Factor1 = nameof(ModbusConnectionConfig.PollingFactorClass1);
        private const string Factor2 = nameof(ModbusConnectionConfig.PollingFactorClass2);
        private const string Factor3 = nameof(ModbusConnectionConfig.PollingFactorClass3);
        private const string Factor4 = nameof(ModbusConnectionConfig.PollingFactorClass4);
        private const string Factor5 = nameof(ModbusConnectionConfig.PollingFactorClass5);
        private const string ConnectionTimeout = nameof(ModbusConnectionConfig.ConnectionTimeoutMilliseconds);
        private const string DisconnectionTimeout = nameof(ModbusConnectionConfig.DisconnectionTimeoutMilliseconds);
        private const string ConnectRetryCount = nameof(ModbusConnectionConfig.ConnectRetryCount);
        private const string ConnectRetryInterval = nameof(ModbusConnectionConfig.ConnectRetryIntervalMilliseconds);
        private const string OperationTimeout = nameof(ModbusConnectionConfig.OperationTimeoutMilliseconds);
        private const string SendRetryCount = nameof(ModbusConnectionConfig.SendRetryCount);
        private const string SendRetryInterval = nameof(ModbusConnectionConfig.SendRetryIntervalMilliseconds);


        [TestMethod]
        [DataRow(Factor1, 1)]
        [DataRow(Factor2, 1)]
        [DataRow(Factor3, 1)]
        [DataRow(Factor4, 1)]
        [DataRow(Factor5, 1)]
        [DataRow(ConnectionTimeout, 0)]
        [DataRow(DisconnectionTimeout, 0)]
        [DataRow(ConnectRetryCount, 0)]
        [DataRow(ConnectRetryInterval, 0)]
        [DataRow(OperationTimeout, 0)]
        [DataRow(SendRetryCount, 0)]
        [DataRow(SendRetryInterval, 0)]
        [DataRow(Factor5, int.MaxValue)]
        [DataRow(OperationTimeout, int.MaxValue)]
        public void SetAtLowerLimitTest(string propertyName, int value)
        {
            ModbusConnectionConfig config = CreateConfig();

            PropertyAccessor.Set(config, propertyName, value);

            Assert.AreEqual(value, PropertyAccessor.Get(config, propertyName));
        }


        [TestMethod]
        [DataRow(Factor1, 0)]
        [DataRow(Factor2, 0)]
        [DataRow(Factor3, 0)]
        [DataRow(Factor4, 0)]
        [DataRow(Factor5, 0)]
        [DataRow(Factor1, -1)]
        [DataRow(ConnectionTimeout, -1)]
        [DataRow(DisconnectionTimeout, -1)]
        [DataRow(ConnectRetryCount, -1)]
        [DataRow(ConnectRetryInterval, -1)]
        [DataRow(OperationTimeout, -1)]
        [DataRow(SendRetryCount, -1)]
        [DataRow(SendRetryInterval, int.MinValue)]
        public void SetBelowLowerLimitTest(string propertyName, int value)
        {
            ModbusConnectionConfig config = CreateConfig();
            object? original = PropertyAccessor.Get(config, propertyName);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PropertyAccessor.Set(config, propertyName, value));

            Assert.AreEqual(original, PropertyAccessor.Get(config, propertyName));
        }


        private ModbusConnectionConfig CreateConfig()
        {
            return new ModbusConnectionConfig { ChangedBy = TestConfigFactory.ChangedBy, Name = "Connection 1" };
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Configs;
using paskalON.Protocols.Modbus.NModbus;

namespace paskalON.Protocols.Modbus.UnitTest.NModbus
{
    [TestClass]
    public sealed class NModbusClientTest
    {
        [TestMethod]
        public void NModbusClientConstructorNullLoggerTest()
        {
            ClientConnectionConfig configuration = CreateConfiguration();
            Assert.ThrowsExactly<ArgumentNullException>(() => new NModbusClient(null!, configuration, 1));
        }


        [TestMethod]
        public void NModbusClientConstructorNullConfigurationTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new NModbusClient(NullLogger<NModbusClient>.Instance, null!, 1));
        }



        [TestMethod]
        public void NModbusClientConstructorSetsConnectionPropertiesTest()
        {
            ClientConnectionConfig configuration = CreateConfiguration();
            NModbusClient client = new NModbusClient(NullLogger<NModbusClient>.Instance, configuration, 7);

            Assert.AreEqual(configuration.ServerAddress, client.ServerAddress);
            Assert.AreEqual(configuration.ServerPort, client.ServerPort);
            Assert.AreEqual((byte)7, client.UnitId);
            Assert.AreEqual(ModbusClientState.Disconnected, client.State);
        }


        [TestMethod]
        public async Task NModbusClientConnectAsyncWithInvalidEndpointFaultsAndRaisesCommunicationErrorTest()
        {
            ClientConnectionConfig configuration = CreateConfiguration() with
            {
                ServerAddress = "127.0.0.1",
                ServerPort = 1,
                ConnectionTimeoutMilliseconds = 100,
                ConnectRetryCount = 0
            };
            NModbusClient client = new NModbusClient(NullLogger<NModbusClient>.Instance, configuration);
            int communicationErrors = 0;
            client.OnCommunicationError += (_, _) => communicationErrors++;

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.ConnectAsync());

            Assert.AreEqual(1, communicationErrors);
            await client.DisposeAsync();
        }


        [TestMethod]
        public async Task NModbusClientReadHoldingRegistersAsyncWhenDispatcherNotRunningTest()
        {
            await using NModbusClient client = new NModbusClient(NullLogger<NModbusClient>.Instance, CreateConfiguration());

            Task<ushort[]> result = client.ReadHoldingRegistersAsync(10, 11);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await result);
        }


        [TestMethod]
        public void NModbusClientConvertRawDataUsesRegisterEntryValuesTest()
        {
            Mock<IModbusRegisterEntry> register = new Mock<IModbusRegisterEntry>();
            register.SetupGet(x => x.Register).Returns(10);
            register.SetupGet(x => x.Scale).Returns(1);
            register.SetupGet(x => x.DataType).Returns(ModbusDataType.MbUint16);
            NModbusClient client = new NModbusClient(NullLogger<NModbusClient>.Instance, CreateConfiguration());

            object? result = client.ConvertRawData(new ushort[] { 4321 }, register.Object, 10);

            Assert.AreEqual((ushort)4321, result);
        }


        [TestMethod]
        public void NModbusClientConvertRawBooleanDataUsesRegisterEntryValuesTest()
        {
            Mock<IModbusRegisterEntry> register = new Mock<IModbusRegisterEntry>();
            register.SetupGet(x => x.Register).Returns(10);
            register.SetupGet(x => x.Scale).Returns(1);
            register.SetupGet(x => x.DataType).Returns(ModbusDataType.MbBool);
            NModbusClient client = new NModbusClient(NullLogger<NModbusClient>.Instance, CreateConfiguration());

            bool result = client.ConvertRawData(new[] { true }, register.Object, 10);

            Assert.IsTrue(result);
        }


        private ClientConnectionConfig CreateConfiguration()
        {
            return new ClientConnectionConfig
            {
                ServerAddress = "localhost",
                ServerPort = 502,
                ConnectionTimeoutMilliseconds = 100,
                DisconnectionTimeoutMilliseconds = 100,
                ConnectRetryCount = 0,
                ConnectRetryIntervalMilliseconds = 1,
                OperationTimeoutMilliseconds = 100,
                SendRetryCount = 0,
                SendRetryIntervalMilliseconds = 1
            };
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.Modbus;
using paskalON.Protocols.Modbus.Configs;
using paskalON.Protocols.Modbus.NModbus;
using System.Net.Sockets;
using System.Reflection;

namespace paskalON.Devices.Application.UnitTest.Factories
{
    [TestClass]
    public class ModbusDeviceFactoryTest
    {
        private readonly List<IModbusClient> _createdClients = new List<IModbusClient>();


        [TestCleanup]
        public async Task Cleanup()
        {
            foreach (IModbusClient client in _createdClients.Distinct())
            {
                if (client is IAsyncDisposable disposable)
                {
                    await disposable.DisposeAsync();
                }
            }
        }


        [TestMethod]
        public void ModbusDeviceFactoryConstructorNullServicesTest()
        {
            IServiceProvider services = null!;

            Assert.ThrowsExactly<ArgumentNullException>(() => new ModbusDeviceFactory(services));
        }


        [TestMethod]
        public void ModbusDeviceFactoryCreateNullConfigTest()
        {
            ModbusDeviceFactory factory = new ModbusDeviceFactory(CreateServiceProviderMock().Object);

            Assert.ThrowsExactly<ArgumentNullException>(() => factory.Create(null!));
        }


        [TestMethod]
        public void ModbusDeviceFactoryCreateMissingLoggerRegistrationTest()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            ModbusDeviceFactory factory = new ModbusDeviceFactory(servicesMock.Object);
            ModbusConfig config = CreateConfig("Inverter-1", "192.168.1.20", 502, 1);

            Assert.ThrowsExactly<InvalidOperationException>(() => factory.Create(config));
        }


        [TestMethod]
        public void ModbusDeviceFactoryCreateReturnsDatafaceAndClientTest()
        {
            ModbusDeviceFactory factory = new ModbusDeviceFactory(CreateServiceProviderMock().Object);
            ModbusConfig config = CreateConfig("Inverter-1", "192.168.1.20", 502, 1);

            (IModbusDataface dataface, IModbusClient client) = factory.Create(config);
            _createdClients.Add(client);

            Assert.IsInstanceOfType<ModbusRegister>(dataface);
            Assert.AreEqual("Inverter-1", dataface.Name);
            Assert.IsInstanceOfType<NModbusClient>(client);
        }


        [TestMethod]
        public void ModbusDeviceFactoryCreateMapsConnectionConfigTest()
        {
            ModbusDeviceFactory factory = new ModbusDeviceFactory(CreateServiceProviderMock().Object);
            ModbusConfig config = CreateConfig("Inverter-1", "fe80::20", 1502, 17, AddressFamily.InterNetworkV6);
            config.ModbusConnectionConfig.ConnectionTimeoutMilliseconds = 1100;
            config.ModbusConnectionConfig.DisconnectionTimeoutMilliseconds = 2200;
            config.ModbusConnectionConfig.ConnectRetryCount = 4;
            config.ModbusConnectionConfig.ConnectRetryIntervalMilliseconds = 3300;
            config.ModbusConnectionConfig.OperationTimeoutMilliseconds = 4400;
            config.ModbusConnectionConfig.SendRetryCount = 6;
            config.ModbusConnectionConfig.SendRetryIntervalMilliseconds = 550;

            (IModbusDataface _, IModbusClient client) = factory.Create(config);
            _createdClients.Add(client);

            NModbusClient modbusClient = (NModbusClient)client;
            ClientConnectionConfig connectionConfig = GetConnectionConfig(modbusClient);
            Assert.AreEqual("fe80::20", modbusClient.ServerAddress);
            Assert.AreEqual(1502, modbusClient.ServerPort);
            Assert.AreEqual((byte)17, modbusClient.UnitId);
            Assert.AreEqual(AddressFamily.InterNetworkV6, connectionConfig.AddressFamily);
            Assert.AreEqual(1100, connectionConfig.ConnectionTimeoutMilliseconds);
            Assert.AreEqual(2200, connectionConfig.DisconnectionTimeoutMilliseconds);
            Assert.AreEqual(4, connectionConfig.ConnectRetryCount);
            Assert.AreEqual(3300, connectionConfig.ConnectRetryIntervalMilliseconds);
            Assert.AreEqual(4400, connectionConfig.OperationTimeoutMilliseconds);
            Assert.AreEqual(6, connectionConfig.SendRetryCount);
            Assert.AreEqual(550, connectionConfig.SendRetryIntervalMilliseconds);
        }


        [TestMethod]
        [DataRow((byte)0)]
        [DataRow((byte)1)]
        [DataRow((byte)247)]
        [DataRow(byte.MaxValue)]
        public void ModbusDeviceFactoryCreateMapsUnitIdTest(byte unitId)
        {
            ModbusDeviceFactory factory = new ModbusDeviceFactory(CreateServiceProviderMock().Object);
            ModbusConfig config = CreateConfig("Inverter-1", "192.168.1.20", 502, unitId);

            (IModbusDataface _, IModbusClient client) = factory.Create(config);
            _createdClients.Add(client);

            Assert.AreEqual(unitId, ((NModbusClient)client).UnitId);
        }


        [TestMethod]
        [DataRow((ushort)1)]
        [DataRow((ushort)502)]
        [DataRow(ushort.MaxValue)]
        public void ModbusDeviceFactoryCreateMapsPortBoundariesTest(ushort port)
        {
            ModbusDeviceFactory factory = new ModbusDeviceFactory(CreateServiceProviderMock().Object);
            ModbusConfig config = CreateConfig("Inverter-1", "192.168.1.20", port, 1);

            (IModbusDataface _, IModbusClient client) = factory.Create(config);
            _createdClients.Add(client);

            Assert.AreEqual(port, ((NModbusClient)client).ServerPort);
        }


        [TestMethod]
        public void ModbusDeviceFactoryCreateSameEndpointReturnsDistinctClientsTest()
        {
            Mock<IServiceProvider> servicesMock = CreateServiceProviderMock();
            ModbusDeviceFactory factory = new ModbusDeviceFactory(servicesMock.Object);
            ModbusConfig firstConfig = CreateConfig("Inverter-1", "192.168.1.20", 502, 1);
            ModbusConfig secondConfig = CreateConfig("Inverter-2", "192.168.1.20", 502, 2);

            (IModbusDataface firstDataface, IModbusClient firstClient) = factory.Create(firstConfig);
            (IModbusDataface secondDataface, IModbusClient secondClient) = factory.Create(secondConfig);
            _createdClients.Add(firstClient);
            _createdClients.Add(secondClient);

            Assert.AreNotSame(firstClient, secondClient);
            Assert.AreNotSame(firstDataface, secondDataface);
            Assert.AreEqual((byte)1, ((NModbusClient)firstClient).UnitId);
            Assert.AreEqual((byte)2, ((NModbusClient)secondClient).UnitId);
            servicesMock.Verify(services => services.GetService(typeof(ILogger<NModbusClient>)), Times.Exactly(2));
        }


        [TestMethod]
        public async Task ModbusDeviceFactoryCreateConcurrentCallsReturnIndependentDevicesTest()
        {
            const int CallerCount = 16;
            ModbusDeviceFactory factory = new ModbusDeviceFactory(CreateServiceProviderMock().Object);
            using Barrier barrier = new Barrier(CallerCount);
            IEnumerable<Task<(IModbusDataface Dataface, IModbusClient Client)>> tasks = Enumerable.Range(0, CallerCount).Select(index => Task.Factory.StartNew(() =>
            {
                ModbusConfig config = CreateConfig($"Inverter-{index}", "192.168.1.20", 502, (byte)(index + 1));
                barrier.SignalAndWait();

                return factory.Create(config);
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            (IModbusDataface Dataface, IModbusClient Client)[] results = await Task.WhenAll(tasks);
            _createdClients.AddRange(results.Select(result => result.Client));

            Assert.HasCount(CallerCount, results.Select(result => result.Client).Distinct());
            Assert.HasCount(CallerCount, results.Select(result => result.Dataface.Name).Distinct());
            foreach ((IModbusDataface dataface, IModbusClient client) in results)
            {
                byte expectedUnitId = (byte)(int.Parse(dataface.Name.Split('-')[1]) + 1);
                Assert.AreEqual(expectedUnitId, ((NModbusClient)client).UnitId);
            }
        }


        private Mock<IServiceProvider> CreateServiceProviderMock()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            servicesMock.Setup(services => services.GetService(typeof(ILogger<NModbusClient>)))
                .Returns(NullLogger<NModbusClient>.Instance);

            return servicesMock;
        }


        private ModbusConfig CreateConfig(string name, string address, ushort port, byte unitId, AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            return new ModbusConfig
            {
                ChangedBy = "UnitTest",
                Name = name,
                Address = address,
                Port = port,
                AddressFamily = addressFamily,
                UnitId = unitId,
                ModbusConnectionConfig = new ModbusConnectionConfig
                {
                    ChangedBy = "UnitTest",
                    Name = "Modbus Connection"
                }
            };
        }


        private ClientConnectionConfig GetConnectionConfig(NModbusClient client)
        {
            FieldInfo? field = typeof(NModbusClient).GetField("_clientConnection", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);

            return (ClientConnectionConfig)field.GetValue(client)!;
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.C37s;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Configs;
using System.Net.Sockets;
using System.Reflection;

namespace paskalON.Devices.Application.UnitTest.Factories
{
    [TestClass]
    public class C37DeviceFactoryTest
    {
        private readonly List<IC37Client> _createdClients = new List<IC37Client>();


        [TestCleanup]
        public async Task Cleanup()
        {
            foreach (IC37Client client in _createdClients.Distinct())
            {
                if (client is IAsyncDisposable disposable)
                {
                    await disposable.DisposeAsync();
                }
            }
        }


        [TestMethod]
        public void C37DeviceFactoryConstructorNullServicesTest()
        {
            IServiceProvider services = null!;

            Assert.ThrowsExactly<ArgumentNullException>(() => new C37DeviceFactory(services));
        }


        [TestMethod]
        public void C37DeviceFactoryCreateNullConfigTest()
        {
            C37DeviceFactory factory = new C37DeviceFactory(CreateServiceProviderMock().Object);

            Assert.ThrowsExactly<ArgumentNullException>(() => factory.Create(null!));
        }


        [TestMethod]
        public void C37DeviceFactoryCreateMissingLoggerRegistrationTest()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            C37DeviceFactory factory = new C37DeviceFactory(servicesMock.Object);
            C37Config config = CreateConfig("PMU-1", "192.168.1.10", 4712);

            Assert.ThrowsExactly<InvalidOperationException>(() => factory.Create(config));
        }


        [TestMethod]
        public void C37DeviceFactoryCreateMissingLoggerRegistrationDoesNotCacheClientTest()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            C37DeviceFactory factory = new C37DeviceFactory(servicesMock.Object);
            C37Config config = CreateConfig("PMU-1", "192.168.1.10", 4712);
            Assert.ThrowsExactly<InvalidOperationException>(() => factory.Create(config));
            servicesMock.Setup(services => services.GetService(typeof(ILogger<C37Client>)))
                .Returns(NullLogger<C37Client>.Instance);

            (IC37Dataface dataface, IC37Client client) = factory.Create(config);
            _createdClients.Add(client);

            Assert.IsNotNull(dataface);
            Assert.IsInstanceOfType<C37Client>(client);
        }


        [TestMethod]
        public void C37DeviceFactoryCreateReturnsDatafaceAndClientTest()
        {
            C37DeviceFactory factory = new C37DeviceFactory(CreateServiceProviderMock().Object);
            C37Config config = CreateConfig("PMU-Feeder-1", "192.168.1.10", 4712);

            (IC37Dataface dataface, IC37Client client) = factory.Create(config);
            _createdClients.Add(client);

            Assert.IsInstanceOfType<C37Register>(dataface);
            Assert.AreEqual("PMU-Feeder-1", dataface.Name);
            Assert.IsInstanceOfType<C37Client>(client);
            Assert.AreEqual(C37ClientState.Disconnected, client.State);
        }


        [TestMethod]
        public void C37DeviceFactoryCreateMapsConnectionConfigTest()
        {
            C37DeviceFactory factory = new C37DeviceFactory(CreateServiceProviderMock().Object);
            C37Config config = CreateConfig("PMU-Feeder-1", "fe80::1", 4713, AddressFamily.InterNetworkV6);
            config.C37ConnectionConfig.ConnectionTimeoutMilliseconds = 1500;
            config.C37ConnectionConfig.DisconnectionTimeoutMilliseconds = 2500;
            config.C37ConnectionConfig.ConnectRetryCount = 7;
            config.C37ConnectionConfig.ConnectRetryIntervalMilliseconds = 3500;
            config.C37ConnectionConfig.OperationTimeoutMilliseconds = 4500;

            (IC37Dataface _, IC37Client client) = factory.Create(config);
            _createdClients.Add(client);

            C37Client c37Client = (C37Client)client;
            ClientConnectionConfig connectionConfig = GetConnectionConfig(c37Client);
            Assert.AreEqual("fe80::1", c37Client.ServerAddress);
            Assert.AreEqual(4713, c37Client.ServerPort);
            Assert.AreEqual("fe80::1", connectionConfig.ServerAddress);
            Assert.AreEqual(4713, connectionConfig.ServerPort);
            Assert.AreEqual(AddressFamily.InterNetworkV6, connectionConfig.AddressFamily);
            Assert.AreEqual(1500, connectionConfig.ConnectionTimeoutMilliseconds);
            Assert.AreEqual(2500, connectionConfig.DisconnectionTimeoutMilliseconds);
            Assert.AreEqual(7, connectionConfig.ConnectRetryCount);
            Assert.AreEqual(3500, connectionConfig.ConnectRetryIntervalMilliseconds);
            Assert.AreEqual(4500, connectionConfig.OperationTimeoutMilliseconds);
        }


        [TestMethod]
        [DataRow("192.168.1.10", "192.168.1.10")]
        [DataRow("pmu.local", "PMU.LOCAL")]
        [DataRow("Pmu.Local", "pmu.local")]
        public void C37DeviceFactoryCreateSameEndpointReturnsSharedClientTest(string firstAddress, string secondAddress)
        {
            Mock<IServiceProvider> servicesMock = CreateServiceProviderMock();
            C37DeviceFactory factory = new C37DeviceFactory(servicesMock.Object);
            C37Config firstConfig = CreateConfig("PMU-1", firstAddress, 4712);
            C37Config secondConfig = CreateConfig("PMU-2", secondAddress, 4712);

            (IC37Dataface firstDataface, IC37Client firstClient) = factory.Create(firstConfig);
            (IC37Dataface secondDataface, IC37Client secondClient) = factory.Create(secondConfig);
            _createdClients.Add(firstClient);
            _createdClients.Add(secondClient);

            Assert.AreSame(firstClient, secondClient);
            Assert.AreNotSame(firstDataface, secondDataface);
            Assert.AreEqual("PMU-1", firstDataface.Name);
            Assert.AreEqual("PMU-2", secondDataface.Name);
            servicesMock.Verify(services => services.GetService(typeof(ILogger<C37Client>)), Times.Once);
        }


        [TestMethod]
        public void C37DeviceFactoryCreateSameEndpointKeepsFirstConnectionConfigTest()
        {
            C37DeviceFactory factory = new C37DeviceFactory(CreateServiceProviderMock().Object);
            C37Config firstConfig = CreateConfig("PMU-1", "192.168.1.10", 4712);
            firstConfig.C37ConnectionConfig.OperationTimeoutMilliseconds = 1000;
            C37Config secondConfig = CreateConfig("PMU-2", "192.168.1.10", 4712);
            secondConfig.C37ConnectionConfig.OperationTimeoutMilliseconds = 9000;

            (IC37Dataface _, IC37Client firstClient) = factory.Create(firstConfig);
            (IC37Dataface _, IC37Client secondClient) = factory.Create(secondConfig);
            _createdClients.Add(firstClient);

            Assert.AreSame(firstClient, secondClient);
            Assert.AreEqual(1000, GetConnectionConfig((C37Client)secondClient).OperationTimeoutMilliseconds);
        }


        [TestMethod]
        [DataRow("192.168.1.10", (ushort)4712, AddressFamily.InterNetwork, "192.168.1.11", (ushort)4712, AddressFamily.InterNetwork)]
        [DataRow("192.168.1.10", (ushort)4712, AddressFamily.InterNetwork, "192.168.1.10", (ushort)4713, AddressFamily.InterNetwork)]
        [DataRow("pmu.local", (ushort)4712, AddressFamily.InterNetwork, "pmu.local", (ushort)4712, AddressFamily.InterNetworkV6)]
        [DataRow("192.168.1.10", (ushort)1, AddressFamily.InterNetwork, "192.168.1.10", ushort.MaxValue, AddressFamily.InterNetwork)]
        public void C37DeviceFactoryCreateDifferentEndpointReturnsDistinctClientTest(string firstAddress, ushort firstPort, AddressFamily firstFamily,
            string secondAddress, ushort secondPort, AddressFamily secondFamily)
        {
            C37DeviceFactory factory = new C37DeviceFactory(CreateServiceProviderMock().Object);
            C37Config firstConfig = CreateConfig("PMU-1", firstAddress, firstPort, firstFamily);
            C37Config secondConfig = CreateConfig("PMU-2", secondAddress, secondPort, secondFamily);

            (IC37Dataface _, IC37Client firstClient) = factory.Create(firstConfig);
            (IC37Dataface _, IC37Client secondClient) = factory.Create(secondConfig);
            _createdClients.Add(firstClient);
            _createdClients.Add(secondClient);

            Assert.AreNotSame(firstClient, secondClient);
            Assert.AreEqual(firstPort, ((C37Client)firstClient).ServerPort);
            Assert.AreEqual(secondPort, ((C37Client)secondClient).ServerPort);
        }


        [TestMethod]
        public async Task C37DeviceFactoryCreateConcurrentSameEndpointReturnsSingleClientTest()
        {
            const int CallerCount = 16;
            Mock<IServiceProvider> servicesMock = CreateServiceProviderMock();
            C37DeviceFactory factory = new C37DeviceFactory(servicesMock.Object);
            using Barrier barrier = new Barrier(CallerCount);
            IEnumerable<Task<IC37Client>> tasks = Enumerable.Range(0, CallerCount).Select(index => Task.Factory.StartNew(() =>
            {
                C37Config config = CreateConfig($"PMU-{index}", "192.168.1.10", 4712);
                barrier.SignalAndWait();

                return factory.Create(config).Client;
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            IC37Client[] clients = await Task.WhenAll(tasks);
            _createdClients.AddRange(clients);

            Assert.HasCount(1, clients.Distinct());
            servicesMock.Verify(services => services.GetService(typeof(ILogger<C37Client>)), Times.Once);
        }


        [TestMethod]
        public async Task C37DeviceFactoryCreateConcurrentDifferentEndpointsReturnsClientPerEndpointTest()
        {
            const int EndpointCount = 8;
            const int CallsPerEndpoint = 4;
            C37DeviceFactory factory = new C37DeviceFactory(CreateServiceProviderMock().Object);
            using Barrier barrier = new Barrier(EndpointCount * CallsPerEndpoint);
            IEnumerable<Task<(ushort Port, IC37Client Client)>> tasks = Enumerable.Range(0, EndpointCount * CallsPerEndpoint).Select(index => Task.Factory.StartNew(() =>
            {
                ushort port = (ushort)(4712 + (index % EndpointCount));
                C37Config config = CreateConfig($"PMU-{index}", "192.168.1.10", port);
                barrier.SignalAndWait();

                return (port, factory.Create(config).Client);
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            (ushort Port, IC37Client Client)[] results = await Task.WhenAll(tasks);
            _createdClients.AddRange(results.Select(result => result.Client));

            Assert.HasCount(EndpointCount, results.Select(result => result.Client).Distinct());
            foreach (IGrouping<ushort, (ushort Port, IC37Client Client)> group in results.GroupBy(result => result.Port))
            {
                Assert.HasCount(1, group.Select(result => result.Client).Distinct());
                Assert.AreEqual(group.Key, ((C37Client)group.First().Client).ServerPort);
            }
        }


        private Mock<IServiceProvider> CreateServiceProviderMock()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            servicesMock.Setup(services => services.GetService(typeof(ILogger<C37Client>)))
                .Returns(NullLogger<C37Client>.Instance);

            return servicesMock;
        }


        private C37Config CreateConfig(string name, string address, ushort port, AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            return new C37Config
            {
                ChangedBy = "UnitTest",
                Name = name,
                Address = address,
                Port = port,
                AddressFamily = addressFamily,
                TransportLayer = C37TransportLayer.TCP,
                StationName = "PMU",
                StreamId = 1,
                C37ConnectionConfig = new C37ConnectionConfig
                {
                    ChangedBy = "UnitTest",
                    Name = "C37 Connection"
                }
            };
        }


        private ClientConnectionConfig GetConnectionConfig(C37Client client)
        {
            FieldInfo? field = typeof(C37Client).GetField("_clientConnection", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);

            return (ClientConnectionConfig)field.GetValue(client)!;
        }
    }
}

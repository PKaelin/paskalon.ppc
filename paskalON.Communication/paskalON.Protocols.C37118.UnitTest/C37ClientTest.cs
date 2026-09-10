// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Dataface.C37s;
using paskalON.Protocols.C37118.Configs;
using System.Net;
using System.Net.Sockets;

namespace paskalON.Protocols.C37118.UnitTest
{
    [TestClass]
    public sealed class C37ClientTest
    {
        [TestMethod]
        public void C37ClientConstructorCopiesConnectionPropertiesTest()
        {
            ClientConnectionConfig configuration = CreateConfiguration(12000);
            C37Client client = new C37Client(NullLogger<C37Client>.Instance, configuration);

            Assert.AreEqual("127.0.0.1", client.ServerAddress);
            Assert.AreEqual(12000, client.ServerPort);
            Assert.AreEqual(C37ClientState.Disconnected, client.State);
        }


        [TestMethod]
        public async Task C37ClientStartStreamingAsyncWhenEndpointIsUnavailableRaisesCommunicationErrorTest()
        {
            ClientConnectionConfig configuration = CreateConfiguration(GetUnusedPort());
            C37Client client = new C37Client(NullLogger<C37Client>.Instance, configuration);
            int errorCount = 0;
            client.OnCommunicationError += (_, _) => errorCount++;

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.StartStreamingAsync());

            Assert.AreEqual(C37ClientState.Disconnected, client.State);
            Assert.AreEqual(1, errorCount);
            await client.DisposeAsync();
        }


        [TestMethod]
        public async Task C37ClientSendCommandAsyncWhenDisconnectedRejectsOperationTest()
        {
            C37Client client = new C37Client(NullLogger<C37Client>.Instance, CreateConfiguration(12001));

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.SendCommandAsync(C37CommandType.TurnOnTransmission));
            await client.DisposeAsync();
        }


        private ClientConnectionConfig CreateConfiguration(int port)
        {
            return new ClientConnectionConfig
            {
                ServerAddress = "127.0.0.1",
                ServerPort = port,
                AddressFamily = AddressFamily.InterNetwork,
                ConnectionTimeoutMilliseconds = 50,
                ConnectRetryCount = 0,
                ConnectRetryIntervalMilliseconds = 1,
                OperationTimeoutMilliseconds = 1
            };
        }


        private int GetUnusedPort()
        {
            using TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }
    }
}

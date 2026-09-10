// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Protocols.C37118.Configs;
using paskalON.Protocols.C37118.Simulations;
using System.Net;
using System.Net.Sockets;

namespace paskalON.Protocols.C37118.IntegrationTest
{
    [TestClass]
    public sealed class C37ClientServerIntegrationTest
    {
        [TestMethod]
        public async Task C37ClientServerStreamsConfigurationAndDataFramesTest()
        {
            int port = GetUnusedPort();

            PmuDataSimulation simulation = new PmuDataSimulation
            {
                StreamId = 15,
                Frequency = 50,
                FrequencyRateOfChange = 0.1f,
                Phasors = new[] { new PhasorMeasurement("VA", 0.2f, 1.5f, PhasorUnitTypes.Voltage) },
                Analogs = new[] { new AnalogMeasurement("P", 5.25f) }
            };

            C37Server server = new C37Server(NullLogger<C37Server>.Instance, new[] { simulation }, port, 1);
            C37Client client = new C37Client(NullLogger<C37Client>.Instance, CreateConfiguration(port));
            TaskCompletionSource<bool> configurationReceived = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource<bool> dataReceived = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            client.ConfigFrameReceived += (_, args) =>
            {
                if (args.Blueprint.Pmus.Count == 1)
                {
                    configurationReceived.TrySetResult(true);
                }
            };

            client.DataFrameReceived += (_, args) =>
            {
                if (args.Header.StreamIdCode == 15)
                {
                    dataReceived.TrySetResult(true);
                }
            };

            try
            {
                await server.StartAsync();
                await client.StartStreamingAsync();
                await configurationReceived.Task.WaitAsync(TimeSpan.FromSeconds(3));
                await dataReceived.Task.WaitAsync(TimeSpan.FromSeconds(3));

                Assert.AreEqual(C37ServerState.Streaming, server.State);
                Assert.AreEqual(C37ClientState.Connected, client.State);
            }
            finally
            {
                await client.StopStreamingAsync();
                await server.StopAsync();
                await client.DisposeAsync();
                await server.DisposeAsync();
            }
        }


        private ClientConnectionConfig CreateConfiguration(int port)
        {
            return new ClientConnectionConfig
            {
                ServerAddress = "127.0.0.1",
                ServerPort = port,
                AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork,
                ConnectionTimeoutMilliseconds = 1000,
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

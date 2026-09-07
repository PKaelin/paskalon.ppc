// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Configs;
using paskalON.Protocols.Modbus.NModbus;
using paskalON.Protocols.Modbus.Stores;
using System.Net;
using System.Net.Sockets;

namespace paskalON.Protocols.Modbus.IntegrationTest
{
    [TestClass]
    public sealed class ModbusClientServerIntegrationTest
    {
        [TestMethod]
        public async Task ModbusClientServerReadsAndWritesAllRegisterFamiliesTest()
        {
            int port = GetUnusedPort();

            ModbusDataMemoryStore store = new ModbusDataMemoryStore(32, 32, 32, 32);
            store.CoilDiscretes.WritePoints(2, new[] { true, false, true });
            store.CoilInputs.WritePoints(2, new[] { false, true, false });
            store.InputRegisters.WritePoints(2, new ushort[] { 100, 200, 300 });
            await using NModbusServer server = new NModbusServer(NullLogger<NModbusServer>.Instance, store, "127.0.0.1", port, 1);
            await using NModbusClient client = new NModbusClient(NullLogger<NModbusClient>.Instance, CreateConfiguration(port), 1);

            try
            {
                await server.StartAsync();
                await client.ConnectAsync();
                bool[] coils = await client.ReadCoilsAsync(2, 4);
                bool[] inputs = await client.ReadDiscreteInputsAsync(2, 4);
                ushort[] inputRegisters = await client.ReadInputRegistersAsync(2, 4);
                await client.WriteMultipleRegistersAsync(5, new ushort[] { 11, 22, 33 }, ModbusDataType.MbUint16);
                ushort[] writtenRegisters = store.HoldingRegisters.ReadPoints(5, 3);

                CollectionAssert.AreEqual(new[] { true, false, true }, coils);
                CollectionAssert.AreEqual(new[] { false, true, false }, inputs);
                CollectionAssert.AreEqual(new ushort[] { 100, 200, 300 }, inputRegisters);
                CollectionAssert.AreEqual(new ushort[] { 11, 22, 33 }, writtenRegisters);
                Assert.AreEqual(ModbusClientState.Connected, client.State);
                Assert.AreEqual(ModbusServerState.Listening, server.State);
            }
            finally
            {
                await client.DisconnectAsync();
                await server.StopAsync();
            }
        }


        private static ClientConnectionConfig CreateConfiguration(int port)
        {
            return new ClientConnectionConfig
            {
                ServerAddress = "127.0.0.1",
                ServerPort = port,
                AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork,
                ConnectionTimeoutMilliseconds = 1000,
                DisconnectionTimeoutMilliseconds = 1000,
                ConnectRetryCount = 0,
                ConnectRetryIntervalMilliseconds = 1,
                OperationTimeoutMilliseconds = 1000,
                SendRetryCount = 0,
                SendRetryIntervalMilliseconds = 1
            };
        }


        private static int GetUnusedPort()
        {
            using TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }
    }
}

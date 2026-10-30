// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.UnitTest.PowerConversionSystems.Simples
{
    [TestClass]
    public class PcsSimpleV1ProxyTest
    {
        private Mock<DerBatteryStorageUnit>? _unit;
        private PowerConversionSystemConfig? _pcsConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            // Der
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");
            Mock<Der> der = new Mock<Der>(NullLogger.Instance, derConfig.Object);
            // Group
            Mock<DerGroupConfig> groupConfig = new Mock<DerGroupConfig>();
            groupConfig.SetupGet(x => x.Name).Returns("DerGroupConfig");
            Mock<DerGroup> group = new Mock<DerGroup>(NullLogger.Instance, groupConfig.Object, der.Object);
            // Circuit
            Mock<DerCircuitConfig> circuitConfig = new Mock<DerCircuitConfig>();
            circuitConfig.SetupGet(x => x.Name).Returns("DerCircuitConfig");
            Mock<DerCircuit> circuit = new Mock<DerCircuit>(NullLogger.Instance, circuitConfig.Object, group.Object);
            // Unit
            Mock<DerBatteryStorageUnitConfig> unitConfig = new Mock<DerBatteryStorageUnitConfig>();
            unitConfig.SetupGet(x => x.Name).Returns("DerBatteryStorageUnitConfig");
            _unit = new Mock<DerBatteryStorageUnit>(NullLogger.Instance, unitConfig.Object, circuit.Object);

            PowerConversionSystemDeviceConfig deviceConfig = new PowerConversionSystemDeviceConfig
            {
                ChangedBy = "Test",
                Name = "PowerConversionSystemDeviceConfig",
                ClassName = "ClassName",
                ZeroOutputOnCommLoss = false
            };
            // Device
            _pcsConfig = new PowerConversionSystemConfig()
            {
                ChangedBy = "Test",
                Name = "PowerConversionSystemConfig",
                IsActive = true,
                DeviceId = 1,
                DerUnitConfig = unitConfig.Object,
                PowerConversionSystemDeviceConfig = deviceConfig,
                ModbusConfig = new Mock<ModbusConfig>().Object,
            };
        }



        [TestMethod]
        public void CreatePcsWithNullClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, null!));
        }


        [TestMethod]
        public void CreatePcsWithNullDatafaceTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, null!, client.Object));
        }



        [TestMethod]
        public void CreatePcsWithMockedClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            Assert.AreEqual(_pcsConfig!.Name, pcs.Name);
        }


        [TestMethod]
        public void PcsWithMockedClientComErrorTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            FakeLogger<PcsSimpleV1ProxyTest> logger = new FakeLogger<PcsSimpleV1ProxyTest>();

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(logger, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);
            EventArgs expectedEvent = new EventArgs();
            client.Raise(x => x.OnCommunicationError += null, this, expectedEvent);

            Assert.AreEqual(_pcsConfig!.Name, pcs.Name);
            Assert.IsTrue(pcs.CommunicationError);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsTrue(logs.First().Message.Contains("CommunicationError state", StringComparison.OrdinalIgnoreCase));
        }


        [TestMethod]
        public async Task PcsStartTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();

            ushort stateTarget = (ushort)PcsSimpleV1Description.State.On;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? state = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, ushort, ModbusDataType, short, double, CancellationToken>((adr, val, type, priority, scale, token) => { address = adr; state = val * scale; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await pcs.StartAsync();

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual((ushort)PcsSimpleV1Description.Register.SelectorState, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            Assert.AreEqual(stateTarget, state);
            Assert.AreEqual(PcsState.Starting, pcs.State);
        }


        [TestMethod]
        public async Task PcsStopTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            client.Setup(x => x.State).Returns(ModbusClientState.Connected);

            ushort stateTarget = (ushort)PcsSimpleV1Description.State.Off;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? state = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, ushort, ModbusDataType, short, double, CancellationToken>((adr, val, type, priority, scale, token) => { address = adr; state = val * scale; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await pcs.StopAsync();

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            Assert.AreEqual((ushort)PcsSimpleV1Description.Register.SelectorState, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            Assert.AreEqual(stateTarget, state);
            Assert.AreEqual(PcsState.Stopping, pcs.State);
        }


        [TestMethod]
        public async Task PcsSetActivePowerTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            client.Setup(x => x.State).Returns(ModbusClientState.Connected);

            double? activePowerTarget = 11000;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? activePower = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<double>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, double, ModbusDataType, short, double, CancellationToken>((adr, val, type, priority, scale, token) => { address = adr; activePower = val * scale; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await pcs.SetActivePowerTargetAsync(activePowerTarget);

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<double>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual((ushort)PcsSimpleV1Description.Register.PReference, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            // Active power is written in kilo watts
            Assert.AreEqual(activePowerTarget / 1000, activePower);
        }


        [TestMethod]
        public async Task PcsSetReactivePowerTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IModbusDataface> dataface = new Mock<IModbusDataface>();
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            client.Setup(x => x.State).Returns(ModbusClientState.Connected);

            double? reactivePowerTarget = 12000;
            ushort? address = null;
            ModbusDataType? modbusDataType = null;
            double? reactivePower = null;

            client
                .Setup(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<double>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
                .Callback<ushort, double, ModbusDataType, short, double, CancellationToken>((adr, val, type, priority, scale, token) => { address = adr; reactivePower = val * scale; modbusDataType = type; })
                .Returns(Task.CompletedTask);

            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, publisher.Object, dataface.Object, client.Object);

            await pcs.SetReactivePowerTargetAsync(reactivePowerTarget);

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<double>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual((ushort)PcsSimpleV1Description.Register.QReference, address);
            Assert.AreEqual(ModbusDataType.MbInt16, modbusDataType);
            // Reactive power is written in kilo vars
            Assert.AreEqual(reactivePowerTarget / 1000, reactivePower);
        }


        [TestMethod]
        public async Task PcsConnectedSendsStartCommandOnFirstConnectOnlyTest()
        {
            _pcsConfig!.InitiallyStarted = true;
            TaskCompletionSource written = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            Mock<IModbusClient> client = CreateConnectedClient();
            client
                .Setup(x => x.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState, (ushort)PcsSimpleV1Description.State.On, ModbusDataType.MbInt16, It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
                .Callback(() => written.TrySetResult())
                .Returns(Task.CompletedTask);
            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig, _unit!.Object, new Mock<IMetricsPublisher>().Object, new Mock<IModbusDataface>().Object, client.Object);

            client.Raise(x => x.OnConnected += null, client.Object, EventArgs.Empty);
            await written.Task.WaitAsync(TimeSpan.FromSeconds(10));
            client.Raise(x => x.OnConnected += null, client.Object, EventArgs.Empty);

            client.Verify(x => x.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState, (ushort)PcsSimpleV1Description.State.On, ModbusDataType.MbInt16, It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(PcsState.Starting, pcs.State);
        }


        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void PcsConnectedDoesNotSendStartCommandTest(bool initiallyStarted, bool inMaintenanceMode)
        {
            _pcsConfig!.InitiallyStarted = initiallyStarted;
            _unit!.Object.IsInMaintenanceMode = inMaintenanceMode;
            Mock<IModbusClient> client = CreateConnectedClient();
            PcsSimpleV1Proxy pcs = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig, _unit.Object, new Mock<IMetricsPublisher>().Object, new Mock<IModbusDataface>().Object, client.Object);

            client.Raise(x => x.OnConnected += null, client.Object, EventArgs.Empty);

            client.Verify(x => x.WriteSingleRegisterAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<ModbusDataType>(), It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.AreNotEqual(PcsState.Starting, pcs.State);
        }


        [TestMethod]
        public async Task PcsConnectedRetriesStartCommandAfterFailureTest()
        {
            _pcsConfig!.InitiallyStarted = true;
            int attempts = 0;
            TaskCompletionSource written = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            Mock<IModbusClient> client = CreateConnectedClient();
            client
                .Setup(x => x.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState, (ushort)PcsSimpleV1Description.State.On, ModbusDataType.MbInt16, It.IsAny<short>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    attempts++;

                    if (attempts == 1)
                    {
                        return Task.FromException(new InvalidOperationException("The client is not connected."));
                    }

                    written.TrySetResult();

                    return Task.CompletedTask;
                });
            PcsWithoutRetryDelay pcs = new PcsWithoutRetryDelay(_pcsConfig, _unit!.Object, client.Object);

            client.Raise(x => x.OnConnected += null, client.Object, EventArgs.Empty);
            await written.Task.WaitAsync(TimeSpan.FromSeconds(10));

            Assert.AreEqual(2, attempts);
            Assert.AreEqual(_pcsConfig.Name, pcs.Name);
        }


        /// <summary>
        /// Creates a Modbus client mock in connected state.
        /// </summary>
        /// <returns>The Modbus client mock.</returns>
        private Mock<IModbusClient> CreateConnectedClient()
        {
            Mock<IModbusClient> client = new Mock<IModbusClient>();
            client.Setup(x => x.State).Returns(ModbusClientState.Connected);

            return client;
        }


        /// <summary>
        /// PCS without a delay between the automatic initial start attempts.
        /// </summary>
        private sealed class PcsWithoutRetryDelay : PcsSimpleV1Proxy
        {
            /// <inheritdoc/>
            protected override TimeSpan InitialStartRetryDelay { get => TimeSpan.Zero; }


            /// <summary>
            /// Constructor of <see cref="PcsWithoutRetryDelay"/>.
            /// </summary>
            /// <param name="config">The power conversion system configuration.</param>
            /// <param name="derUnit">The parent DER unit.</param>
            /// <param name="client">The Modbus client interface.</param>
            public PcsWithoutRetryDelay(PowerConversionSystemConfig config, DerUnit derUnit, IModbusClient client)
                : base(NullLogger.Instance, config, derUnit, new Mock<IMetricsPublisher>().Object, new Mock<IModbusDataface>().Object, client)
            {
            }
        }
    }
}

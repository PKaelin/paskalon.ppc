// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Communication.Protocols.C37118.Types;
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Equipments.C37;
using paskalON.Devices.Equipments.Meters.PowerMeters.Simples;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Frames;
using paskalON.Protocols.C37118.Generators;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.IntegrationTest.Meters.PowerMeters.Simples
{
    [TestClass]
    public class SystemPowerMeterSimpleV1ProxyTest
    {
        private SystemPowerMeterConfig? _pmConfig;
        private PowerMeterMapC37Config? _powerMeterMapC37Config;


        [TestInitialize]
        public void TestInitialize()
        {
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");

            _powerMeterMapC37Config = new PowerMeterMapC37Config
            {
                ChangedBy = "Test",
                Name = "PowerMeterMapC37Config",
                // Power
                ApparentPower = "Analog0",
                ActivePower = "Analog1",
                ActivePowerA = "Analog2",
                ActivePowerB = "Analog3",
                ActivePowerC = "Analog4",
                ReactivePower = "Analog5",
                ReactivePowerA = "Analog6",
                ReactivePowerB = "Analog7",
                ReactivePowerC = "Analog8",
                EnergyDelivered = "Analog9",
                EnergyReceived = "Analog10",
                ReactiveEnergyDelivered = "Analog11",
                ReactiveEnergyReceived = "Analog12",
                // Voltage
                VoltageA = "Phasor0",
                VoltageB = "Phasor1",
                VoltageC = "Phasor2",
                VoltageAB = "Phasor3",
                VoltageBC = "Phasor4",
                VoltageCA = "Phasor5",
                VoltagePositiveSequence = "Phasor6",
                VoltageLLAvg = "Analog100",
                // Current
                CurrentA = "Phasor7",
                CurrentB = "Phasor8",
                CurrentC = "Phasor9",
            };

            C37Config c37Config = new C37Config
            {
                ChangedBy = "Test",
                Name = "C37Config",
                StationName = "PMU",
                StreamId = 1,
                IpAddress = "127.0.0.1",
                Port = 52,
                TransportLayer = C37TransportLayer.UDP
            };

            PowerMeterDeviceConfig powerMeterDeviceConfig = new PowerMeterDeviceConfig
            {
                ChangedBy = "Test",
                Name = "PowerMeterDeviceConfig",
                ClassName = "ClassName",
                PowerMeterMapC37Config = _powerMeterMapC37Config
            };

            _pmConfig = new SystemPowerMeterConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerMeterConfig",
                IsActive = true,
                DeviceId = 1,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = derConfig.Object,
                PowerMeterDeviceConfig = powerMeterDeviceConfig,
                C37Config = c37Config,
            };
        }


        [TestMethod]
        public async Task PowerMeterTransmissionTest()
        {
            float signalFrequencyValue = 50;
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            Mock<IC37Client> client = new Mock<IC37Client>();
            C37TransmissionEngine engine = new C37TransmissionEngine(NullLogger.Instance, client.Object, dataface, _pmConfig!.C37Config!.StationName, _pmConfig!.C37Config!.StreamId);
            SystemPowerMeterSimpleV1Proxy meter = new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, publisher.Object, dataface, client.Object);

            // Setup phasor names for config frame in a specific order
            HashSet<string> namesPhasor = new HashSet<string>()
            {
                // Current
                _powerMeterMapC37Config!.CurrentA!,
                _powerMeterMapC37Config!.CurrentB!,
                _powerMeterMapC37Config!.CurrentC!,
                // Voltage
                _powerMeterMapC37Config!.VoltageA!,
                _powerMeterMapC37Config!.VoltageB!,
                _powerMeterMapC37Config!.VoltageC!,
                _powerMeterMapC37Config!.VoltageAB!,
                _powerMeterMapC37Config!.VoltageBC!,
                _powerMeterMapC37Config!.VoltageCA!,
                _powerMeterMapC37Config!.VoltagePositiveSequence!,
            };
            // Setup analog names for config frame in a specific order
            HashSet<string> namesAnalog = new HashSet<string>()
            {
                // Power
                _powerMeterMapC37Config!.ActivePower!,
                _powerMeterMapC37Config!.ActivePowerA!,
                _powerMeterMapC37Config!.ActivePowerB!,
                _powerMeterMapC37Config!.ActivePowerC!,
                _powerMeterMapC37Config!.ApparentPower!,
                _powerMeterMapC37Config!.ReactivePower!,
                _powerMeterMapC37Config!.ReactivePowerA!,
                _powerMeterMapC37Config!.ReactivePowerB!,
                _powerMeterMapC37Config!.ReactivePowerC!,
                // Energy
                _powerMeterMapC37Config!.EnergyDelivered!,
                _powerMeterMapC37Config!.EnergyReceived!,
                _powerMeterMapC37Config!.ReactiveEnergyDelivered!,
                _powerMeterMapC37Config!.ReactiveEnergyReceived!,
                // Voltage but analog
                _powerMeterMapC37Config!.VoltageLLAvg!,
            };
            // Setup phasor values for data frame in a specific order
            List<(float Mag, float Ang)> valuesPhasors = new List<(float Mag, float Ang)>
            {
                (1,11), // CurrentA
                (2,22), // CurrentB
                (3,33), // CurrentC                
                (4,44), // VoltageA
                (5,55), // VoltageB
                (6,66), // VoltageC
                (7,77), // VoltageAB
                (8,88), // VoltageBC
                (9,99), // VoltageCA                
                (10,100), // VoltagePositiveSequence
            };
            // Setup analog values for data frame in a specific order
            List<float> valuesAnalogs = new List<float>
            {
                1, // ActivePower
                2, // ActivePowerA
                3, // ActivePowerB
                4, // ActivePowerC
                5, // ApparentPower
                6, // ReactivePower
                7, // ReactivePowerA
                8, // ReactivePowerB
                9, // ReactivePowerC
                10, // EnergyDelivered
                11, // EnergyReceived
                12, // ReactiveEnergyDelivered
                13, // ReactiveEnergyReceived
                14, // VoltageLLAvg                
            };

            byte[] configBytes = C37DataGenerator.CreateConfigFrame(_pmConfig!.C37Config!.StationName, _pmConfig!.C37Config!.StreamId, namesPhasor.ToList(), namesAnalog.ToList());
            byte[] dataBytes = C37DataGenerator.CreateDataFrame(_pmConfig!.C37Config.StreamId, valuesPhasors, valuesAnalogs, signalFrequencyValue);

            C37ConfigFrameEventArgs expectedConfigFrame = new C37ConfigFrameEventArgs(configBytes);
            C37DataFrameEventArgs expectedDataFrame = new C37DataFrameEventArgs(dataBytes);
            C37DataFrameEventArgs? raisedDataFrame = null;
            client.Object.DataFrameReceived += (sender, args) => { raisedDataFrame = args; };

            client.Raise(c => c.ConfigFrameReceived += null, this, expectedConfigFrame);
            client.Raise(c => c.DataFrameReceived += null, this, expectedDataFrame);

            Assert.IsNotNull(raisedDataFrame);
            Assert.HasCount(dataface.Registers.Count, engine.Mappings);
            CollectionAssert.AreEquivalent(dataface.Registers.Select(n => n.Name).ToArray(), engine.Mappings.Select(n => n.Register.Name).ToArray());
            // Current phasors
            Assert.AreEqual(valuesPhasors[0].Mag, meter.CurrentAMagnitude);
            Assert.AreEqual(valuesPhasors[0].Ang, meter.CurrentAAngle);
            Assert.AreEqual(valuesPhasors[1].Mag, meter.CurrentBMagnitude);
            Assert.AreEqual(valuesPhasors[1].Ang, meter.CurrentBAngle);
            Assert.AreEqual(valuesPhasors[2].Mag, meter.CurrentCMagnitude);
            Assert.AreEqual(valuesPhasors[2].Ang, meter.CurrentCAngle);
            // Voltage phasors
            Assert.AreEqual(valuesPhasors[3].Mag, meter.VoltageAMagnitude);
            Assert.AreEqual(valuesPhasors[3].Ang, meter.VoltageAAngle);
            Assert.AreEqual(valuesPhasors[4].Mag, meter.VoltageBMagnitude);
            Assert.AreEqual(valuesPhasors[4].Ang, meter.VoltageBAngle);
            Assert.AreEqual(valuesPhasors[5].Mag, meter.VoltageCMagnitude);
            Assert.AreEqual(valuesPhasors[5].Ang, meter.VoltageCAngle);
            Assert.AreEqual(valuesPhasors[6].Mag, meter.VoltageABMagnitude);
            Assert.AreEqual(valuesPhasors[6].Ang, meter.VoltageABAngle);
            Assert.AreEqual(valuesPhasors[7].Mag, meter.VoltageBCMagnitude);
            Assert.AreEqual(valuesPhasors[7].Ang, meter.VoltageBCAngle);
            Assert.AreEqual(valuesPhasors[8].Mag, meter.VoltageCAMagnitude);
            Assert.AreEqual(valuesPhasors[8].Ang, meter.VoltageCAAngle);
            Assert.AreEqual(valuesPhasors[9].Mag, meter.VoltagePositiveSequenceMagnitude);
            Assert.AreEqual(valuesPhasors[9].Ang, meter.VoltagePositiveSequenceAngle);
            // Power analogs
            Assert.AreEqual(valuesAnalogs[0], meter.ActivePowerValue);
            Assert.AreEqual(valuesAnalogs[1], meter.ActivePowerAValue);
            Assert.AreEqual(valuesAnalogs[2], meter.ActivePowerBValue);
            Assert.AreEqual(valuesAnalogs[3], meter.ActivePowerCValue);
            Assert.AreEqual(valuesAnalogs[4], meter.ApparentPowerValue);
            Assert.AreEqual(valuesAnalogs[5], meter.ReactivePowerValue);
            Assert.AreEqual(valuesAnalogs[6], meter.ReactivePowerAValue);
            Assert.AreEqual(valuesAnalogs[7], meter.ReactivePowerBValue);
            Assert.AreEqual(valuesAnalogs[8], meter.ReactivePowerCValue);
            Assert.AreEqual(valuesAnalogs[9], meter.EnergyDeliveredValue);
            Assert.AreEqual(valuesAnalogs[10], meter.EnergyReceivedValue);
            Assert.AreEqual(valuesAnalogs[11], meter.ReactiveEnergyDeliveredValue);
            Assert.AreEqual(valuesAnalogs[12], meter.ReactiveEnergyReceivedValue);
            Assert.AreEqual(valuesAnalogs[13], meter.VoltageLLAvg);
            // Frequency
            Assert.AreEqual(signalFrequencyValue, meter.Frequency);
        }
    }
}

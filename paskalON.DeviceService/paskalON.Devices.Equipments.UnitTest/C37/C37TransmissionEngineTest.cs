// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.C37s;
using paskalON.Devices.Equipments.C37;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Frames;
using paskalON.Protocols.C37118.Generators;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.UnitTest.C37
{
    [TestClass]
    public class C37TransmissionEngineTest
    {
        private float _analogValue;
        private ulong _phasorValue;
        private float _frequencyValue;


        [TestMethod]
        public async Task TransmissionEngineConfigFrameWithOneAnalogTest()
        {
            string signalName = "ActivePower";
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            dataface.Register<C37TransmissionEngineTest, IC37Register>(r => r.Register<C37TransmissionEngineTest, float>(this, signalName, C37SignalType.Analog, (x, v) => x._analogValue = v));
            Mock<IC37Client> client = new Mock<IC37Client>();
            C37TransmissionEngine engine = new C37TransmissionEngine(NullLogger.Instance, client.Object, dataface);

            byte[] configBytes = C37DataGenerator.CreateConfigFrame(1, 1, new List<string>(), new List<string> { signalName });
            C37ConfigFrameEventArgs expectedFrame = new C37ConfigFrameEventArgs(configBytes);
            C37ConfigFrameEventArgs? raisedFrame = null;
            client.Object.ConfigFrameReceived += (sender, args) => { raisedFrame = args; };

            client.Raise(c => c.ConfigFrameReceived += null, this, expectedFrame);

            Assert.IsNotNull(raisedFrame);
            Assert.AreEqual(1, raisedFrame.Header.StreamIdCode);
            Assert.HasCount(2, raisedFrame.Blueprint.ChannelMap);
            Assert.IsNotNull(raisedFrame.Blueprint.ChannelMap.FirstOrDefault(n => n.Key == "FREQUENCY"));
            Assert.IsNotNull(raisedFrame.Blueprint.ChannelMap.FirstOrDefault(n => n.Key == signalName));
            Assert.HasCount(1, engine.Mappings);
            Assert.IsNotNull(engine.Mappings.FirstOrDefault(n => n.Register.Name == signalName));
        }


        [TestMethod]
        public async Task TransmissionEngineConfigFrameWithOnePhasorTest()
        {
            string signalName = "VoltageA";
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            dataface.Register<C37TransmissionEngineTest, IC37Register>(r => r.Register<C37TransmissionEngineTest, ulong>(this, signalName, C37SignalType.Phasor, (x, v) => x._phasorValue = v));
            Mock<IC37Client> client = new Mock<IC37Client>();
            C37TransmissionEngine engine = new C37TransmissionEngine(NullLogger.Instance, client.Object, dataface);

            byte[] configBytes = C37DataGenerator.CreateConfigFrame(1, 1, new List<string> { signalName }, new List<string>());
            C37ConfigFrameEventArgs expectedFrame = new C37ConfigFrameEventArgs(configBytes);
            C37ConfigFrameEventArgs? raisedFrame = null;
            client.Object.ConfigFrameReceived += (sender, args) => { raisedFrame = args; };

            client.Raise(c => c.ConfigFrameReceived += null, this, expectedFrame);

            Assert.IsNotNull(raisedFrame);
            Assert.AreEqual(1, raisedFrame.Header.StreamIdCode);
            Assert.HasCount(2, raisedFrame.Blueprint.ChannelMap);
            Assert.IsNotNull(raisedFrame.Blueprint.ChannelMap.FirstOrDefault(n => n.Key == "FREQUENCY"));
            Assert.IsNotNull(raisedFrame.Blueprint.ChannelMap.FirstOrDefault(n => n.Key == signalName));
            Assert.HasCount(1, engine.Mappings);
            Assert.IsNotNull(engine.Mappings.FirstOrDefault(n => n.Register.Name == signalName));
        }


        [TestMethod]
        public async Task TransmissionEngineDataFrameWithOneAnalogTest()
        {
            string signalName = "ActivePower";
            float signalValue = 12;
            string signalFrequency = "FREQUENCY";
            float signalFrequencyValue = 50;
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            dataface.Register<C37TransmissionEngineTest, IC37Register>(r => r.Register<C37TransmissionEngineTest, float>(this, signalName, C37SignalType.Analog, (x, v) => x._analogValue = v));
            dataface.Register<C37TransmissionEngineTest, IC37Register>(r => r.Register<C37TransmissionEngineTest, float>(this, signalFrequency, C37SignalType.Frequency, (x, v) => x._frequencyValue = v));

            Mock<IC37Client> client = new Mock<IC37Client>();
            C37TransmissionEngine engine = new C37TransmissionEngine(NullLogger.Instance, client.Object, dataface);

            byte[] configBytes = C37DataGenerator.CreateConfigFrame(1, 1, new List<string>(), new List<string> { signalName });
            byte[] dataBytes = C37DataGenerator.CreateDataFrame(1, new List<(float Mag, float Ang)>(), new List<float> { signalValue }, signalFrequencyValue);
            C37ConfigFrameEventArgs expectedConfigFrame = new C37ConfigFrameEventArgs(configBytes);
            C37DataFrameEventArgs expectedDataFrame = new C37DataFrameEventArgs(dataBytes);
            C37DataFrameEventArgs? raisedDataFrame = null;
            client.Object.DataFrameReceived += (sender, args) => { raisedDataFrame = args; };

            client.Raise(c => c.ConfigFrameReceived += null, this, expectedConfigFrame);
            client.Raise(c => c.DataFrameReceived += null, this, expectedDataFrame);

            Assert.IsNotNull(raisedDataFrame);
            Assert.HasCount(2, engine.Mappings);
            Assert.IsNotNull(engine.Mappings.FirstOrDefault(n => n.Register.Name == signalName));
            Assert.IsNotNull(engine.Mappings.FirstOrDefault(n => n.Register.Name == signalFrequency));
            Assert.AreEqual(signalValue, _analogValue);
            Assert.AreEqual(signalFrequencyValue, _frequencyValue);
        }


        [TestMethod]
        public async Task TransmissionEngineDataFrameWithOnePhasorTest()
        {
            string signalName = "VoltageA";
            float signalValueMag = 11;
            float signalValueAng = 22;
            string signalFrequency = "FREQUENCY";
            float signalFrequencyValue = 50;
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            C37Register dataface = new C37Register("Test");
            // Remember magnitude and angle get stored into one value
            dataface.Register<C37TransmissionEngineTest, IC37Register>(r => r.Register<C37TransmissionEngineTest, ulong>(this, signalName, C37SignalType.Phasor, (x, v) => x._phasorValue = v));
            dataface.Register<C37TransmissionEngineTest, IC37Register>(r => r.Register<C37TransmissionEngineTest, float>(this, signalFrequency, C37SignalType.Frequency, (x, v) => x._frequencyValue = v));

            Mock<IC37Client> client = new Mock<IC37Client>();
            C37TransmissionEngine engine = new C37TransmissionEngine(NullLogger.Instance, client.Object, dataface);

            byte[] configBytes = C37DataGenerator.CreateConfigFrame(1, 1, new List<string> { signalName }, new List<string>());
            byte[] dataBytes = C37DataGenerator.CreateDataFrame(1, new List<(float Mag, float Ang)> { (signalValueMag, signalValueAng) }, new List<float>(), signalFrequencyValue);
            C37ConfigFrameEventArgs expectedConfigFrame = new C37ConfigFrameEventArgs(configBytes);
            C37DataFrameEventArgs expectedDataFrame = new C37DataFrameEventArgs(dataBytes);
            C37DataFrameEventArgs? raisedDataFrame = null;
            client.Object.DataFrameReceived += (sender, args) => { raisedDataFrame = args; };

            client.Raise(c => c.ConfigFrameReceived += null, this, expectedConfigFrame);
            client.Raise(c => c.DataFrameReceived += null, this, expectedDataFrame);

            Assert.IsNotNull(raisedDataFrame);
            Assert.HasCount(2, engine.Mappings);
            Assert.IsNotNull(engine.Mappings.FirstOrDefault(n => n.Register.Name == signalName));
            Assert.IsNotNull(engine.Mappings.FirstOrDefault(n => n.Register.Name == signalFrequency));
            // This calculation is taken from PowerMeterBase -> GetMagnitudeFromPhasorValue
            Assert.AreEqual(signalValueMag, BitConverter.UInt32BitsToSingle((uint)(_phasorValue >> 32)));
            // This calculation is taken from PowerMeterBase -> GetAngleFromPhasorValue
            Assert.AreEqual(signalValueAng, BitConverter.UInt32BitsToSingle((uint)(_phasorValue & 0xFFFFFFFF)));
            Assert.AreEqual(signalFrequencyValue, _frequencyValue);
        }

    }
}

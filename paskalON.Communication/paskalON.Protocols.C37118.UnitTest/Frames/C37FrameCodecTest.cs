// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Protocols.C37118.Frames;
using paskalON.Protocols.C37118.Simulations;

namespace paskalON.Protocols.C37118.UnitTest.Frames
{
    [TestClass]
    public class C37FrameCodecTest
    {
        [TestMethod]
        public void C37FrameCodecCreateDataFrameTest()
        {
            PmuDataSimulation simulation = new PmuDataSimulation
            {
                StreamId = 42,
                Frequency = 50.0f,
                FrequencyRateOfChange = -0.25f,
                Phasors = new[] { new PhasorMeasurement("VA", 0.25f, 12.5f, PhasorUnitTypes.Voltage) },
                Analogs = new[] { new AnalogMeasurement("P", 3.5f) }
            };

            byte[] bytes = C37FrameCodec.CreateDataFrame(simulation);
            C37DataFrame frame = new C37DataFrame(bytes, 1, 1, 0);

            Assert.AreEqual((ushort)42, frame.StreamIdCode);
            Assert.AreEqual((ushort)0, frame.Status);
            Assert.AreEqual(12.5d, frame.Phasors[0].Real, 0.001d);
            Assert.AreEqual(0.25d, frame.Phasors[0].Imaginary, 0.001d);
            Assert.AreEqual(50d, frame.Frequency, 0.001d);
            Assert.AreEqual(-0.25d, frame.RateOfChangeOfFrequency, 0.001d);
            Assert.AreEqual(3.5d, frame.Analogs[0], 0.001d);
        }


        [TestMethod]
        public void C37FrameCodecCreateConfigurationFrameTest()
        {
            PmuDataSimulation simulation = new PmuDataSimulation
            {
                StreamId = 7,
                Frequency = 50,
                Phasors = new[] { new PhasorMeasurement("VA", 0, 1, PhasorUnitTypes.Voltage) },
                Analogs = new[] { new AnalogMeasurement("P", 2) }
            };

            byte[] bytes = C37FrameCodec.CreateConfigurationFrame(new[] { simulation }, 30);
            C37ConfigFrameEventArgs frame = new C37ConfigFrameEventArgs(bytes);

            Assert.AreEqual((ushort)7, frame.Header.StreamIdCode);
            Assert.HasCount(1, frame.Blueprint.Pmus);
            Assert.AreEqual(1, frame.Blueprint.Pmus[0].NumberOfPhasors);
            Assert.AreEqual(1, frame.Blueprint.Pmus[0].NumberOfAnalogs);
        }


        [TestMethod]
        public void C37FrameCodecCreateCommandFrameTest()
        {
            byte[] bytes = C37FrameCodec.CreateCommandFrame(9, (ushort)C37CommandCode.TurnOn);
            C37CommandFrame frame = new C37CommandFrame(bytes);

            Assert.AreEqual((ushort)9, frame.StreamIdCode);
            Assert.AreEqual(C37CommandCode.TurnOn, frame.CommandCode);
            Assert.IsEmpty(frame.Parameters.ToArray());
        }


        [TestMethod]
        public void C37FrameCodecRejectsCorruptedChecksumTest()
        {
            PmuDataSimulation simulation = new PmuDataSimulation { StreamId = 1, Frequency = 50 };
            byte[] bytes = C37FrameCodec.CreateDataFrame(simulation);
            bytes[14] = 1;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new C37DataFrame(bytes, 0, 0, 0));
        }
    }
}

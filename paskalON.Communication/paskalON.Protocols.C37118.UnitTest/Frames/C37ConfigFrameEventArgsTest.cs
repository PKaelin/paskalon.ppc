// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Protocols.C37118.Frames;

namespace paskalON.Protocols.C37118.UnitTest.Frames
{
    [TestClass]
    public class C37ConfigFrameEventArgsTest
    {
        [TestMethod]
        public void CreateConfigFrameEventWithNoSignalsTest()
        {
            // In IEEE C37.118 standard a device count of 0 is structurally invalid for a configuration frame but test anyway.
            byte[] payload = C37TestDataGenerator.CreateMockConfigFrame(1, new List<string>(), new List<string>());
            C37ConfigFrameEventArgs configFrame = new C37ConfigFrameEventArgs(payload);

            Assert.IsNotNull(configFrame.Blueprint);
        }


        [TestMethod]
        public void CreateConfigFrameEventWithOnePhasorTest()
        {
            byte[] payload = C37TestDataGenerator.CreateMockConfigFrame(1, new List<string>(new[] { "Test" }), new List<string>());
            C37ConfigFrameEventArgs configFrame = new C37ConfigFrameEventArgs(payload);

            Assert.IsNotNull(configFrame.Blueprint);

            Assert.HasCount(1, configFrame.Blueprint.Pmus);
            Assert.AreEqual(1, configFrame.Blueprint.Pmus.First().NumberOfPhasors);
            Assert.AreEqual(0, configFrame.Blueprint.Pmus.First().NumberOfAnalogs);
            Assert.AreEqual(0, configFrame.Blueprint.Pmus.First().NumberOfDigitals);
            Assert.AreEqual(1, configFrame.Blueprint.Pmus.First().StationId);
            Assert.HasCount(2, configFrame.Blueprint.ChannelMap);
            Assert.IsNotNull(configFrame.Blueprint.ChannelMap.FirstOrDefault(n => n.Key == "Test"));
            Assert.IsNotNull(configFrame.Blueprint.ChannelMap.FirstOrDefault(n => n.Key == "FREQUENCY"));
        }


        [TestMethod]
        public void CreateConfigFrameEventWithOneAnalogTest()
        {
            byte[] payload = C37TestDataGenerator.CreateMockConfigFrame(1, new List<string>(), new List<string>(new[] { "Test" }));
            C37ConfigFrameEventArgs configFrame = new C37ConfigFrameEventArgs(payload);

            Assert.IsNotNull(configFrame.Blueprint);

            Assert.HasCount(1, configFrame.Blueprint.Pmus);
            Assert.AreEqual(0, configFrame.Blueprint.Pmus.First().NumberOfPhasors);
            Assert.AreEqual(1, configFrame.Blueprint.Pmus.First().NumberOfAnalogs);
            Assert.AreEqual(0, configFrame.Blueprint.Pmus.First().NumberOfDigitals);
            Assert.AreEqual(1, configFrame.Blueprint.Pmus.First().StationId);
            Assert.HasCount(2, configFrame.Blueprint.ChannelMap);
            Assert.IsNotNull(configFrame.Blueprint.ChannelMap.FirstOrDefault(n => n.Key == "Test"));
            Assert.IsNotNull(configFrame.Blueprint.ChannelMap.FirstOrDefault(n => n.Key == "FREQUENCY"));
        }

    }
}

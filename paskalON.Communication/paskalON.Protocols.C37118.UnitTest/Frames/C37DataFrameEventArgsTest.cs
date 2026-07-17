// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Protocols.C37118.Frames;
using paskalON.Protocols.C37118.Generators;

namespace paskalON.Protocols.C37118.UnitTest.Frames
{
    [TestClass]
    public class C37DataFrameEventArgsTest
    {
        [TestMethod]
        public void CreateDataFrameEventWithNoDataTest()
        {
            byte[] data = C37DataGenerator.CreateDataFrame(1, new List<(float Mag, float Ang)>(), new List<float>(), 0);
            C37DataFrameEventArgs dataFrame = new C37DataFrameEventArgs(data);

            Assert.AreEqual(0, dataFrame.RawPayload.Length);
        }


        [TestMethod]
        public void CreateDataFrameEventWithOnePhasorTest()
        {
            byte[] data = C37DataGenerator.CreateDataFrame(1, new List<(float Mag, float Ang)> { (1, 2) }, new List<float>(), 10);
            C37DataFrameEventArgs dataFrame = new C37DataFrameEventArgs(data);

            Assert.IsGreaterThan(0, dataFrame.RawPayload.Length);
        }


        [TestMethod]
        public void CreateDataFrameEventWithOneAnalogTest()
        {
            byte[] data = C37DataGenerator.CreateDataFrame(1, new List<(float Mag, float Ang)>(), new List<float> { 1 }, 10);
            C37DataFrameEventArgs dataFrame = new C37DataFrameEventArgs(data);

            Assert.IsGreaterThan(0, dataFrame.RawPayload.Length);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Configs.UnitTest
{
    [TestClass]
    public class C37ConfigTest
    {
        [TestMethod]
        [DataRow((ushort)1)]
        [DataRow((ushort)4712)]
        [DataRow(ushort.MaxValue)]
        public void StreamIdValidTest(ushort streamId)
        {
            C37Config config = TestConfigFactory.CreateC37Config();

            config.StreamId = streamId;

            Assert.AreEqual(streamId, config.StreamId);
        }


        [TestMethod]
        public void StreamIdZeroTest()
        {
            C37Config config = TestConfigFactory.CreateC37Config();

            Action set = () => config.StreamId = 0;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
            Assert.AreEqual((ushort)1, config.StreamId);
        }


        [TestMethod]
        [DataRow((ushort)1)]
        [DataRow((ushort)4712)]
        [DataRow(ushort.MaxValue)]
        public void PortValidTest(ushort port)
        {
            C37Config config = TestConfigFactory.CreateC37Config();

            config.Port = port;

            Assert.AreEqual(port, config.Port);
        }


        [TestMethod]
        public void PortZeroTest()
        {
            C37Config config = TestConfigFactory.CreateC37Config();

            Action set = () => config.Port = 0;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
        }


        [TestMethod]
        public void AddressNullTest()
        {
            C37Config config = TestConfigFactory.CreateC37Config();

            Action set = () => config.Address = null!;

            Assert.ThrowsExactly<ArgumentNullException>(set);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\t")]
        public void AddressEmptyOrWhiteSpaceTest(string address)
        {
            C37Config config = TestConfigFactory.CreateC37Config();

            Action set = () => config.Address = address;

            Assert.ThrowsExactly<ArgumentException>(set);
            Assert.AreEqual(Constants.Ip4Localhost, config.Address);
        }


        [TestMethod]
        [DataRow(nameof(C37Config.ConfigFrameTimeoutMilliseconds), 0)]
        [DataRow(nameof(C37Config.DataFrameTimeoutMilliseconds), 0)]
        [DataRow(nameof(C37Config.ConfigFrameTimeoutMilliseconds), int.MaxValue)]
        public void TimeoutAtLimitTest(string propertyName, int timeout)
        {
            C37Config config = TestConfigFactory.CreateC37Config();

            PropertyAccessor.Set(config, propertyName, timeout);

            Assert.AreEqual(timeout, PropertyAccessor.Get(config, propertyName));
        }


        [TestMethod]
        [DataRow(nameof(C37Config.ConfigFrameTimeoutMilliseconds))]
        [DataRow(nameof(C37Config.DataFrameTimeoutMilliseconds))]
        public void TimeoutNegativeTest(string propertyName)
        {
            C37Config config = TestConfigFactory.CreateC37Config();
            object? original = PropertyAccessor.Get(config, propertyName);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PropertyAccessor.Set(config, propertyName, -1));

            Assert.AreEqual(original, PropertyAccessor.Get(config, propertyName));
        }


        [TestMethod]
        public void ToStringTest()
        {
            C37Config config = TestConfigFactory.CreateC37Config();

            string text = config.ToString();

            Assert.AreEqual($"PMU 1 {Constants.Ip4Localhost}:{Constants.PortStartMeter}", text);
        }
    }
}

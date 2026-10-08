// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Configs.UnitTest
{
    [TestClass]
    public class ModbusConfigTest
    {
        [TestMethod]
        [DataRow((ushort)1)]
        [DataRow((ushort)502)]
        [DataRow(ushort.MaxValue)]
        public void PortValidTest(ushort port)
        {
            ModbusConfig config = TestConfigFactory.CreateModbusConfig();

            config.Port = port;

            Assert.AreEqual(port, config.Port);
        }


        [TestMethod]
        public void PortZeroTest()
        {
            ModbusConfig config = TestConfigFactory.CreateModbusConfig();

            Action set = () => config.Port = 0;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(set);
            Assert.AreEqual((ushort)Constants.PortStartPcs, config.Port);
        }


        [TestMethod]
        public void AddressNullTest()
        {
            ModbusConfig config = TestConfigFactory.CreateModbusConfig();

            Action set = () => config.Address = null!;

            Assert.ThrowsExactly<ArgumentNullException>(set);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\t")]
        public void AddressEmptyOrWhiteSpaceTest(string address)
        {
            ModbusConfig config = TestConfigFactory.CreateModbusConfig();

            Action set = () => config.Address = address;

            Assert.ThrowsExactly<ArgumentException>(set);
            Assert.AreEqual(Constants.Ip4Localhost, config.Address);
        }


        [TestMethod]
        public void ToStringTest()
        {
            ModbusConfig config = TestConfigFactory.CreateModbusConfig();

            string text = config.ToString();

            Assert.AreEqual($"Modbus PCS 1 {Constants.Ip4Localhost}:{Constants.PortStartPcs}", text);
        }
    }
}

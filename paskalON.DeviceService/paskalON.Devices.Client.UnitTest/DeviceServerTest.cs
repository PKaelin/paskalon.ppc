// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Client.UnitTest
{
    [TestClass]
    public sealed class DeviceServerTest
    {
        [TestMethod]
        public void DeviceServerNullClientTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceServer(null!));
        }


        [TestMethod]
        public void DeviceServerClientWithoutBaseAddressTest()
        {
            using HttpClient client = new HttpClient();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceServer(client));
        }
    }
}

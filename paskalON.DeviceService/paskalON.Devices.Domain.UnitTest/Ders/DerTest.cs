// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.UnitTest.Ders
{
    [TestClass]
    public class DerTest
    {
        [TestMethod]
        public void CreateWithoutConfigTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new Der(NullLogger<Der>.Instance, null!));
        }


        [TestMethod]
        public void CreateWithNullNameConfigTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = null! };
            Assert.ThrowsExactly<ArgumentNullException>(() => new Der(NullLogger<Der>.Instance, derConfig));
        }


        [TestMethod]
        public void CreateWithEmptyNameConfigTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = string.Empty };
            Assert.ThrowsExactly<ArgumentException>(() => new Der(NullLogger<Der>.Instance, derConfig));
        }


        [TestMethod]
        public void CreateDerTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = "Der" };
            Der der = new Der(NullLogger<Der>.Instance, derConfig);

            Assert.IsNotNull(der.AuxiliaryPowerMeters);
            Assert.IsNotNull(der.DerGroups);
            Assert.IsNotNull(der.SystemPowerMeters);
            Assert.IsNotNull(der.ExternalPowerMeters);
            Assert.IsNotNull(der.GenericModbusDevices);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new Der(NullLogger<Der>.Instance, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void CreateWithNullNameConfigTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = null };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => new Der(NullLogger<Der>.Instance, derConfig));
        }


        [TestMethod]
        public void CreateWithEmptyNameConfigTest()
        {
            DerConfig derConfig = new DerConfig { ChangedBy = "Test", Name = string.Empty };
            Assert.ThrowsExactly<ArgumentException>(() => new Der(NullLogger<Der>.Instance, derConfig));
        }
    }
}

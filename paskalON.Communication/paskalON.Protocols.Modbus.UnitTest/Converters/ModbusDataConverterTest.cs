// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Converters;

namespace paskalON.Protocols.Modbus.UnitTest.Converters
{
    [TestClass]
    public sealed class ModbusDataConverterTest
    {

        [TestMethod]
        [DataRow((Int16)10, 1, (Int16)10, typeof(Int16))]
        [DataRow(10, 1, 10, typeof(int))]
        [DataRow(10, 2, (double)20, typeof(double))]
        [DataRow(10, 0.5, (double)5, typeof(double))]
        [DataRow((uint)10, 1, (uint)10, typeof(uint))]
        [DataRow((uint)10, 0.5, (double)5, typeof(double))]
        [DataRow((double)10, 1, (double)10, typeof(double))]
        [DataRow((double)20, 0.5, (double)10, typeof(double))]
        [DataRow((UInt64)10, 1, (UInt64)10, typeof(UInt64))]
        [DataRow((UInt64)10, 2, (double)20, typeof(double))]
        public void ApplyScaleTest(object value, double scale, object expected, Type type)
        {
            ModbusDataConverter converter = new ModbusDataConverter();
            object result = converter.ApplyScale(value, scale);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(type, result.GetType());
        }


        [TestMethod]
        [DataRow(ModbusDataType.MbInt16, false)]
        [DataRow(ModbusDataType.MbInt32Be, true)]
        [DataRow(ModbusDataType.MbFloatBe, true)]
        [DataRow(ModbusDataType.MbDoubleBe, true)]
        [DataRow(ModbusDataType.MbInt32M10KBe, true)]
        [DataRow(ModbusDataType.MbUint32Be, true)]
        [DataRow(ModbusDataType.MbUint64Be, true)]
        [DataRow(ModbusDataType.MbPackedBool32Be, true)]
        [DataRow(ModbusDataType.MbBool, false)]
        [DataRow(ModbusDataType.MbDoubleLe, false)]
        [DataRow(ModbusDataType.MbFloatLe, false)]
        [DataRow(ModbusDataType.MbInt32Le, false)]
        [DataRow(ModbusDataType.MbInt32M10KLe, false)]
        [DataRow(ModbusDataType.MbInt64Le, false)]
        [DataRow(ModbusDataType.MbPackedBool16, false)]
        public void IsBigEndianTest(ModbusDataType type, bool expected)
        {
            ModbusDataConverter converter = new ModbusDataConverter();
            bool result = converter.IsBigEndian(type);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        [DataRow(ModbusDataType.MbBool, 1)]
        [DataRow(ModbusDataType.MbInt16, 1)]
        [DataRow(ModbusDataType.MbUint16, 1)]
        [DataRow(ModbusDataType.MbPackedBool16, 1)]
        [DataRow(ModbusDataType.MbFloatBe, 2)]
        [DataRow(ModbusDataType.MbFloatLe, 2)]
        [DataRow(ModbusDataType.MbInt32Be, 2)]
        [DataRow(ModbusDataType.MbInt32Le, 2)]
        [DataRow(ModbusDataType.MbInt32M10KBe, 2)]
        [DataRow(ModbusDataType.MbInt32M10KLe, 2)]
        [DataRow(ModbusDataType.MbPackedBool32Be, 2)]
        [DataRow(ModbusDataType.MbPackedBool32Le, 2)]
        [DataRow(ModbusDataType.MbUint32Be, 2)]
        [DataRow(ModbusDataType.MbUint32Le, 2)]
        [DataRow(ModbusDataType.MbUint32M10KBe, 2)]
        [DataRow(ModbusDataType.MbUint32M10KLe, 2)]
        [DataRow(ModbusDataType.MbDoubleBe, 4)]
        [DataRow(ModbusDataType.MbDoubleLe, 4)]
        [DataRow(ModbusDataType.MbInt64Be, 4)]
        [DataRow(ModbusDataType.MbInt64Le, 4)]
        [DataRow(ModbusDataType.MbUint64Be, 4)]
        [DataRow(ModbusDataType.MbUint64Le, 4)]
        public void GetRegisterLengthTest(ModbusDataType type, int expected)
        {
            ModbusDataConverter converter = new ModbusDataConverter();
            int result = converter.GetRegisterLength(type);

            Assert.AreEqual(expected, result);
        }


        // Pass in the bytes as little endian.
        [TestMethod]
        [DataRow(new byte[] { 57, 48 }, ModbusDataType.MbInt16, (Int16)12345, typeof(Int16))]
        [DataRow(new byte[] { 57, 48 }, ModbusDataType.MbUint16, (UInt16)12345, typeof(UInt16))]
        [DataRow(new byte[] { 57, 48, 0, 0 }, ModbusDataType.MbInt32Be, 12345, typeof(int))]
        [DataRow(new byte[] { 57, 48, 0, 0 }, ModbusDataType.MbInt32Le, 12345, typeof(int))]
        [DataRow(new byte[] { 0, 228, 64, 70 }, ModbusDataType.MbFloatBe, 12345f, typeof(float))]
        [DataRow(new byte[] { 0, 228, 64, 70 }, ModbusDataType.MbFloatLe, 12345f, typeof(float))]
        [DataRow(new byte[] { 57, 48, 0, 0, 0, 0, 0, 0 }, ModbusDataType.MbInt64Le, (Int64)12345, typeof(Int64))]
        [DataRow(new byte[] { 57, 48, 0, 0, 0, 0, 0, 0 }, ModbusDataType.MbInt64Be, (Int64)12345, typeof(Int64))]
        [DataRow(new byte[] { 57, 48, 0, 0, 0, 0, 0, 0 }, ModbusDataType.MbUint64Le, (UInt64)12345, typeof(UInt64))]
        [DataRow(new byte[] { 57, 48, 0, 0, 0, 0, 0, 0 }, ModbusDataType.MbUint64Be, (UInt64)12345, typeof(UInt64))]
        [DataRow(new byte[] { 0, 0, 0, 0, 128, 28, 200, 64 }, ModbusDataType.MbDoubleBe, 12345D, typeof(double))]
        [DataRow(new byte[] { 0, 0, 0, 0, 128, 28, 200, 64 }, ModbusDataType.MbDoubleLe, 12345D, typeof(double))]
        [DataRow(new byte[] { 51, 51, 51, 51, 211, 28, 200, 64 }, ModbusDataType.MbDoubleLe, 12345.65D, typeof(double))]
        public void ConvertBytesToTypeTest(byte[] bytes, ModbusDataType type, object expected, Type expectedType)
        {
            IEnumerable<byte> endianBytes = bytes;

            // Reverse the order depending on the system.
            if (BitConverter.IsLittleEndian == false)
            {
                endianBytes = bytes.Reverse();
            }

            ModbusDataConverter converter = new ModbusDataConverter();
            object result = converter.ConvertBytesToType(endianBytes.ToArray(), type);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(expectedType, result.GetType());
        }


        [TestMethod]
        [DataRow(12345, ModbusDataType.MbInt16, 1, new ushort[] { 12345 })]
        [DataRow(12345, ModbusDataType.MbInt32Be, 1, new ushort[] { 0, 14640 })]
        [DataRow(12345, ModbusDataType.MbInt32Le, 1, new ushort[] { 12345, 0 })]
        [DataRow(12345, ModbusDataType.MbUint32Be, 1, new ushort[] { 0, 14640 })]
        [DataRow(12345, ModbusDataType.MbUint32Le, 1, new ushort[] { 12345, 0 })]
        [DataRow(12345, ModbusDataType.MbFloatBe, 1, new ushort[] { 16454, 228 })]
        [DataRow(12345, ModbusDataType.MbFloatLe, 1, new ushort[] { 58368, 17984 })]
        [DataRow(12345, ModbusDataType.MbDoubleBe, 1, new ushort[] { 51264, 32796, 0, 0 })]
        [DataRow(12345, ModbusDataType.MbDoubleLe, 1, new ushort[] { 0, 0, 7296, 16584 })]
        [DataRow(12345, ModbusDataType.MbUint64Be, 1, new ushort[] { 0, 0, 0, 14640 })]
        [DataRow(12345, ModbusDataType.MbUint64Le, 1, new ushort[] { 12345, 0, 0, 0 })]
        public void RegisterArrayFromValueTest(double value, ModbusDataType type, double scale, ushort[] expected)
        {
            ModbusDataConverter converter = new ModbusDataConverter();
            ushort[] result = converter.RegisterArrayFromValue(value, type, scale);

            Assert.IsNotNull(result);
            Assert.HasCount(expected.Count(), result);
            CollectionAssert.AreEqual(expected, result);
        }


        // Expected values are in little endian
        [TestMethod]
        [DataRow(new ushort[] { 12345 }, ModbusDataType.MbInt16, new byte[] { 57, 48 })]
        [DataRow(new ushort[] { 0, 12345 }, ModbusDataType.MbInt32Be, new byte[] { 57, 48, 0, 0 })]
        [DataRow(new ushort[] { 12345, 0 }, ModbusDataType.MbInt32Le, new byte[] { 57, 48, 0, 0 })]
        [DataRow(new ushort[] { 17984, 58368 }, ModbusDataType.MbFloatBe, new byte[] { 0, 228, 64, 70 })]
        [DataRow(new ushort[] { 58368, 17984 }, ModbusDataType.MbFloatLe, new byte[] { 0, 228, 64, 70 })]
        [DataRow(new ushort[] { 16531, 19005, 28835, 55050 }, ModbusDataType.MbDoubleBe, new byte[] { 10, 215, 163, 112, 61, 74, 147, 64 })]
        [DataRow(new ushort[] { 55050, 28835, 19005, 16531 }, ModbusDataType.MbDoubleLe, new byte[] { 10, 215, 163, 112, 61, 74, 147, 64 })]
        public void ConvertToByteArrayTest(ushort[] registers, ModbusDataType type, byte[] expected)
        {
            // Reverse the order depending on the system.
            if (BitConverter.IsLittleEndian == false)
            {
                Array.Reverse(expected);
            }

            ModbusDataConverter converter = new ModbusDataConverter();
            byte[] result = converter.ConvertToByteArray(registers, type);

            CollectionAssert.AreEqual(expected, result);
        }


        [TestMethod]
        public void ConvertRawDataIndexBiggerThanDataLenghtTest()
        {
            bool[] rawData = new bool[] { true, false };
            ModbusRegisterEntry<ModbusDataConverterTest, bool> entry = new ModbusRegisterEntry<ModbusDataConverterTest, bool>(this, "T", (x, v) => { }, 1000, 1, ModbusDataType.MbBool, 0);
            ModbusDataConverter converter = new ModbusDataConverter();
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => converter.ConvertRawData(rawData, entry, 100));
        }


        [TestMethod]
        public void ConvertRawDataStartAddressNegativeTest()
        {
            bool[] rawData = new bool[] { true, false };
            ModbusRegisterEntry<ModbusDataConverterTest, bool> entry = new ModbusRegisterEntry<ModbusDataConverterTest, bool>(this, "T", (x, v) => { }, 1, 1, ModbusDataType.MbBool, 0);
            ModbusDataConverter converter = new ModbusDataConverter();
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => converter.ConvertRawData(rawData, entry, 100));
        }


        [TestMethod]
        [DataRow(new bool[] { true }, (ushort)1000, (ushort)1000, true)]
        [DataRow(new bool[] { true, false }, (ushort)1000, (ushort)1000, true)]
        [DataRow(new bool[] { true, false }, (ushort)1001, (ushort)1000, false)]
        public void ConvertRawDataTest(bool[] rawData, ushort register, ushort startAddress, bool expected)
        {
            ModbusRegisterEntry<ModbusDataConverterTest, bool> entry = new ModbusRegisterEntry<ModbusDataConverterTest, bool>(this, "T", (x, v) => { }, register, 1, ModbusDataType.MbBool, 0);
            ModbusDataConverter converter = new ModbusDataConverter();
            bool result = converter.ConvertRawData(rawData, entry, startAddress);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        [DataRow(new ushort[] { 12345 }, ModbusDataType.MbInt16, (ushort)10, 1, (ushort)10, (Int16)12345)]
        [DataRow(new ushort[] { 0, 12345 }, ModbusDataType.MbInt32Be, (ushort)10, 1, (ushort)10, 12345)]
        [DataRow(new ushort[] { 12345, 0 }, ModbusDataType.MbInt32Le, (ushort)10, 1, (ushort)10, 12345)]
        [DataRow(new ushort[] { 17984, 58368 }, ModbusDataType.MbFloatBe, (ushort)10, 1, (ushort)10, (float)12345)]
        [DataRow(new ushort[] { 58368, 17984 }, ModbusDataType.MbFloatLe, (ushort)10, 1, (ushort)10, (float)12345)]
        [DataRow(new ushort[] { 16531, 19005, 28835, 55050 }, ModbusDataType.MbDoubleBe, (ushort)10, 1, (ushort)10, (double)1234.56)]
        [DataRow(new ushort[] { 55050, 28835, 19005, 16531 }, ModbusDataType.MbDoubleLe, (ushort)10, 1, (ushort)10, (double)1234.56)]
        public void ConvertRawDataTest(ushort[] rawData, ModbusDataType type, ushort register, double scale, ushort startAddress, object expected)
        {
            ModbusRegisterEntry<ModbusDataConverterTest, bool> entry = new ModbusRegisterEntry<ModbusDataConverterTest, bool>(this, "T", (x, v) => { }, register, scale, type, 0);
            ModbusDataConverter converter = new ModbusDataConverter();

            object? result = converter.ConvertRawData(rawData, entry, startAddress);

            Assert.AreEqual(expected, result);
        }
    }
}
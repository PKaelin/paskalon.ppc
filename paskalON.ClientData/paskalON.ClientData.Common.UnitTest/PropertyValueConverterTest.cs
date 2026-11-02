// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common.UnitTest
{
    /// <summary>
    /// Tests of the class <see cref="PropertyValueConverter"/>.
    /// </summary>
    [TestClass]
    public class PropertyValueConverterTest
    {
        [TestMethod]
        [DataRow("Battery01", typeof(string), "Battery01")]
        [DataRow("100", typeof(int), 100)]
        [DataRow("100", typeof(int?), 100)]
        [DataRow("3.14", typeof(double), 3.14)]
        [DataRow("true", typeof(bool), true)]
        public void ConvertReturnsScalarTest(string value, Type type, object expected)
        {
            object result = PropertyValueConverter.Convert(value, type);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void ConvertReturnsDateTimeOffsetTest()
        {
            DateTimeOffset expected = new DateTimeOffset(2026, 11, 1, 10, 0, 0, TimeSpan.Zero);

            object result = PropertyValueConverter.Convert("2026-11-01T10:00:00Z", typeof(DateTimeOffset));

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void ConvertReturnsGuidTest()
        {
            Guid expected = new Guid("7d9f8d2e-0d5b-4b0c-9a55-0f4f6a1a2b3c");

            object result = PropertyValueConverter.Convert("7d9f8d2e-0d5b-4b0c-9a55-0f4f6a1a2b3c", typeof(Guid));

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void ConvertReturnsEnumTest()
        {
            object result = PropertyValueConverter.Convert("Monday", typeof(DayOfWeek?));

            Assert.AreEqual(DayOfWeek.Monday, result);
        }


        [TestMethod]
        [DataRow("abc", typeof(int))]
        [DataRow("70000", typeof(ushort))]
        [DataRow("300", typeof(byte))]
        [DataRow("yes", typeof(bool))]
        [DataRow("1", typeof(DayOfWeek))]
        [DataRow("monday", typeof(DayOfWeek))]
        [DataRow("1", typeof(TimeSpan))]
        public void ConvertRejectsInvalidValueTest(string value, Type type)
        {
            Action action = () => PropertyValueConverter.Convert(value, type);

            Assert.ThrowsExactly<FormatException>(action);
        }
    }
}

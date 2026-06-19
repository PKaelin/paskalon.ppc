using paskalON.PhysicalUnits.Thermals;

namespace paskalON.PhysicalUnits.UnitTest.Thermals
{
    [TestClass]
    public class TemperatureTest
    {
        [TestMethod]
        public void TemperatureConstructorTest()
        {
            Temperature temperature = new Temperature(TemperatureUnit.Celsius, 12.345678901);
            Assert.AreEqual(12.34568, temperature.CelsiusPrecision);
        }


        [TestMethod]
        public void TemperatureCelciusToFahrenheitTest()
        {
            Temperature temperature = new Temperature(TemperatureUnit.Celsius, 12.3456);
            Assert.AreEqual(54.22208, temperature.FahrenheitPrecision);
        }


        [TestMethod]
        public void TemperatureFahrenheitToCelciusTest()
        {
            Temperature temperature = new Temperature(TemperatureUnit.Fahrenheit, 36.897);
            Assert.AreEqual(2.72056, temperature.CelsiusPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void TemperatureEqualsTest(double temperature1, double temperature2, bool expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Assert.AreEqual(expected, rp1.Equals(rp2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void TemperatureEqualsObjectTest(double temperature1, double temperature2, bool expected)
        {
            object rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            object rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Assert.AreEqual(expected, rp1.Equals(rp2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void TemperatureEqualsOperatorTest(double temperature1, double temperature2, bool expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Assert.AreEqual(expected, rp1 == rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, false)]
        [DataRow(12.34567, 12.34567111, false)]
        [DataRow(12.34567, 12.34567890, true)]
        [DataRow(12.34567, 12.34544, true)]
        public void TemperatureNotEqualsOperatorTest(double temperature1, double temperature2, bool expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Assert.AreEqual(expected, rp1 != rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 24.69134)]
        [DataRow(12.34567, 12.34567111, 24.69134)]
        [DataRow(12.34567, 12.34567890, 24.69135)]
        [DataRow(12.34567, 12.34544, 24.69111)]
        public void TemperatureAddOperatorTest(double temperature1, double temperature2, double expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Temperature power = rp1 + rp2;
            Assert.AreEqual(expected, power.CelsiusPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 0)]
        [DataRow(12.34567, 12.34567111, 0)]
        [DataRow(12.34567, 12.34567890, -0.00001)]
        [DataRow(12.34567, 12.34544, 0.00023)]
        public void TemperatureMinusOperatorTest(double temperature1, double temperature2, double expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Temperature power = rp1 - rp2;
            Assert.AreEqual(expected, power.CelsiusPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 152.41557)]
        [DataRow(12.34567, 12.34567111, 152.41558)]
        [DataRow(12.34567, 12.34567890, 152.41568)]
        [DataRow(12.34567, 12.34544, 152.41273)]
        public void TemperatureMultiplyOperatorTest(double temperature1, double temperature2, double expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Temperature power = rp1 * rp2;
            Assert.AreEqual(expected, power.CelsiusPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, 6.17284)]
        [DataRow(12.34567, 3, 4.11522)]
        [DataRow(12.34567, 2.5, 4.93827)]
        [DataRow(12.34567, 12, 1.02881)]
        [DataRow(12.34567, 0, double.NaN)]
        public void TemperatureDivisionOperatorTest(double temperature1, double temperature2, double expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Temperature power = rp1 / rp2;
            Assert.AreEqual(expected, power.CelsiusPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void TemperatureSmallerOperatorTest(double temperature1, double temperature2, bool expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Assert.AreEqual(expected, rp1 < rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void TemperatureSmallerEqualOperatorTest(double temperature1, double temperature2, bool expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Assert.AreEqual(expected, rp1 <= rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void TemperatureGreaterOperatorTest(double temperature1, double temperature2, bool expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Assert.AreEqual(expected, rp1 > rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void TemperatureGreaterEqualOperatorTest(double temperature1, double temperature2, bool expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Temperature rp2 = new Temperature(TemperatureUnit.Celsius, temperature2);
            Assert.AreEqual(expected, rp1 >= rp2);
        }


        [TestMethod]
        [DataRow(double.NaN, true)]
        [DataRow(12.34567, false)]
        [DataRow(double.MinValue, false)]
        [DataRow(double.MaxValue, false)]
        public void TemperatureIsNaNOperatorTest(double temperature1, bool expected)
        {
            Temperature rp1 = new Temperature(TemperatureUnit.Celsius, temperature1);
            Assert.AreEqual(expected, rp1.IsNaN());
        }

    }
}

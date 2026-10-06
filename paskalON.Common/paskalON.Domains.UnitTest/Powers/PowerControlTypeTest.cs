// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.Configs;
using System.Numerics;

namespace paskalON.Domains.UnitTest.Powers
{
    [TestClass]
    public sealed class PowerControlTypeTest
    {
        [TestMethod]
        public void PowerControlTypeIsFlagsEnumTest()
        {
            bool hasFlagsAttribute = typeof(PowerControlType).IsDefined(typeof(FlagsAttribute), false);

            Assert.IsTrue(hasFlagsAttribute);
        }


        [TestMethod]
        public void PowerControlTypeDefinedValuesAreDistinctSingleBitsTest()
        {
            PowerControlType[] values = Enum.GetValues<PowerControlType>()
                .Where(x => x != PowerControlType.None)
                .ToArray();

            int[] bitCounts = values.Select(x => BitOperations.PopCount((uint)x)).ToArray();
            int combined = values.Aggregate(0, (current, x) => current | (int)x);
            int sum = values.Sum(x => (int)x);

            Assert.HasCount(6, values);
            Assert.IsTrue(bitCounts.All(x => x == 1));
            Assert.AreEqual(sum, combined);
            Assert.AreEqual(0x3F, combined);
        }


        [TestMethod]
        public void PowerControlTypeCombinedFlagsContainOnlySelectedTypesTest()
        {
            PowerControlType combined = PowerControlType.Bess | PowerControlType.Solar;

            bool hasBess = combined.HasFlag(PowerControlType.Bess);
            bool hasSolar = combined.HasFlag(PowerControlType.Solar);
            bool hasWind = combined.HasFlag(PowerControlType.Wind);

            Assert.IsTrue(hasBess);
            Assert.IsTrue(hasSolar);
            Assert.IsFalse(hasWind);
            Assert.AreEqual(0x03, (int)combined);
        }


        [TestMethod]
        public void PowerControlTypeNoneIntersectionTest()
        {
            PowerControlType renewable = PowerControlType.Solar | PowerControlType.Wind | PowerControlType.Hydro;
            PowerControlType fossilAndNuclear = PowerControlType.Gas | PowerControlType.Nuclear;

            PowerControlType intersection = renewable & fossilAndNuclear;

            Assert.AreEqual(PowerControlType.None, intersection);
        }


        [TestMethod]
        [DataRow(PowerControlType.None, "None")]
        [DataRow(PowerControlType.Bess, "Bess")]
        [DataRow(PowerControlType.Bess | PowerControlType.Solar, "Bess, Solar")]
        [DataRow(PowerControlType.Bess | PowerControlType.Solar | PowerControlType.Wind | PowerControlType.Nuclear | PowerControlType.Hydro | PowerControlType.Gas, "Bess, Solar, Wind, Nuclear, Hydro, Gas")]
        public void PowerControlTypeToStringTest(PowerControlType type, string expected)
        {
            string text = type.ToString();

            Assert.AreEqual(expected, text);
        }


        [TestMethod]
        [DataRow("Bess, Solar", PowerControlType.Bess | PowerControlType.Solar)]
        [DataRow("gas,hydro", PowerControlType.Gas | PowerControlType.Hydro)]
        [DataRow("None", PowerControlType.None)]
        public void PowerControlTypeParseTest(string text, PowerControlType expected)
        {
            PowerControlType parsed = Enum.Parse<PowerControlType>(text, ignoreCase: true);

            Assert.AreEqual(expected, parsed);
        }


        [TestMethod]
        public void PowerControlTypeParseUnknownNameThrowsTest()
        {
            string text = "Coal";

            Action action = () => Enum.Parse<PowerControlType>(text);

            Assert.ThrowsExactly<ArgumentException>(action);
        }


        [TestMethod]
        public void PowerControlTypeUndefinedBitIsNotDefinedTest()
        {
            PowerControlType undefined = (PowerControlType)0x40;

            bool isDefined = Enum.IsDefined(undefined);

            Assert.IsFalse(isDefined);
        }
    }
}

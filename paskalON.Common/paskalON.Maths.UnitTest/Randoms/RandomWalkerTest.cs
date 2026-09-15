// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Maths.Randoms;

namespace paskalON.Maths.UnitTest.Randoms
{
    [TestClass]
    public class RandomWalkerTest
    {
        [TestMethod]
        public void RandomWalkerOutOfBoundsLowerTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new RandomWalker<int>(10, 1, 20, 30, int.MaxValue));
        }


        [TestMethod]
        public void RandomWalkerOutOfBoundsUpperTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new RandomWalker<int>(40, 1, 20, 30, int.MaxValue));
        }


        [TestMethod]
        public void RandomWalkerNegativePrecisionTest()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RandomWalker<int>(25, 1, 20, 30, int.MaxValue, -1));
        }


        [TestMethod]
        public void RandomWalkerConstructorTest()
        {
            RandomWalker<int> walker = new RandomWalker<int>(25, 1, 20, 30, int.MaxValue);

            Assert.AreEqual(25, walker.Home);
            Assert.AreEqual(1, walker.StepSize);
            Assert.AreEqual(20, walker.LowerBound);
            Assert.AreEqual(30, walker.UpperBound);
            Assert.AreEqual(25, walker.CurrentValue);
        }


        [TestMethod]
        public void RandomWalkerStayTheSameTest()
        {
            RandomWalker<int> walker = new RandomWalker<int>(20, 1, 20, 20, int.MaxValue);

            Assert.AreEqual(20, walker.Next());
            Assert.AreEqual(20, walker.Next());
            Assert.AreEqual(20, walker.Next());
            Assert.AreEqual(20, walker.Next());
            Assert.AreEqual(20, walker.Next());
            Assert.AreEqual(20, walker.Next());
        }


        [TestMethod]
        public void RandomWalkerStayWithinOneTest()
        {
            RandomWalker<int> walker = new RandomWalker<int>(20, 1, 19, 21, int.MaxValue);
            int[] expected = new int[] { 20, 19, 21 };

            CollectionAssert.Contains(expected, walker.Next());
            CollectionAssert.Contains(expected, walker.Next());
            CollectionAssert.Contains(expected, walker.Next());
            CollectionAssert.Contains(expected, walker.Next());
            CollectionAssert.Contains(expected, walker.Next());
            CollectionAssert.Contains(expected, walker.Next());
        }


        [TestMethod]
        public void RandomWalkerStayWithinDistanceTest()
        {
            RandomWalker<int> walker = new RandomWalker<int>(20, 1, 10, 30, 1);

            int previous = walker.CurrentValue;
            int val = walker.Next();

            Assert.IsTrue(val >= previous - 1 && val <= previous + 1);
            previous = val;
            val = walker.Next();
            Assert.IsTrue(val >= previous - 1 && val <= previous + 1);
            previous = val;
            val = walker.Next();
            Assert.IsTrue(val >= previous - 1 && val <= previous + 1);
            previous = val;
            val = walker.Next();
            Assert.IsTrue(val >= previous - 1 && val <= previous + 1);
            previous = val;
            val = walker.Next();
            Assert.IsTrue(val >= previous - 1 && val <= previous + 1);
            previous = val;
            val = walker.Next();
            Assert.IsTrue(val >= previous - 1 && val <= previous + 1);
            previous = val;
            val = walker.Next();
        }


        [TestMethod]
        public void RandomWalkerStayWithinOneDoubleTest()
        {
            RandomWalker<double> walker = new RandomWalker<double>(20, 0.1, 19, 21, int.MaxValue);

            double val = walker.Next();
            Assert.IsTrue(val >= 19 && val <= 21);
            val = walker.Next();
            Assert.IsTrue(val >= 19 && val <= 21);
            val = walker.Next();
            Assert.IsTrue(val >= 19 && val <= 21);
            val = walker.Next();
            Assert.IsTrue(val >= 19 && val <= 21);
            val = walker.Next();
            Assert.IsTrue(val >= 19 && val <= 21);
            val = walker.Next();
            Assert.IsTrue(val >= 19 && val <= 21);
            val = walker.Next();
            Assert.IsTrue(val >= 19 && val <= 21);
            val = walker.Next();
            Assert.IsTrue(val >= 19 && val <= 21);
        }
    }
}

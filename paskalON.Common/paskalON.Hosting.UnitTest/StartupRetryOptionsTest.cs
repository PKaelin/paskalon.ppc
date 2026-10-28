// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Hosting.UnitTest
{
    [TestClass]
    public sealed class StartupRetryOptionsTest
    {
        [TestMethod]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow("   ", "   ")]
        public void CreateMissingValuesUsesDefaultsTest(string? retryCount, string? retryIntervalSeconds)
        {
            StartupRetryOptions options = StartupRetryOptions.Create(retryCount, retryIntervalSeconds);

            Assert.AreEqual(StartupRetryOptions.DefaultRetryCount, options.RetryCount);
            Assert.AreEqual(TimeSpan.FromSeconds(StartupRetryOptions.DefaultRetryIntervalSeconds), options.RetryInterval);
        }


        [TestMethod]
        [DataRow("3", "5", 3, 5)]
        [DataRow("0", "0", 0, 0)]
        [DataRow("20", "30", 20, 30)]
        public void CreateValidValuesUsesValuesTest(string retryCount, string retryIntervalSeconds, int expectedRetryCount, int expectedIntervalSeconds)
        {
            StartupRetryOptions options = StartupRetryOptions.Create(retryCount, retryIntervalSeconds);

            Assert.AreEqual(expectedRetryCount, options.RetryCount);
            Assert.AreEqual(TimeSpan.FromSeconds(expectedIntervalSeconds), options.RetryInterval);
        }


        [TestMethod]
        [DataRow("eight", "15")]
        [DataRow("8", "15s")]
        [DataRow("8.5", "15")]
        public void CreateNonIntegerValueThrowsTest(string retryCount, string retryIntervalSeconds)
        {
            Action act = () => StartupRetryOptions.Create(retryCount, retryIntervalSeconds);

            Assert.ThrowsExactly<FormatException>(act);
        }


        [TestMethod]
        [DataRow("-1", "15")]
        [DataRow("8", "-1")]
        public void CreateNegativeValueThrowsTest(string retryCount, string retryIntervalSeconds)
        {
            Action act = () => StartupRetryOptions.Create(retryCount, retryIntervalSeconds);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }
    }
}

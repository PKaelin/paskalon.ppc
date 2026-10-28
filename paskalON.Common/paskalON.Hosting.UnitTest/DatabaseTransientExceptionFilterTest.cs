// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Hosting.UnitTest.TestDoubles;

namespace paskalON.Hosting.UnitTest
{
    [TestClass]
    public sealed class DatabaseTransientExceptionFilterTest
    {
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsTransientDbExceptionReturnsTransientFlagTest(bool isTransient)
        {
            TestDbException exception = new TestDbException(isTransient);

            bool result = DatabaseTransientExceptionFilter.IsTransient(exception);

            Assert.AreEqual(isTransient, result);
        }


        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsTransientWrappedDbExceptionReturnsTransientFlagTest(bool isTransient)
        {
            // Entity Framework Npgsql execution strategy wraps transient errors in an invalid operation exception
            InvalidOperationException exception = new InvalidOperationException(
                "An exception has been raised that is likely due to a transient failure.", new TestDbException(isTransient));

            bool result = DatabaseTransientExceptionFilter.IsTransient(exception);

            Assert.AreEqual(isTransient, result);
        }


        [TestMethod]
        public void IsTransientWrappedTimeoutReturnsTrueTest()
        {
            InvalidOperationException exception = new InvalidOperationException(
                "An exception has been raised that is likely due to a transient failure.", new TimeoutException("Timeout during reading attempt"));

            bool result = DatabaseTransientExceptionFilter.IsTransient(exception);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void IsTransientTimeoutReturnsTrueTest()
        {
            TimeoutException exception = new TimeoutException("Timeout during reading attempt");

            bool result = DatabaseTransientExceptionFilter.IsTransient(exception);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void IsTransientOtherExceptionReturnsFalseTest()
        {
            InvalidOperationException exception = new InvalidOperationException("Sequence contains no elements");

            bool result = DatabaseTransientExceptionFilter.IsTransient(exception);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void IsTransientNullThrowsTest()
        {
            Action act = () => DatabaseTransientExceptionFilter.IsTransient(null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }
    }
}

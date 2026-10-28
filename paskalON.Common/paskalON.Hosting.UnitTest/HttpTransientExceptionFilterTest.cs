// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Net;
using System.Net.Sockets;

namespace paskalON.Hosting.UnitTest
{
    [TestClass]
    public sealed class HttpTransientExceptionFilterTest
    {
        [TestMethod]
        [DataRow(HttpRequestError.ConnectionError, true)]
        [DataRow(HttpRequestError.NameResolutionError, true)]
        [DataRow(HttpRequestError.ResponseEnded, true)]
        [DataRow(HttpRequestError.SecureConnectionError, false)]
        [DataRow(HttpRequestError.InvalidResponse, false)]
        [DataRow(HttpRequestError.Unknown, false)]
        public void IsTransientHttpRequestErrorTest(HttpRequestError error, bool expected)
        {
            HttpRequestException exception = new HttpRequestException(error, "Connection refused (paskalon.devices.service:8080)",
                new SocketException((int)SocketError.ConnectionRefused));

            bool result = HttpTransientExceptionFilter.IsTransient(exception);

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        public void IsTransientErrorResponseReturnsFalseTest(HttpStatusCode statusCode)
        {
            HttpRequestException exception = new HttpRequestException(HttpRequestError.Unknown, "Response status code does not indicate success", null, statusCode);

            bool result = HttpTransientExceptionFilter.IsTransient(exception);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void IsTransientHttpClientTimeoutReturnsTrueTest()
        {
            TaskCanceledException exception = new TaskCanceledException("The request was canceled due to the configured HttpClient.Timeout of 10 seconds elapsing.",
                new TimeoutException("The operation was canceled."));

            bool result = HttpTransientExceptionFilter.IsTransient(exception);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void IsTransientTaskCanceledWithoutTimeoutReturnsFalseTest()
        {
            TaskCanceledException exception = new TaskCanceledException("A task was canceled.");

            bool result = HttpTransientExceptionFilter.IsTransient(exception);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void IsTransientTimeoutReturnsTrueTest()
        {
            TimeoutException exception = new TimeoutException("The operation has timed out.");

            bool result = HttpTransientExceptionFilter.IsTransient(exception);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void IsTransientOtherExceptionReturnsFalseTest()
        {
            InvalidOperationException exception = new InvalidOperationException("Device server returned an empty DER data transfer object");

            bool result = HttpTransientExceptionFilter.IsTransient(exception);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void IsTransientNullThrowsTest()
        {
            Action act = () => HttpTransientExceptionFilter.IsTransient(null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }
    }
}

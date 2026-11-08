// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Net;
using System.Text;
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Alarms.Client.UnitTest
{
    /// <summary>
    /// Tests <see cref="AlarmServer"/> with an in-memory HTTP message handler.
    /// </summary>
    [TestClass]
    public sealed class AlarmServerTest
    {
        /// <summary>
        /// The in-memory HTTP message handler recording the requests.
        /// </summary>
        private StubHttpMessageHandler _handler = null!;


        /// <summary>
        /// The HTTP client with the base address of the alarm service API.
        /// </summary>
        private HttpClient _client = null!;


        /// <summary>
        /// The alarm server under test.
        /// </summary>
        private AlarmServer _server = null!;


        /// <summary>
        /// Creates the alarm server with an HTTP client answering with OK.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _handler = new StubHttpMessageHandler();
            _client = new HttpClient(_handler) { BaseAddress = new Uri("http://paskalon.alarms.service:8080/api/v1/") };
            _server = new AlarmServer(_client);
        }


        /// <summary>
        /// Disposes the HTTP client.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
        }


        /// <summary>
        /// The constructor validates the HTTP client and its base address.
        /// </summary>
        [TestMethod]
        public void AlarmServerConstructorInvalidClientThrowsTest()
        {
            using HttpClient clientWithoutBaseAddress = new HttpClient();

            Action withoutBaseAddress = () => new AlarmServer(clientWithoutBaseAddress);

            Assert.ThrowsExactly<ArgumentNullException>(() => new AlarmServer(null!));
            Assert.ThrowsExactly<ArgumentNullException>(withoutBaseAddress);
        }


        /// <summary>
        /// The alarm summary is requested from the summary endpoint and deserialized.
        /// </summary>
        [TestMethod]
        public async Task GetAlarmSummaryReturnsSummaryTest()
        {
            _handler.ResponseContent = "{\"calculatedOn\":\"2026-11-07T11:00:00+00:00\",\"infoCount\":2,\"warningCount\":1,\"errorCount\":7,\"criticalCount\":0}";

            AlarmSummaryDto summary = await _server.GetAlarmSummary();

            Assert.AreEqual(HttpMethod.Get, _handler.Request!.Method);
            Assert.AreEqual("/api/v1/alarms/summary", _handler.Request.RequestUri!.AbsolutePath);
            Assert.AreEqual(new DateTimeOffset(2026, 11, 7, 11, 0, 0, TimeSpan.Zero), summary.CalculatedOn);
            Assert.AreEqual(2, summary.InfoCount);
            Assert.AreEqual(1, summary.WarningCount);
            Assert.AreEqual(7, summary.ErrorCount);
            Assert.AreEqual(0, summary.CriticalCount);
        }


        /// <summary>
        /// An error of the alarm service (e.g. database not available) throws.
        /// </summary>
        [TestMethod]
        public async Task GetAlarmSummaryServiceUnavailableThrowsTest()
        {
            _handler.StatusCode = HttpStatusCode.ServiceUnavailable;

            Func<Task> getSummary = () => _server.GetAlarmSummary();

            HttpRequestException exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(getSummary);
            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }


        /// <summary>
        /// In-memory HTTP message handler answering with a configurable response and recording the last request.
        /// </summary>
        private sealed class StubHttpMessageHandler : HttpMessageHandler
        {
            /// <summary>
            /// Gets or sets the status code of the response.
            /// </summary>
            public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;


            /// <summary>
            /// Gets or sets the JSON content of the response.
            /// </summary>
            public string ResponseContent { get; set; } = string.Empty;


            /// <summary>
            /// Gets the last request.
            /// </summary>
            public HttpRequestMessage? Request { get; private set; }


            /// <inheritdoc/>
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Request = request;

                return Task.FromResult(new HttpResponseMessage(StatusCode) { Content = new StringContent(ResponseContent, Encoding.UTF8, "application/json") });
            }
        }
    }
}

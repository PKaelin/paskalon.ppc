// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Net;
using System.Text;
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Localizations;
using paskalON.Alarms.Dto.Ppcs;
using paskalON.Alarms.Dto.Summaries;
using paskalON.Alarms.Service.Dto.V1.Requests;
using paskalON.Alarms.Service.Dto.V1.Responses;

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
            _handler.ResponseContent = "{\"calculatedOn\":\"2026-11-07T11:00:00+00:00\",\"infoCount\":2,\"warningCount\":1,\"errorCount\":7,\"criticalCount\":0,\"acknowledgedInfoCount\":0,\"acknowledgedWarningCount\":0,\"acknowledgedErrorCount\":3,\"acknowledgedCriticalCount\":1}";

            AlarmSummaryDto summary = await _server.GetAlarmSummary();

            Assert.AreEqual(HttpMethod.Get, _handler.Request!.Method);
            Assert.AreEqual("/api/v1/alarms/summary", _handler.Request.RequestUri!.AbsolutePath);
            Assert.AreEqual(new DateTimeOffset(2026, 11, 7, 11, 0, 0, TimeSpan.Zero), summary.CalculatedOn);
            Assert.AreEqual(2, summary.InfoCount);
            Assert.AreEqual(1, summary.WarningCount);
            Assert.AreEqual(7, summary.ErrorCount);
            Assert.AreEqual(0, summary.CriticalCount);
            Assert.AreEqual(3, summary.AcknowledgedErrorCount);
            Assert.AreEqual(1, summary.AcknowledgedCriticalCount);
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
        /// All set criteria of the filter are sent as query parameters, the enumerations by name and the timestamp escaped.
        /// </summary>
        [TestMethod]
        public async Task GetPpcAlarmsSendsFilterAsQueryTest()
        {
            _handler.ResponseContent = "[]";
            PpcAlarmFilterDto filter = new PpcAlarmFilterDto
            {
                CreatedFrom = new DateTimeOffset(2026, 11, 9, 6, 0, 0, TimeSpan.FromHours(1)),
                SeverityLevel = AlarmSeverityDto.Critical,
                AlarmSequenceState = AlarmSequenceStateDto.Acknowledged,
                DeviceName = " PCS 7 "
            };

            await _server.GetPpcAlarms(filter);

            Assert.AreEqual(HttpMethod.Get, _handler.Request!.Method);
            Assert.AreEqual("/api/v1/alarms/ppcalarms", _handler.Request.RequestUri!.AbsolutePath);
            Assert.AreEqual("?createdFrom=2026-11-09T06%3A00%3A00.0000000%2B01%3A00&severityLevel=Critical" +
                "&alarmSequenceState=Acknowledged&deviceName=PCS%207", _handler.Request.RequestUri.Query);
        }


        /// <summary>
        /// Criteria that are not set are not sent.
        /// </summary>
        [TestMethod]
        public async Task GetPpcAlarmsWithoutCriteriaSendsCreatedFromOnlyTest()
        {
            _handler.ResponseContent = "[]";
            PpcAlarmFilterDto filter = new PpcAlarmFilterDto { CreatedFrom = new DateTimeOffset(2026, 11, 9, 6, 0, 0, TimeSpan.Zero), DeviceName = "  " };

            await _server.GetPpcAlarms(filter);

            Assert.AreEqual("?createdFrom=2026-11-09T06%3A00%3A00.0000000%2B00%3A00", _handler.Request!.RequestUri!.Query);
        }


        /// <summary>
        /// The alarms of the alarm service are deserialized with their configuration, the enumerations are sent as numbers.
        /// </summary>
        [TestMethod]
        public async Task GetPpcAlarmsReturnsAlarmsTest()
        {
            _handler.ResponseContent = "[{\"id\":100,\"ppcAlarmType\":1,\"deviceId\":7,\"deviceName\":\"PCS 7\",\"propertyName\":\"FaultStates.OverTemperature\"," +
                "\"alarmSequenceState\":1,\"acknowledgedBy\":\"Homer\",\"acknowledgedOn\":\"2026-11-09T07:05:00+00:00\",\"canBeAcknowledged\":false,\"createdOn\":\"2026-11-09T07:00:00+00:00\",\"changedBy\":\"Homer\"," +
                "\"changedDate\":\"2026-11-09T07:05:00+00:00\",\"ppcAlarmConfig\":{\"alarmId\":\"PCS-FAULT\",\"name\":\"{{PCS.Fault.Name}}\"," +
                "\"severityLevel\":2,\"isAcknowledgeable\":true,\"description\":\"{{PCS.Fault.Desc}}\",\"possibleCauses\":\"{{PCS.Fault.Causes}}\",\"alarmActions\":[]}}]";
            PpcAlarmFilterDto filter = new PpcAlarmFilterDto { CreatedFrom = new DateTimeOffset(2026, 11, 9, 6, 0, 0, TimeSpan.Zero) };

            List<PpcAlarmDto> alarms = await _server.GetPpcAlarms(filter);

            Assert.HasCount(1, alarms);
            PpcAlarmDto alarm = alarms[0];
            Assert.AreEqual(100, alarm.Id);
            Assert.AreEqual(PpcAlarmTypeDto.PcsAlarm, alarm.PpcAlarmType);
            Assert.AreEqual("PCS 7", alarm.DeviceName);
            Assert.AreEqual(AlarmSequenceStateDto.Acknowledged, alarm.AlarmSequenceState);
            Assert.AreEqual("Homer", alarm.AcknowledgedBy);
            Assert.AreEqual(new DateTimeOffset(2026, 11, 9, 7, 5, 0, TimeSpan.Zero), alarm.AcknowledgedOn);
            Assert.IsFalse(alarm.CanBeAcknowledged);
            Assert.IsTrue(alarm.PpcAlarmConfig.IsAcknowledgeable);
            Assert.AreEqual(new DateTimeOffset(2026, 11, 9, 7, 0, 0, TimeSpan.Zero), alarm.CreatedOn);
            Assert.AreEqual("Homer", alarm.ChangedBy);
            Assert.AreEqual(new DateTimeOffset(2026, 11, 9, 7, 5, 0, TimeSpan.Zero), alarm.ChangedDate);
            Assert.AreEqual(AlarmSeverityDto.Error, alarm.PpcAlarmConfig.SeverityLevel);
            Assert.AreEqual("{{PCS.Fault.Name}}", alarm.PpcAlarmConfig.Name);
            Assert.AreEqual("{{PCS.Fault.Desc}}", alarm.PpcAlarmConfig.Description);
            Assert.AreEqual("{{PCS.Fault.Causes}}", alarm.PpcAlarmConfig.PossibleCauses);
        }


        /// <summary>
        /// An error of the alarm service (e.g. database not available) throws.
        /// </summary>
        [TestMethod]
        public async Task GetPpcAlarmsServiceUnavailableThrowsTest()
        {
            _handler.StatusCode = HttpStatusCode.ServiceUnavailable;
            PpcAlarmFilterDto filter = new PpcAlarmFilterDto { CreatedFrom = new DateTimeOffset(2026, 11, 9, 6, 0, 0, TimeSpan.Zero) };

            Func<Task> getAlarms = () => _server.GetPpcAlarms(filter);

            HttpRequestException exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(getAlarms);
            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }


        /// <summary>
        /// The texts are requested in the escaped language and deserialized by their localization key.
        /// </summary>
        [TestMethod]
        public async Task GetLocalizedTextsReturnsTextsOfLanguageTest()
        {
            _handler.ResponseContent = "{\"language\":\"de-CH\",\"texts\":{\"PCS.CommunicationError.Name\":\"PCS Kommunikationsfehler\"," +
                "\"PCS.CommunicationError.Desc\":\"The PPC cannot communicate with the power conversion system.\"}}";

            LocalizedTextsDto texts = await _server.GetLocalizedTexts(" de-CH ");

            Assert.AreEqual(HttpMethod.Get, _handler.Request!.Method);
            Assert.AreEqual("/api/v1/localizations/texts", _handler.Request.RequestUri!.AbsolutePath);
            Assert.AreEqual("?language=de-CH", _handler.Request.RequestUri.Query);
            Assert.AreEqual("de-CH", texts.Language);
            Assert.HasCount(2, texts.Texts);
            Assert.AreEqual("PCS Kommunikationsfehler", texts.Texts["PCS.CommunicationError.Name"]);
        }


        /// <summary>
        /// A rejected language or an unavailable alarm service throws with the status code.
        /// </summary>
        /// <param name="statusCode">Status code of the alarm service.</param>
        [TestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        public async Task GetLocalizedTextsErrorThrowsTest(HttpStatusCode statusCode)
        {
            _handler.StatusCode = statusCode;

            Func<Task> getTexts = () => _server.GetLocalizedTexts("en");

            HttpRequestException exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(getTexts);
            Assert.AreEqual(statusCode, exception.StatusCode);
        }


        /// <summary>
        /// An empty language is rejected without a request.
        /// </summary>
        [TestMethod]
        public async Task GetLocalizedTextsEmptyLanguageThrowsTest()
        {
            Func<Task> getTexts = () => _server.GetLocalizedTexts(" ");

            await Assert.ThrowsExactlyAsync<ArgumentException>(getTexts);
            Assert.IsNull(_handler.Request);
        }


        /// <summary>
        /// The alarm ids and the user are posted to the acknowledge endpoint, the number of acknowledged alarms is returned.
        /// </summary>
        [TestMethod]
        public async Task AcknowledgeAlarmsPostsRequestTest()
        {
            _handler.ResponseContent = "{\"acknowledgedCount\":2}";
            AcknowledgeAlarmsRequest request = new AcknowledgeAlarmsRequest { AlarmIds = new List<int> { 100, 101, 102 }, AcknowledgedBy = "Operator" };

            AcknowledgeAlarmsResponse response = await _server.AcknowledgeAlarms(request);

            Assert.AreEqual(HttpMethod.Post, _handler.Request!.Method);
            Assert.AreEqual("/api/v1/alarms/acknowledge", _handler.Request.RequestUri!.AbsolutePath);
            Assert.AreEqual("{\"alarmIds\":[100,101,102],\"acknowledgedBy\":\"Operator\"}", _handler.RequestContent);
            Assert.AreEqual(2, response.AcknowledgedCount);
        }


        /// <summary>
        /// An error of the alarm service (e.g. database not available) throws.
        /// </summary>
        [TestMethod]
        public async Task AcknowledgeAlarmsServiceUnavailableThrowsTest()
        {
            _handler.StatusCode = HttpStatusCode.ServiceUnavailable;
            AcknowledgeAlarmsRequest request = new AcknowledgeAlarmsRequest { AlarmIds = new List<int> { 100 }, AcknowledgedBy = "Operator" };

            Func<Task> acknowledge = () => _server.AcknowledgeAlarms(request);

            HttpRequestException exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(acknowledge);
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


            /// <summary>
            /// Gets the content of the last request, null without content.
            /// </summary>
            public string? RequestContent { get; private set; }


            /// <inheritdoc/>
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Request = request;
                RequestContent = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);

                return new HttpResponseMessage(StatusCode) { Content = new StringContent(ResponseContent, Encoding.UTF8, "application/json") };
            }
        }
    }
}

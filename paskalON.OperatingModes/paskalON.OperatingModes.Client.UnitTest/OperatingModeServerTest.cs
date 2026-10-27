// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Dto.Overrides;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.OperatingModes.Dto.Stacks;
using paskalON.OperatingModes.Service.Dto.V1.Requests;
using System.Net;
using System.Text;
using System.Text.Json;

namespace paskalON.OperatingModes.Client.UnitTest
{
    /// <summary>
    /// Tests <see cref="OperatingModeServer"/> with an in-memory HTTP message handler.
    /// </summary>
    [TestClass]
    public sealed class OperatingModeServerTest
    {
        /// <summary>
        /// The in-memory HTTP message handler recording the requests.
        /// </summary>
        private StubHttpMessageHandler _handler = null!;


        /// <summary>
        /// The HTTP client with the base address of the operating mode service API.
        /// </summary>
        private HttpClient _client = null!;


        /// <summary>
        /// The operating mode server under test.
        /// </summary>
        private OperatingModeServer _server = null!;


        /// <summary>
        /// Creates the operating mode server with an HTTP client answering with OK.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _handler = new StubHttpMessageHandler();
            _client = new HttpClient(_handler) { BaseAddress = new Uri("http://paskalon.operatingmodes.service:8080/api/v1/") };
            _server = new OperatingModeServer(_client);
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
        /// Confirms the constructor validates the HTTP client and its base address.
        /// </summary>
        [TestMethod]
        public void OperatingModeServerConstructorInvalidClientThrowsTest()
        {
            using HttpClient clientWithoutBaseAddress = new HttpClient();

            Assert.ThrowsExactly<ArgumentNullException>(() => new OperatingModeServer(null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => new OperatingModeServer(clientWithoutBaseAddress));
        }


        /// <summary>
        /// Confirms the stack DTOs are requested from the endpoint and deserialized with their specialized types.
        /// </summary>
        /// <param name="isActive">True for the active operating modes, false for the available operating modes.</param>
        /// <param name="expectedPath">The expected request path.</param>
        [TestMethod]
        [DataRow(false, "/api/v1/stack/getavailableoperatingmodes")]
        [DataRow(true, "/api/v1/stack/getactiveoperatingmodes")]
        public async Task OperatingModeServerGetOperatingModesDeserializesDerivedDtosTest(bool isActive, string expectedPath)
        {
            List<OperatingModeStackDto> dtos = new List<OperatingModeStackDto>
            {
                new OperatingModeStackDto { OperatingModeName = "ActivePowerMode", Type = OperatingModeStackTypeDto.BaseActive, Priority = 1 },
                new OperatingModeLimitingStackDto { OperatingModeName = "MaximumActivePowerLimitMode", Type = OperatingModeStackTypeDto.LimitingActive, Priority = 2, Limit = 6000000 }
            };
            _handler.ResponseContent = JsonSerializer.Serialize(dtos);

            IReadOnlyList<OperatingModeStackDto> result = isActive
                ? await _server.GetActiveOperatingModes()
                : await _server.GetAvailableOperatingModes();

            Assert.AreEqual(HttpMethod.Get, _handler.Request!.Method);
            Assert.AreEqual(expectedPath, _handler.Request!.RequestUri!.AbsolutePath);
            Assert.HasCount(2, result);
            Assert.AreEqual("ActivePowerMode", result[0].OperatingModeName);
            OperatingModeLimitingStackDto limitingDto = Assert.IsInstanceOfType<OperatingModeLimitingStackDto>(result[1]);
            Assert.AreEqual(6000000, limitingDto.Limit);
            Assert.AreEqual(2, limitingDto.Priority);
        }


        /// <summary>
        /// Confirms the stack requests are posted as JSON with web defaults (camel case) to their endpoints.
        /// </summary>
        /// <param name="action">The action to call.</param>
        /// <param name="expectedPath">The expected request path.</param>
        [TestMethod]
        [DataRow("Add", "/api/v1/stack/addoperatingmode")]
        [DataRow("Move", "/api/v1/stack/moveoperatingmode")]
        [DataRow("Remove", "/api/v1/stack/removeoperatingmode")]
        [DataRow("Replace", "/api/v1/stack/replaceoperatingmode")]
        public async Task OperatingModeServerStackRequestPostsToEndpointTest(string action, string expectedPath)
        {
            Task request = action switch
            {
                "Add" => _server.AddOperatingMode(new AddOperatingModeStackRequest { OperatingModeName = "ActivePowerMode", Priority = 1 }),
                "Move" => _server.MoveOperatingMode(new MoveOperatingModeStackRequest { OperatingModeName = "ActivePowerMode", Priority = 2 }),
                "Remove" => _server.RemoveOperatingMode(new RemoveOperatingModeStackRequest { OperatingModeName = "ActivePowerMode" }),
                _ => _server.ReplaceOperatingMode(new ReplaceOperatingModeStackRequest { PredecessorName = "ActivePowerFixedMode", OperatingModeName = "ActivePowerMode" })
            };

            await request;

            Assert.AreEqual(HttpMethod.Post, _handler.Request!.Method);
            Assert.AreEqual(expectedPath, _handler.Request!.RequestUri!.AbsolutePath);
            StringAssert.Contains(_handler.RequestContent, "\"operatingModeName\":\"ActivePowerMode\"");
        }


        /// <summary>
        /// Confirms the active override DTOs are requested from the override endpoint and deserialized.
        /// </summary>
        [TestMethod]
        public async Task OperatingModeServerGetActiveOverrideOperatingModesDeserializesDtosTest()
        {
            List<OperatingModeOverrideDto> dtos = new List<OperatingModeOverrideDto>
            {
                new OperatingModeOverrideDto { OperatingModeName = "ShutdownMode", TargetDerUnit = null },
                new OperatingModeOverrideDto { OperatingModeName = "MaintenanceModeUnit1", TargetDerUnit = "Unit1" }
            };
            _handler.ResponseContent = JsonSerializer.Serialize(dtos);

            IReadOnlyList<OperatingModeOverrideDto> result = await _server.GetActiveOverrideOperatingModes();

            Assert.AreEqual(HttpMethod.Get, _handler.Request!.Method);
            Assert.AreEqual("/api/v1/override/getactiveoperatingmodes", _handler.Request!.RequestUri!.AbsolutePath);
            CollectionAssert.AreEqual(dtos, result.ToList());
        }


        /// <summary>
        /// Confirms the override requests are posted as JSON with web defaults (camel case) to their endpoints.
        /// </summary>
        /// <param name="isActivated">True to activate, false to deactivate the override operating mode.</param>
        /// <param name="expectedPath">The expected request path.</param>
        [TestMethod]
        [DataRow(true, "/api/v1/override/activateoperatingmode")]
        [DataRow(false, "/api/v1/override/deactivateoperatingmode")]
        public async Task OperatingModeServerOverrideRequestPostsToEndpointTest(bool isActivated, string expectedPath)
        {
            Task request = isActivated
                ? _server.ActivateOverrideOperatingMode(new ActivateOverrideRequest { OperatingModeName = "ShutdownMode" })
                : _server.DeactivateOverrideOperatingMode(new DeactivateOverrideRequest { OperatingModeName = "ShutdownMode" });

            await request;

            Assert.AreEqual(HttpMethod.Post, _handler.Request!.Method);
            Assert.AreEqual(expectedPath, _handler.Request!.RequestUri!.AbsolutePath);
            StringAssert.Contains(_handler.RequestContent, "\"operatingModeName\":\"ShutdownMode\"");
        }


        /// <summary>
        /// Confirms the system setpoint is requested from the system endpoint and deserialized.
        /// </summary>
        [TestMethod]
        public async Task OperatingModeServerGetSystemSetpointDeserializesDtoTest()
        {
            SystemSetpointDto dto = new SystemSetpointDto { ActivePowerWatts = 5000000, ReactivePowerVars = -1000000 };
            _handler.ResponseContent = JsonSerializer.Serialize(dto);

            SystemSetpointDto result = await _server.GetSystemSetpoint();

            Assert.AreEqual(HttpMethod.Get, _handler.Request!.Method);
            Assert.AreEqual("/api/v1/system/getsetpoint", _handler.Request!.RequestUri!.AbsolutePath);
            Assert.AreEqual(dto, result);
        }


        /// <summary>
        /// Confirms the system setpoint request is posted as JSON to the system endpoint.
        /// </summary>
        [TestMethod]
        public async Task OperatingModeServerSetSystemSetpointPostsToEndpointTest()
        {
            SetSystemSetpointRequest request = new SetSystemSetpointRequest { ActivePowerWatts = 5000000, ReactivePowerVars = -1000000 };

            await _server.SetSystemSetpoint(request);

            using JsonDocument content = JsonDocument.Parse(_handler.RequestContent!);
            Assert.AreEqual(HttpMethod.Post, _handler.Request!.Method);
            Assert.AreEqual("/api/v1/system/setsetpoint", _handler.Request!.RequestUri!.AbsolutePath);
            Assert.AreEqual(5000000, content.RootElement.GetProperty("activePowerWatts").GetDouble());
            Assert.AreEqual(-1000000, content.RootElement.GetProperty("reactivePowerVars").GetDouble());
        }


        /// <summary>
        /// Confirms a request rejected by the operating mode service throws with its status code.
        /// </summary>
        [TestMethod]
        public async Task OperatingModeServerRejectedRequestThrowsWithStatusCodeTest()
        {
            _handler.StatusCode = HttpStatusCode.BadRequest;

            HttpRequestException exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(
                () => _server.AddOperatingMode(new AddOperatingModeStackRequest { OperatingModeName = "Unknown", Priority = 1 }));

            Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
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
            /// Gets the content of the last request.
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

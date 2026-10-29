// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Service.Dto.V1.Requests;
using System.Net;
using System.Text.Json;

namespace paskalON.Devices.Client.UnitTest
{
    [TestClass]
    public sealed class DeviceServerTest
    {
        private Uri _baseAddress = new Uri($"http://somehost:1234/api/v1/");


        [TestMethod]
        public void DeviceServerNullClientTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceServer(null!));
        }


        [TestMethod]
        public void DeviceServerClientWithoutBaseAddressTest()
        {
            using HttpClient client = new HttpClient();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceServer(client));
        }


        [TestMethod]
        public async Task StartPcsPostsDeviceIdToStartEndpointTest()
        {
            using RecordingHttpMessageHandler handler = new RecordingHttpMessageHandler(HttpStatusCode.OK);
            using HttpClient client = new HttpClient(handler) { BaseAddress = _baseAddress };
            DeviceServer server = new DeviceServer(client);

            await server.StartPcs(7);

            Assert.AreEqual(HttpMethod.Post, handler.Method);
            Assert.AreEqual(new Uri($"{_baseAddress.AbsoluteUri}pcs/start"), handler.RequestUri);
            StartPcsRequestDto? request = handler.ReadBody<StartPcsRequestDto>();
            Assert.IsNotNull(request);
            Assert.AreEqual(7, request.DeviceId);
        }


        [TestMethod]
        public async Task SetPcsPowerTargetPostsTargetsToSetPowerTargetEndpointTest()
        {
            using RecordingHttpMessageHandler handler = new RecordingHttpMessageHandler(HttpStatusCode.OK);
            using HttpClient client = new HttpClient(handler) { BaseAddress = _baseAddress };
            DeviceServer server = new DeviceServer(client);

            await server.SetPcsPowerTarget(7, 25000.5, -4000.25);

            Assert.AreEqual(HttpMethod.Post, handler.Method);
            Assert.AreEqual(new Uri($"{_baseAddress.AbsoluteUri}pcs/setpowertarget"), handler.RequestUri);
            SetPowerTargetRequest? request = handler.ReadBody<SetPowerTargetRequest>();
            Assert.IsNotNull(request);
            Assert.AreEqual(7, request.DeviceId);
            Assert.AreEqual(25000.5, request.ActivePowerWatt);
            Assert.AreEqual(-4000.25, request.ReactivePowerVar);
        }


        /// <summary>
        /// Confirms the device commands post the device identifier to their endpoints.
        /// </summary>
        /// <param name="command">The command to call.</param>
        /// <param name="expectedPath">The expected relative path of the endpoint.</param>
        [TestMethod]
        [DataRow("StopPcs", "pcs/stop")]
        [DataRow("StandbyPcs", "pcs/standby")]
        [DataRow("ConnectBatteryBank", "bb/connect")]
        [DataRow("DisconnectBatteryBank", "bb/disconnect")]
        public async Task DeviceCommandPostsDeviceIdToEndpointTest(string command, string expectedPath)
        {
            using RecordingHttpMessageHandler handler = new RecordingHttpMessageHandler(HttpStatusCode.OK);
            using HttpClient client = new HttpClient(handler) { BaseAddress = _baseAddress };
            DeviceServer server = new DeviceServer(client);
            Task request = command switch
            {
                "StopPcs" => server.StopPcs(7),
                "StandbyPcs" => server.StandbyPcs(7),
                "ConnectBatteryBank" => server.ConnectBatteryBank(7),
                _ => server.DisconnectBatteryBank(7)
            };

            await request;

            Assert.AreEqual(HttpMethod.Post, handler.Method);
            Assert.AreEqual(new Uri($"{_baseAddress.AbsoluteUri}{expectedPath}"), handler.RequestUri);
            StopPcsRequestDto? body = handler.ReadBody<StopPcsRequestDto>();
            Assert.IsNotNull(body);
            Assert.AreEqual(7, body.DeviceId);
        }


        /// <summary>
        /// Confirms putting a DER unit into or out of maintenance posts the unit name and the flag to the DER endpoint.
        /// </summary>
        /// <param name="intoMaintenance">True to put the unit into maintenance, false to take it out of maintenance.</param>
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task SetMaintenancePostsUnitNameToDerEndpointTest(bool intoMaintenance)
        {
            using RecordingHttpMessageHandler handler = new RecordingHttpMessageHandler(HttpStatusCode.OK);
            using HttpClient client = new HttpClient(handler) { BaseAddress = _baseAddress };
            DeviceServer server = new DeviceServer(client);

            await server.SetMaintenance("Bess1", intoMaintenance);

            Assert.AreEqual(HttpMethod.Post, handler.Method);
            Assert.AreEqual(new Uri($"{_baseAddress.AbsoluteUri}der/setmaintenance"), handler.RequestUri);
            SetMaintenanceRequest? request = handler.ReadBody<SetMaintenanceRequest>();
            Assert.IsNotNull(request);
            Assert.AreEqual("Bess1", request.UnitName);
            Assert.AreEqual(intoMaintenance, request.IntoMaintenance);
        }


        /// <summary>
        /// Confirms an invalid DER unit name is rejected before calling the device service.
        /// </summary>
        /// <param name="unitName">The invalid DER unit name.</param>
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public async Task SetMaintenanceInvalidUnitNameThrowsTest(string unitName)
        {
            using RecordingHttpMessageHandler handler = new RecordingHttpMessageHandler(HttpStatusCode.OK);
            using HttpClient client = new HttpClient(handler) { BaseAddress = _baseAddress };
            DeviceServer server = new DeviceServer(client);

            await Assert.ThrowsExactlyAsync<ArgumentException>(() => server.SetMaintenance(unitName, true));

            Assert.IsNull(handler.Method);
        }


        [TestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task StartPcsUnsuccessfulStatusThrowsTest(HttpStatusCode statusCode)
        {
            using RecordingHttpMessageHandler handler = new RecordingHttpMessageHandler(statusCode);
            using HttpClient client = new HttpClient(handler) { BaseAddress = _baseAddress };
            DeviceServer server = new DeviceServer(client);

            Task start = server.StartPcs(7);

            await Assert.ThrowsExactlyAsync<HttpRequestException>(async () => await start);
        }


        [TestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task SetPcsPowerTargetUnsuccessfulStatusThrowsTest(HttpStatusCode statusCode)
        {
            using RecordingHttpMessageHandler handler = new RecordingHttpMessageHandler(statusCode);
            using HttpClient client = new HttpClient(handler) { BaseAddress = _baseAddress };
            DeviceServer server = new DeviceServer(client);

            Task setTarget = server.SetPcsPowerTarget(7, 25000, 0);

            await Assert.ThrowsExactlyAsync<HttpRequestException>(async () => await setTarget);
        }


        /// <summary>
        /// In-memory HTTP handler that records the request and returns a fixed status code.
        /// </summary>
        private sealed class RecordingHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;
            private string _body = string.Empty;
            public HttpMethod? Method { get; private set; }
            public Uri? RequestUri { get; private set; }


            public RecordingHttpMessageHandler(HttpStatusCode statusCode)
            {
                _statusCode = statusCode;
            }


            public T? ReadBody<T>()
            {
                return JsonSerializer.Deserialize<T>(_body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Method = request.Method;
                RequestUri = request.RequestUri;

                if (request.Content != null)
                {
                    _body = await request.Content.ReadAsStringAsync(cancellationToken);
                }

                return new HttpResponseMessage(_statusCode);
            }
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Service.Dto.V1.Requests;
using System.Net.Http.Json;

namespace paskalON.Devices.Client
{
    /// <summary>
    /// Device server for device service actions.
    /// </summary>
    public class DeviceServer : IDeviceServer
    {
        /// <summary>
        /// Relative path of the get DER endpoint.
        /// </summary>
        private const string GetDerPath = "der/getder";


        /// <summary>
        /// Relative path of the start PCS endpoint.
        /// </summary>
        private const string StartPcsPath = "pcs/start";


        /// <summary>
        /// Relative path of the start all PCS endpoint.
        /// </summary>
        private const string StartAllPcsPath = "pcs/startall";

        /// <summary>
        /// Relative path of the standby all PCS endpoint.
        /// </summary>
        private const string StandbyAllPcsPath = "pcs/standbyall";


        /// <summary>
        /// Relative path of the stop all PCS endpoint.
        /// </summary>
        private const string StopAllPcsPath = "pcs/stopall";


        /// <summary>
        /// Relative path of the set PCS power target endpoint.
        /// </summary>
        private const string SetPcsPowerTargetPath = "pcs/setpowertarget";


        /// <summary>
        /// Relative path of the stop PCS endpoint.
        /// </summary>
        private const string StopPcsPath = "pcs/stop";


        /// <summary>
        /// Relative path of the standby PCS endpoint.
        /// </summary>
        private const string StandbyPcsPath = "pcs/standby";


        /// <summary>
        /// Relative path of the put into maintenance endpoint.
        /// </summary>
        private const string PutIntoMaintenancePath = "der/putintomaintenance";


        /// <summary>
        /// Relative path of the connect battery bank endpoint.
        /// </summary>
        private const string ConnectBatteryBankPath = "bb/connect";


        /// <summary>
        /// Relative path of the disconnect battery bank endpoint.
        /// </summary>
        private const string DisconnectBatteryBankPath = "bb/disconnect";


        /// <summary>
        /// HTTP client with the base address of the device service API.
        /// </summary>
        private readonly HttpClient _client;


        /// <summary>
        /// Constructor of <see cref="DeviceServer"/>.
        /// </summary>
        /// <param name="client">HTTP client with the base address of the device service API (e.g. http://host:8080/api/v1/).</param>
        public DeviceServer(HttpClient client)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(client.BaseAddress);

            _client = client;
        }


        /// <inheritdoc/>
        public async Task<DerDto> GetDer()
        {
            using HttpResponseMessage response = await _client.GetAsync(GetDerPath);
            response.EnsureSuccessStatusCode();

            DerDto? dto = await response.Content.ReadFromJsonAsync<DerDto>();

            if (dto == null)
            {
                throw new InvalidOperationException("Device server returned an empty DER data transfer object");
            }

            return dto;
        }


        /// <inheritdoc/>
        public async Task StartPcs(int deviceId, CancellationToken cancellationToken = default)
        {
            StartPcsRequestDto request = new StartPcsRequestDto { DeviceId = deviceId };

            using HttpResponseMessage response = await _client.PostAsJsonAsync(StartPcsPath, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc/>
        public async Task StartAllPcs(CancellationToken cancellationToken = default)
        {
            using HttpResponseMessage response = await _client.PostAsync(StartAllPcsPath, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc/>
        public async Task StandbyAllPcs(CancellationToken cancellationToken = default)
        {
            using HttpResponseMessage response = await _client.PostAsync(StandbyAllPcsPath, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc/>
        public async Task StopAllPcs(CancellationToken cancellationToken = default)
        {
            using HttpResponseMessage response = await _client.PostAsync(StopAllPcsPath, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc/>
        public async Task SetPcsPowerTarget(int deviceId, double activePowerWatt, double reactivePowerVar, CancellationToken cancellationToken = default)
        {
            SetPowerTargetRequest request = new SetPowerTargetRequest
            {
                DeviceId = deviceId,
                ActivePowerWatt = activePowerWatt,
                ReactivePowerVar = reactivePowerVar
            };

            using HttpResponseMessage response = await _client.PostAsJsonAsync(SetPcsPowerTargetPath, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc/>
        public async Task StopPcs(int deviceId, CancellationToken cancellationToken = default)
        {
            StopPcsRequestDto request = new StopPcsRequestDto { DeviceId = deviceId };

            using HttpResponseMessage response = await _client.PostAsJsonAsync(StopPcsPath, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc/>
        public async Task StandbyPcs(int deviceId, CancellationToken cancellationToken = default)
        {
            StandbyPcsRequestDto request = new StandbyPcsRequestDto { DeviceId = deviceId };

            using HttpResponseMessage response = await _client.PostAsJsonAsync(StandbyPcsPath, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc/>
        public async Task PutIntoMaintenance(string unitName, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(unitName);

            PutIntoMaintenanceRequest request = new PutIntoMaintenanceRequest { UnitName = unitName };

            using HttpResponseMessage response = await _client.PostAsJsonAsync(PutIntoMaintenancePath, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc/>
        public async Task ConnectBatteryBank(int deviceId, CancellationToken cancellationToken = default)
        {
            ConnectBbRequest request = new ConnectBbRequest { DeviceId = deviceId };

            using HttpResponseMessage response = await _client.PostAsJsonAsync(ConnectBatteryBankPath, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }


        /// <inheritdoc/>
        public async Task DisconnectBatteryBank(int deviceId, CancellationToken cancellationToken = default)
        {
            DisconnectBbRequest request = new DisconnectBbRequest { DeviceId = deviceId };

            using HttpResponseMessage response = await _client.PostAsJsonAsync(DisconnectBatteryBankPath, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}

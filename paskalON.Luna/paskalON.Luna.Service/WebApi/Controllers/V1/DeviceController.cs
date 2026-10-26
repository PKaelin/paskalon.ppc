// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Devices.Client;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Service.Dto.V1.Requests;
using paskalON.Luna.Application;

namespace paskalON.Luna.Service.WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class DeviceController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        public readonly ILogger<DeviceController> _logger;


        /// <summary>
        /// Device manager for managing DER (Distributed Energy Resources) devices and their operations.
        /// </summary>
        public readonly ILunaDeviceManager _deviceManager;


        /// <summary>
        /// Device server for calling the device service API.
        /// </summary>
        private readonly IDeviceServer _deviceServer;


        /// <summary>
        /// Constructor of <see cref="DeviceController"/>.
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager for managing DER (Distributed Energy Resources) devices and their operations.</param>
        /// <param name="deviceServer">Device server for calling the device service API.</param>
        public DeviceController(ILogger<DeviceController> logger, ILunaDeviceManager deviceManager, IDeviceServer deviceServer)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);
            ArgumentNullException.ThrowIfNull(deviceServer);

            _logger = logger;
            _deviceManager = deviceManager;
            _deviceServer = deviceServer;
        }


        /// <summary>
        /// Gets the DER root with all its contents.
        /// </summary>
        /// <returns>The DER root object.</returns>
        [HttpGet]
        public DerDto GetDer()
        {
            return _deviceManager.GetDer();
        }


        /// <summary>
        /// Puts a DER unit into maintenance.
        /// </summary>
        /// <param name="request">The request containing the name of the DER unit.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> PutIntoMaintenance(PutIntoMaintenanceRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _deviceServer.PutIntoMaintenance(request.UnitName, cancellationToken));
        }


        /// <summary>
        /// Starts all power conversion systems (PCS) that are not in maintenance mode.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> StartAllPcs(CancellationToken cancellationToken)
        {
            return await Forward(() => _deviceServer.StartAllPcs(cancellationToken));
        }


        /// <summary>
        /// Starts a specific power conversion system (PCS).
        /// </summary>
        /// <param name="request">The request containing the device identifier of the PCS.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> StartPcs(StartPcsRequestDto request, CancellationToken cancellationToken)
        {
            return await Forward(() => _deviceServer.StartPcs(request.DeviceId, cancellationToken));
        }


        /// <summary>
        /// Stops all power conversion systems (PCS) that are not in maintenance mode.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> StopAllPcs(CancellationToken cancellationToken)
        {
            return await Forward(() => _deviceServer.StopAllPcs(cancellationToken));
        }


        /// <summary>
        /// Stops a specific power conversion system (PCS).
        /// </summary>
        /// <param name="request">The request containing the device identifier of the PCS.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> StopPcs(StopPcsRequestDto request, CancellationToken cancellationToken)
        {
            return await Forward(() => _deviceServer.StopPcs(request.DeviceId, cancellationToken));
        }


        /// <summary>
        /// Puts all power conversion systems (PCS) that are not in maintenance mode into standby.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> StandbyAllPcs(CancellationToken cancellationToken)
        {
            return await Forward(() => _deviceServer.StandbyAllPcs(cancellationToken));
        }


        /// <summary>
        /// Puts a specific power conversion system (PCS) into standby.
        /// </summary>
        /// <param name="request">The request containing the device identifier of the PCS.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> StandbyPcs(StandbyPcsRequestDto request, CancellationToken cancellationToken)
        {
            return await Forward(() => _deviceServer.StandbyPcs(request.DeviceId, cancellationToken));
        }


        /// <summary>
        /// Connects a specific battery bank (BB).
        /// </summary>
        /// <param name="request">The request containing the device identifier of the battery bank.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> ConnectBatteryBank(ConnectBbRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _deviceServer.ConnectBatteryBank(request.DeviceId, cancellationToken));
        }


        /// <summary>
        /// Disconnects a specific battery bank (BB).
        /// </summary>
        /// <param name="request">The request containing the device identifier of the battery bank.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> DisconnectBatteryBank(DisconnectBbRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _deviceServer.DisconnectBatteryBank(request.DeviceId, cancellationToken));
        }


        /// <summary>
        /// Forwards a request to the device service.
        /// </summary>
        /// <param name="request">The request to the device service.</param>
        /// <returns>Ok or the status code of the device service if it rejected the request.</returns>
        private async Task<IActionResult> Forward(Func<Task> request)
        {
            try
            {
                await request.Invoke();

                return Ok();
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                _logger.LogWarning(ex, "Device service rejected the request with status code {StatusCode}.", ex.StatusCode);

                return StatusCode((int)ex.StatusCode.Value);
            }
        }
    }
}

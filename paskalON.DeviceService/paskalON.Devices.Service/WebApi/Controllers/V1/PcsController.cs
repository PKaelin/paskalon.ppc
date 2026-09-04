// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Devices.Application;
using paskalON.Devices.Service.Dto.V1.Requests;

namespace paskalON.Devices.Service.WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class PcsController : ControllerBase
    {
        private readonly ILogger<PcsController> _logger;
        private readonly IDeviceManager _deviceManager;


        public PcsController(ILogger<PcsController> logger, IDeviceManager deviceManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);

            _logger = logger;
            _deviceManager = deviceManager;
        }


        [HttpPost]
        public async Task<IActionResult> StartAll()
        {
            // Dont wait for them all
            _ = Task.Run(() => _deviceManager.StartAllPcsAsync());

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Start(StartPcsRequestDto request)
        {
            await _deviceManager.StartPcsAsync(request.DeviceId);

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> StopAll()
        {
            // Dont wait for them all
            _ = Task.Run(() => _deviceManager.StopAllPcsAsync());

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Stop(StopPcsRequestDto request)
        {
            await _deviceManager.StopPcsAsync(request.DeviceId);

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> StandbyAll()
        {
            _ = Task.Run(() => _deviceManager.StandbyAllPcsAsync());

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Standby(StandbyPcsRequestDto request)
        {
            await _deviceManager.StandbyPcsAsync(request.DeviceId);

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> SetPowerTarget(SetPowerTargetRequest request)
        {
            await _deviceManager.SetPcsPowerTarget(request.DeviceId, request.ActivePowerWatt, request.ReactivePowerVar);

            return Ok();
        }
    }
}

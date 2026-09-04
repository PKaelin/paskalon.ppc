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
    public class BbController : ControllerBase
    {
        private readonly ILogger<BbController> _logger;
        private readonly IDeviceManager _deviceManager;


        public BbController(ILogger<BbController> logger, IDeviceManager deviceManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);

            _logger = logger;
            _deviceManager = deviceManager;
        }


        [HttpPost]
        public async Task<IActionResult> Connect(ConnectBbRequest request)
        {
            await _deviceManager.ConnectBatteryBankAsync(request.DeviceId);

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Disconnect(DisconnectBbRequest request)
        {
            await _deviceManager.DisconnectBatteryBankAsync(request.DeviceId);

            return Ok();
        }
    }
}

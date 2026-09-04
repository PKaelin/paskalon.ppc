
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
    public class DerController : ControllerBase
    {
        private readonly ILogger<DerController> _logger;
        private readonly IDeviceManager _deviceManager;


        public DerController(ILogger<DerController> logger, IDeviceManager deviceManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);

            _logger = logger;
            _deviceManager = deviceManager;
        }


        [HttpPost]
        public async Task<IActionResult> PutIntoMaintenance(PutIntoMaintenanceRequest request)
        {
            _deviceManager.PutIntoMaintenance(request.UnitName);

            return Ok();
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.DeviceSimulator.Application;
using paskalON.DeviceSimulator.Dto.Ders;
using paskalON.DeviceSimulator.Service.Dto.V1.Requests;

namespace paskalON.DeviceSimulator.Service.WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class DerController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DerController> _logger;


        /// <summary>
        /// Device manager for managing DER (Distributed Energy Resources) devices and their operations.
        /// </summary>
        private readonly IDeviceManagerSimulator _deviceManager;


        /// <summary>
        /// Constructor of <see cref="DerController"/>
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager for managing DER (Distributed Energy Resources).</param>
        public DerController(ILogger<DerController> logger, IDeviceManagerSimulator deviceManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);

            _logger = logger;
            _deviceManager = deviceManager;
        }


        /// <summary>
        /// Gets the DER root with all its contents.
        /// </summary>
        /// <returns>The DER root object.</returns>
        [HttpGet]
        public async Task<ActionResult<DerDto>> GetDer()
        {
            DeviceMapperSimulator mapper = new DeviceMapperSimulator(_deviceManager.StoreRegisters, _deviceManager.StreamRegisters);
            DerDto derDto = mapper.MapDer(_deviceManager.Der);

            return derDto;
        }


        /// <summary>
        /// Sets the expanded lists.
        /// </summary>
        /// <param name="request">The expanded request.</param>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> SetExpanded(SetExpandedRequest request)
        {
            if (request.ExpandedUnits != null)
            {
                _deviceManager.ExpandedUnits = request.ExpandedUnits;
            }

            if (request.ExpandedDevices != null)
            {
                _deviceManager.ExpandedDevices = request.ExpandedDevices;
            }

            return Ok();
        }
    }
}

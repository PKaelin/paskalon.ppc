// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.DeviceSimulator.Application;
using paskalON.DeviceSimulator.Service.Dto.Pcs;
using paskalON.DeviceSimulator.Service.Dto.V1.Requests;

namespace paskalON.DeviceSimulator.Service.WebApi.Controllers.V1
{
    /// <summary>
    /// Controls the behavior of the simulated devices (events, communication loss, response speed).
    /// </summary>
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class SimulationController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<SimulationController> _logger;


        /// <summary>
        /// Device manager of the simulated devices.
        /// </summary>
        private readonly IDeviceManagerSimulator _deviceManager;


        /// <summary>
        /// Constructor of <see cref="SimulationController"/>.
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager of the simulated devices.</param>
        public SimulationController(ILogger<SimulationController> logger, IDeviceManagerSimulator deviceManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);

            _logger = logger;
            _deviceManager = deviceManager;
        }


        /// <summary>
        /// Sets the warning, fault and vendor event bitmasks of a simulated PCS or battery bank.
        /// </summary>
        /// <param name="request">The events request.</param>
        /// <returns>Ok, NotFound when the device does not exist or BadRequest when the device has no such register.</returns>
        [HttpPost]
        public IActionResult SetEvents(SetEventsRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                _deviceManager.SetEvents(request.DeviceType, request.DeviceId, request.Warning, request.Fault, request.VendorEvent);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }


        /// <summary>
        /// Starts or stops answering Modbus requests of a simulated PCS or battery bank (communication loss).
        /// </summary>
        /// <param name="request">The communication request.</param>
        /// <returns>Ok or NotFound when the device does not exist.</returns>
        [HttpPost]
        public async Task<IActionResult> SetCommunication(SetCommunicationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                await _deviceManager.SetCommunicationAsync(request.DeviceType, request.DeviceId, request.IsResponding);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }


        /// <summary>
        /// Slows down the active and reactive power response of a simulated PCS.
        /// </summary>
        /// <param name="request">The slow down request.</param>
        /// <returns>Ok, NotFound when the PCS does not exist or BadRequest for negative values.</returns>
        [HttpPost]
        public IActionResult SlowDownPower(SlowDownPowerRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                _deviceManager.SlowDownPower(request.DeviceId, request.SlowActivePowerByPercent, request.SlowReactivePowerByPercent);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}

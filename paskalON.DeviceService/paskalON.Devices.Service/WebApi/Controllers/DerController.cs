
// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;

namespace paskalON.Devices.Service.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DerController : ControllerBase
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "Freezing", "Bracing" };
        }
    }
}

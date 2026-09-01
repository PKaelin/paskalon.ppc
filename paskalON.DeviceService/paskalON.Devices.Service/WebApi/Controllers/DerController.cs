
// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Devices.Infrastructure.Storage.Repositories;

namespace paskalON.Devices.Service.WebApi.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        /// <summary>
        /// Version repository.
        /// </summary>
        private IVersionRepository _repository;


        /// <summary>
        /// Constructor of <see cref="VersionController"/>.
        /// </summary>
        /// <param name="repository">The version repository.</param>
        public VersionController(IVersionRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            _repository = repository;
        }


        /// <summary>
        /// Gets the lates database version.
        /// </summary>
        /// <returns>Returns the latest database version.</returns>
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            string version = await _repository.GetDatabaseVersionAsync();

            return version;
        }
    }
}

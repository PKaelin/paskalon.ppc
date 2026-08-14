// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;

namespace paskalON.PowerControls.Domain.Strategies
{
    public class DistributionStrategyBase
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        protected readonly ILogger _logger;


        /// <summary>
        /// Constructor of <see cref="DistributionStrategyBase"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        public DistributionStrategyBase(ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }
    }
}
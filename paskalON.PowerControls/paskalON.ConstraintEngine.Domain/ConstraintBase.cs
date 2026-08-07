// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;

namespace paskalON.PowerControls.Domain.Constraints
{
    /// <summary>
    /// Base class for all constraints.
    /// </summary>
    public abstract class ConstraintBase
    {
        /// <summary>
        /// ILogger for handling application logging and diagnostics.
        /// </summary>
        protected readonly ILogger _logger;


        public ConstraintBase(ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }
    }
}

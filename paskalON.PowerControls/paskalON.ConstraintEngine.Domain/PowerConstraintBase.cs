// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs;

namespace paskalON.ConstraintEngine.Domain
{
    public abstract class PowerConstraintBase : ConstraintBase
    {
        public PowerConstraintBase(ILogger logger, ConstraintBaseConfig config, PowerConstraintBaseMap map)
            : base(logger, config, map)
        {
        }
    }
}

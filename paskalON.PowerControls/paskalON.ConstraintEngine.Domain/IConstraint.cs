// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain
{
    public interface IConstraint
    {
        string Name { get; }

        bool IsEnabled { get; }

        void ApplyLimits(ref ActivePower activePower, ref ReactivePower reactivePower);
    }
}

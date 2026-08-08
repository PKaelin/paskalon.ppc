// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Domain
{
    public abstract class PowerControlBaseMap
    {
        public required Func<DerState> State { get; set; }
    }
}

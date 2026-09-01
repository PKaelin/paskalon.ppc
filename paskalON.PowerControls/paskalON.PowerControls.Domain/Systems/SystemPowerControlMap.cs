// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Systems
{
    public class SystemPowerControlMap : PowerControlBaseMap
    {
        public required Func<SystemState> State { get; init; }
    }
}

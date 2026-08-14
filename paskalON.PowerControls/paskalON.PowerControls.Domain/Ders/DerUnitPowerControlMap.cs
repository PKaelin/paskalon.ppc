// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Ders
{
    public class DerUnitPowerControlMap : PowerControlBaseMap
    {
        public required Func<DerState> State { get; init; }
    }
}

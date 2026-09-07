// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Dto.Pcs
{
    public class SlowDownPowerRequest
    {
        public int DeviceId { get; set; }
        public double SlowActivePowerByPercent { get; set; }
        public double SlowReactivePowerByPercent { get; set; }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Describes the operations that can be carried out on the machine from a remote controller.
    /// </summary>
    public enum CircuitBreakerOperation
    {
        ReadOnly,
        TripOnly,
        ResetOnly,
        TripAndReset,
    }
}

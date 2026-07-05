// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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

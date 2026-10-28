// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// The device a client of the <see cref="DeviceHub"/> subscribed to.
    /// </summary>
    /// <param name="Type">Type of the device.</param>
    /// <param name="DeviceId">Device ID of the device.</param>
    public record DeviceSubscription(DeviceSubscriptionType Type, int DeviceId);
}

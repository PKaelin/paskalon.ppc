// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// Type of the device a client of the <see cref="DeviceHub"/> subscribed to.
    /// </summary>
    public enum DeviceSubscriptionType
    {
        /// <summary>
        /// A system power meter (PmSystemDto).
        /// </summary>
        SystemPowerMeter,


        /// <summary>
        /// An auxiliary power meter (PmAuxiliaryDto).
        /// </summary>
        AuxiliaryPowerMeter,


        /// <summary>
        /// An external power meter (PmExternalDto).
        /// </summary>
        ExternalPowerMeter,


        /// <summary>
        /// A power conversion system (PcsDto).
        /// </summary>
        PowerConversionSystem,


        /// <summary>
        /// A battery bank (BbDto).
        /// </summary>
        BatteryBank
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Alarms.Domain.Configs
{
    /// <summary>
    /// General configuration class for the microservice.
    /// </summary>
    /// <remarks>
    /// Each microservice might need some service configurations. E.g. AlarmExpirationDays, AlarmArchivalMonths, etc.
    /// To achieve this and still have separation a configuration table must be created for each microservice with a key/value.
    /// </remarks>
    public class Configuration : ConfigurationBase
    {
        // This is more a reminder that there is a configuration table.
    }
}

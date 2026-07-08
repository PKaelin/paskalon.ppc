// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Domains
{
    /// <summary>
    /// General configuration class.
    /// </summary>
    /// <remarks>
    /// For the microservice:
    /// Each microservice might need some service configurations. E.g. archive URL of aggregated warranty data, audit table retention span, etc.
    /// To achieve this and still have separation a configuration table must be created for each microservice with a key/value.
    /// For custom extension:
    /// A Domain configuration might have a required attribute that is need but should not be included in the standard.
    /// Hence custom configuration domains shall extend the standard domain:  [Domain] 1----* [DomainCustom]
    /// Type safety is sacrificed for the sake of cleaner structure.
    /// </remarks>
    public abstract class ConfigurationBase : DomainBase
    {

        /// <summary>
        /// Key of the configuration on which the value can be retrieved.
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// Value of the configuration can be converted to any type.
        /// </summary>
        public required string Value { get; set; }

        /// <summary>
        /// Description of the configuration entry.
        /// </summary>
        public string? Description { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace paskalON.Domains
{
    /// <summary>
    /// General configuration class for the microservice
    /// </summary>
    /// <remarks>
    /// Each microservice might need some service configurations. E.g. archive URL of aggregated warranty data, audit table retention span, etc.
    /// To achieve this and still have separation a configuration table must be created for each microservice with a key/value.
    /// </remarks>
    public abstract class ConfigurationBase : DomainBase
    {

        /// <summary>
        /// Key of the configuration on which the value can be retrieved.
        /// </summary>
        [Required, StringLength(100)]
        public required string Key { get; set; }

        /// <summary>
        /// Value of the configuration can be converted to any type.
        /// </summary>
        [Required, StringLength(500)]
        public required string Value { get; set; }

        /// <summary>
        /// Description of the configuration entry.
        /// </summary>
        [StringLength(800)]
        public string? Description { get; set; }
    }
}

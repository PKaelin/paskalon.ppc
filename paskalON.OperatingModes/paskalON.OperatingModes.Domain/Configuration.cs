using paskalON.Domains;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// General configuration class for the microservice
    /// </summary>
    /// <remarks>
    /// Each microservice might need some service configurations. E.g. archive URL of aggregated warranty data, audit table retention span, etc.
    /// To achieve this and still have separation a configuration table must be created for each microservice with a key/value.
    /// </remarks>
    public class Configuration : ConfigurationBase
    {
        // This design is more a reminder that a configuration option exists.
    }
}

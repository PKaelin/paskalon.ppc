using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs
{
    public abstract class NameBase : DomainBase
    {
        /// <summary>
        /// Name of the Distributed Energy Resource (DER).
        /// </summary>
        public required string Name { get; set; }

    }
}

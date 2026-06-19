using paskalON.Domains;
using System.ComponentModel.DataAnnotations;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Base class for all operating mode configurations.
    /// </summary>
    public abstract class OperatingModeConfigBase : DomainBase
    {
        [Required, StringLength(150)]
        public required string Name { get; set; }
    }
}

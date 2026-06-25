using Microsoft.EntityFrameworkCore;
using paskalON.Domains;
using System.ComponentModel.DataAnnotations;


namespace paskalON.Devices.Domain.Configs
{
    [Index(nameof(Name), IsUnique = true)]
    public abstract class NameBase : DomainBase
    {
        /// <summary>
        /// Name of the Distributed Energy Resource (DER).
        /// </summary>
        [Required]
        public required string Name { get; set; }

    }
}

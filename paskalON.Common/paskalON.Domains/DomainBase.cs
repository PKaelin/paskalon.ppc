using System.ComponentModel.DataAnnotations;

namespace paskalON.Domains
{
    /// <summary>
    /// Base class for all domain entities, providing common properties for tracking changes and identifying entities.
    /// </summary>
    public abstract class DomainBase
    {
        /// <summary>
        /// Unique identifier of the domain.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the user or system whom initiated the change.
        /// </summary>

        [Required, StringLength(250)]
        public required string ChangedBy { get; set; }


        /// <summary>
        /// When the change happened.
        /// </summary>
        [Required]
        public DateTimeOffset ChangedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace paskalON.Domains
{
    /// <summary>
    /// Defines the table the entity framework migration history data is stored.
    /// </summary>
    [Table("__EFMigrationsHistory")]
    public class History
    {
        /// <summary>
        /// Migration id of history
        /// </summary>
        [Key]
        public required string MigrationId { get; set; }


        /// <summary>
        /// Product version
        /// </summary>
        [Required]
        public required string ProductVersion { get; set; }
    }
}

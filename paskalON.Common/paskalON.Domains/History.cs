// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
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
        public required string MigrationId { get; set; }


        /// <summary>
        /// Product version
        /// </summary>
        public required string ProductVersion { get; set; }
    }
}

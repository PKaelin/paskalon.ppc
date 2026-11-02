// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Selects the database migration of a schema version.
    /// </summary>
    public static class SchemaVersionResolver
    {
        /// <summary>
        /// Finds the migration for the schema version.
        /// </summary>
        /// <param name="schemaVersion">Either a version like 1.0 which selects the migration ending in _v_1_0, a full migration id or null for the latest version.</param>
        /// <param name="migrations">The ids of the available migrations.</param>
        /// <returns>The migration id or null for the latest version.</returns>
        /// <exception cref="ClientDataArgumentException">Thrown when there is no migration for the schema version.</exception>
        public static string? Resolve(string? schemaVersion, IEnumerable<string> migrations)
        {
            ArgumentNullException.ThrowIfNull(migrations);

            if (string.IsNullOrWhiteSpace(schemaVersion))
            {
                return null;
            }

            string suffix = $"_v_{schemaVersion.Replace('.', '_')}";
            // Full or partial match of the migration id, e.g. 20261020230224_v_0_1 or 0.1
            string? migration = migrations.FirstOrDefault(x => x == schemaVersion || x.EndsWith(suffix, StringComparison.Ordinal));

            if (migration is null)
            {
                throw new ClientDataArgumentException($"There is no migration for schema version '{schemaVersion}'.");
            }

            return migration;
        }
    }
}

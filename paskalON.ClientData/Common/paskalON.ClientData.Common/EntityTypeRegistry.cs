// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Domains;

namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Finds the entity types a client project file can create for a database context.
    /// </summary>
    public static class EntityTypeRegistry
    {
        /// <summary>
        /// Creates the lookup of the entity types by their class name from the DbSet properties of the context.
        /// The migration history is not client data and therefore is excluded.
        /// </summary>
        /// <typeparam name="TContext">The database context type.</typeparam>
        /// <returns>The entity types by their class name.</returns>
        public static IReadOnlyDictionary<string, Type> FromContext<TContext>() where TContext : DbContext
        {
            return typeof(TContext).GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(x => x.PropertyType.GetGenericArguments()[0])
                .Where(x => x != typeof(History))
                .Distinct()
                .ToDictionary(x => x.Name, x => x, StringComparer.Ordinal);
        }
    }
}

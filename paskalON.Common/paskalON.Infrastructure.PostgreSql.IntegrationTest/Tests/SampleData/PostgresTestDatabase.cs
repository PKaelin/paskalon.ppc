// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Infrastructure.PostgreSql.Tests;

namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData
{
    /// <summary>
    /// A standalone <see cref="PostgresTestBase{T}"/> implementation that allows tests to drive
    /// the database initialization explicitly, e.g. to create several databases within one test.
    /// </summary>
    /// <typeparam name="T">The type of the DbContext.</typeparam>
    public class PostgresTestDatabase<T> : PostgresTestBase<T> where T : DbContext
    {
        /// <summary>
        /// The optional seeding logic executed during initialization.
        /// </summary>
        private readonly Func<T, Task>? _seed;


        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresTestDatabase{T}"/> class.
        /// </summary>
        /// <param name="seed">The optional seeding logic executed during initialization.</param>
        public PostgresTestDatabase(Func<T, Task>? seed = null)
        {
            _seed = seed;
        }


        /// <summary>
        /// Gets the connection string of the temporary database.
        /// </summary>
        public string DatabaseConnectionString => ConnectionString;


        /// <summary>
        /// Creates a new context for the temporary database.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public T CreateContext()
        {
            return CreateDbContext<T>();
        }


        /// <inheritdoc/>
        protected override Task SeedAsync(T db)
        {
            if (_seed is null)
            {
                return Task.CompletedTask;
            }

            return _seed(db);
        }
    }
}

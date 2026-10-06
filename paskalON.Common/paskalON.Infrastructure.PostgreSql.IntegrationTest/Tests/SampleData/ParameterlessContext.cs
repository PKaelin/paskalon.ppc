// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;

namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData
{
    /// <summary>
    /// A sample database context that does not provide a constructor accepting <see cref="DbContextOptions{TContext}"/>.
    /// </summary>
    public class ParameterlessContext : DbContext
    {
    }
}

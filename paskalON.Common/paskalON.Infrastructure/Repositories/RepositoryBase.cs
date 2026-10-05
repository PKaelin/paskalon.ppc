// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace paskalON.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TContext> where TContext : DbContext
    {
        /// <summary>
        /// ILogger for handling application logging and diagnostics.
        /// </summary>
        protected readonly ILogger _logger;


        /// <summary>
        /// Database context.
        /// </summary>
        protected TContext Context { get; private set; }


        /// <summary>
        /// Constructor for <see cref="RepositoryBase{TContext}"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="context">Database context.</param>
        public RepositoryBase(ILogger logger, TContext context)
        {
            _logger = logger;
            Context = context;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace paskalON.Devices.Infrastructure.Storage.Repositories
{
    public abstract class RepositoryBase<TContext> where TContext : DbContext
    {
        /// <summary>
        /// ILogger for handling application logging and diagnostics.
        /// </summary>
        protected ILogger _logger;


        /// <summary>
        /// Database context.
        /// </summary>
        protected TContext Context { get; private set; }


        /// <summary>
        /// Constructor for <see cref="RepositoryBase"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="context">Database context.</param>
        public RepositoryBase(ILogger logger, TContext context)
        {
            _logger = logger;
            Context = context;
        }


        /// <summary>
        /// Disposes managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Context.Dispose();
            // Prevents finalization overhead since resources are already freed
            GC.SuppressFinalize(this);
        }

    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Domain.Configs;
using paskalON.PowerControls.Infrastructure.Storage;

namespace paskalON.DemoSuperSimpleBattery.PowerControls.Data
{
    /// <summary>
    /// Create common data for the service here.
    /// </summary>
    static class CommonData
    {
        /// <summary>
        /// Initial changed by user.
        /// </summary>
        private const string _changedBy = "System Init";


        /// <summary>
        /// Main method to create the common data.
        /// </summary>
        /// <param name="context">DB context interface.</param>
        public static async Task CreateAsync(IPowerControlContext context)
        {
            context.Configurations.Add(new Configuration
            {
                Key = "AuditCleanUpInMonths",
                Value = "60",
                Description = "How long the updated records should be kept in the audit tables",
                ChangedBy = _changedBy
            });

            await context.SaveChangesAsync();
        }
    }
}

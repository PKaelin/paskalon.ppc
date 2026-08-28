// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Infrastructure.Storage;

namespace paskalON.DemoSimple.Devices.Data
{
    static class CommonData
    {
        private const string _changedBy = "System Init";

        public static void Create(IDeviceServiceContext context)
        {
            context.Configurations.Add(new Configuration
            {
                Key = "AuditCleanUpInMonths",
                Value = "60",
                Description = "How long the updated records should be kept in the audit tables",
                ChangedBy = _changedBy
            });

            // TODO: Add common data
        }
    }
}

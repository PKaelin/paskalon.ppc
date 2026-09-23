// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Telemetry;

namespace paskalON.PowerControls.Application
{
    public interface IPowerControlManager
    {
        ICollection<IMetricsPublisher> MetricsPublishers { get; }
    }
}

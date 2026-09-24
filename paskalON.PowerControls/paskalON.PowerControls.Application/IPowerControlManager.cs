// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Systems;
using paskalON.PowerControls.Domain.Systems;
using paskalON.PowerControls.Infrastructure.Storage;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Application
{
    public interface IPowerControlManager
    {
        ICollection<IMetricsPublisher> MetricsPublishers { get; }


        IReadOnlyList<SystemPowerControlConfig> SystemPowerControlConfigurations { get; }


        IReadOnlyList<DerUnitPowerControlConfig> DerUnitPowerControlConfigurations { get; }


        IReadOnlyList<DerUnitEnergyStoragePowerControlConfig> DerUnitEnergyStoragePowerControlConfigurations { get; }


        IReadOnlyList<ConstraintBaseConfig> ConstraintConfigurations { get; }


        SystemPowerControl? SystemPowerControl { get; }


        Task Initialize(PowerControlContext context);

        Task SetSystemPowerTarget(ActivePower activePower, ReactivePower reactivePower);
    }
}

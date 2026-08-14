// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Domain.Ders
{
    public class DerUnitPowerEnergyStorageControl : PowerControlBase
    {
        private readonly DerUnitPowerEnergyStorageControlConfig _config;
        private readonly DerUnitPowerEnergyStorageControlMap _map;
        private readonly IEnumerable<IDerUnitConstraint> _constraints;

        public DerUnitPowerEnergyStorageControl(ILogger logger, DerUnitPowerEnergyStorageControlConfig config, DerUnitPowerEnergyStorageControlMap map,
            IMetricsPublisher publisher, IEnumerable<IDerUnitConstraint> constraints)
            : base(logger, config, map, publisher)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(constraints);

            _config = config;
            _map = map;
            _constraints = constraints;
        }


        public override void UpdatePower(ActivePower activePower, ReactivePower reactivePower)
        {
            foreach (IDerUnitConstraint constraint in _constraints)
            {
                constraint.ApplyConstraints(ref activePower, ref reactivePower);
            }
        }
    }
}

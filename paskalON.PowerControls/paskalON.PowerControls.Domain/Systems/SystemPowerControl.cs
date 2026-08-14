// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Systems;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Domain.Systems
{
    public class SystemPowerControl : PowerControlBase
    {
        private readonly SystemPowerControlConfig _config;
        private readonly SystemPowerControlMap _map;
        private readonly IEnumerable<ISystemConstraint> _constraints;
        private ActivePower _actualSystemActivePowerTarget;
        private ReactivePower _actualSystemReactivePowerTarget;

        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        public ActivePower SystemActivePowerTarget
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        public ReactivePower SystemReactivePowerTarget
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        public ActivePower ActualSystemActivePowerTarget
        {
            get { lock (dataLock) { return _actualSystemActivePowerTarget; } }
            private set { lock (dataLock) { _actualSystemActivePowerTarget = value; } }
        }


        public ReactivePower ActualSystemReactivePowerTarget
        {
            get { lock (dataLock) { return _actualSystemReactivePowerTarget; } }
            private set { lock (dataLock) { _actualSystemReactivePowerTarget = value; } }
        }


        /// <summary>
        /// Maximum Active Power is the potential technical or nameplate limits of the system.
        /// </summary>
        public ActivePower MaximumActivePower { get; init; }


        /// <summary>
        /// Minimum Active Power is the potential technical or nameplate limits of the system.
        /// </summary>
        public ActivePower MinimumActivePower { get; init; }


        /// <summary>
        /// Maximum Reactive Power is the potential technical or nameplate limits of the system.
        /// </summary>
        public ReactivePower MaximumReactivePower { get; init; }


        /// <summary>
        /// Minimum Reactive Power is the potential technical or nameplate limits of the system.
        /// </summary>
        public ReactivePower MinimumReactivePower { get; init; }


        /// <summary>
        /// Indicates whether the system should derate per unit stopped.
        /// </summary>
        public bool DeratePerUnitStopped { get; init; }


        /// <summary>
        /// Indicates whether the system should derate per unit in maintenance.
        /// </summary>
        public bool DeratePerUnitInMaintenance { get; init; }




        public SystemPowerControl(ILogger logger, SystemPowerControlConfig config, SystemPowerControlMap map, IMetricsPublisher publisher, IEnumerable<ISystemConstraint> constraints)
            : base(logger, config, map, publisher)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(constraints);

            _config = config;
            _map = map;
            _constraints = constraints;
            DeratePerUnitInMaintenance = _config.SystemPowerConstraintConfig.DeratePerUnitInMaintenance;
            DeratePerUnitStopped = _config.SystemPowerConstraintConfig.DeratePerUnitStopped;
            MaximumActivePower = ActivePower.FromKilo(_config.SystemPowerConstraintConfig.MaximumActivePowerKiloWatt ?? 0);
            MinimumActivePower = ActivePower.FromKilo(_config.SystemPowerConstraintConfig.MinimumActivePowerKiloWatt ?? 0);
            MaximumReactivePower = ReactivePower.FromKilo(_config.SystemPowerConstraintConfig.MaximumReactivePowerKiloVars ?? 0);
            MinimumReactivePower = ReactivePower.FromKilo(_config.SystemPowerConstraintConfig.MinimumReactivePowerKiloVars ?? 0);
        }



        public override void UpdatePower(ActivePower activePower, ReactivePower reactivePower)
        {
            if (activePower.Watts != SystemActivePowerTarget.Watts || reactivePower.VoltAmperesReactivePrecision != SystemReactivePowerTarget.VoltAmperesReactive)
            {
                _logger.LogInformation("Update system power control. Active Power Kilo {ActivePower}, Reactive Power Kilo {ReactivePower}", activePower.KiloWatts, reactivePower.KiloVoltAmperesReactive);
                _actualSystemActivePowerTarget = new ActivePower(SystemActivePowerTarget.Watts);
                _actualSystemReactivePowerTarget = new(SystemReactivePowerTarget.VoltAmperesReactive);

                foreach (ISystemConstraint constraint in _constraints)
                {
                    constraint.ApplyConstraints(ref _actualSystemActivePowerTarget, ref _actualSystemReactivePowerTarget);
                }
            }
        }
    }
}

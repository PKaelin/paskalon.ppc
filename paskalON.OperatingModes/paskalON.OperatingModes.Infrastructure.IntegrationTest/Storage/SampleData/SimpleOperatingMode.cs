// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.EnergyStorages;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Curves;
using paskalON.OperatingModes.Domain.Configs.Modes.ComplexPower;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Ramps;

namespace paskalON.OperatingModes.Infrastructure.IntegrationTest.Storage.SampleData
{
    /// <summary>
    /// Used to create at least one entity of db set.
    /// </summary>
    public class SimpleOperatingMode
    {
        // Core
        public SystemConfig? SystemConfig { get; set; }

        // Ramps
        public RampRateConfig? RampRateConfig { get; set; }
        public RampRatePercentageConfig? RampRatePercentageConfig { get; set; }
        public RampTimeConfig? RampTimeConfig { get; set; }
        public RampTimeConstantConfig? RampTimeConstantConfig { get; set; }

        // Curves
        public FrequencyWattCurveConfig? FrequencyWattCurveConfig { get; set; }
        public CurvePointConfig? CurvePointConfigFWC1 { get; set; }
        public CurvePointConfig? CurvePointConfigFWC2 { get; set; }
        public VoltWattCurveConfig? VoltWattCurveConfig { get; set; }
        public CurvePointConfig? CurvePointConfigVWC1 { get; set; }
        public VoltVarCurveConfig? VoltVarCurveConfig { get; set; }
        public CurvePointConfig? CurvePointConfigVVC1 { get; set; }


        // Operating closed modes
        public MaintenanceSocModeConfig? MaintenanceSocModeConfig { get; set; }
        // Operating closed modes Energy Storages
        public ChargeDischargeModeConfig? ChargeDischargeModeConfig { get; set; }
        public CoordinatedChargeDischargeModeConfig? CoordinatedChargeDischargeModeConfig { get; set; }
        // Operating closed modes Frequency Actives
        public ActivePowerModeConfig? ActivePowerModeConfig { get; set; }
        public FrequencyDroopModeConfig? FrequencyDroopModeConfig { get; set; }
        public FrequencyWattModeConfig? FrequencyWattModeConfig { get; set; }
        public MaximumActivePowerLimitModeConfig? MaximumActivePowerLimitModeConfig { get; set; }
        // Operating closed modes Voltage Actives
        public VoltageWattDroopModeConfig? VoltageWattDroopModeConfig { get; set; }
        // Operating closed modes Voltage Reactives
        public PowerFactorModeConfig? PowerFactorModeConfig { get; set; }
        public ReactivePowerModeConfig? ReactivePowerModeConfig { get; set; }
        public VoltageModeConfig? VoltageModeConfig { get; set; }
        public VoltageVarDroopModeConfig? VoltageVarDroopModeConfig { get; set; }

        // Operating open modes
        public MaintenanceModeConfig? MaintenanceModeConfig { get; set; }
        // Operating open modes Energy Storages
        public MaximumPowerPointTrackingModeConfig? MaximumPowerPointTrackingModeConfig { get; set; }
        // Operating open modes Frequency Actives
        public ActivePowerFixedModeConfig? ActivePowerFixedModeConfig { get; set; }
        // Operating open modes Voltage Reactives
        public ReactivePowerFixedModeConfig? ReactivePowerFixedModeConfig { get; set; }


        /// <summary>
        /// Constructor that unusually creates all data.
        /// </summary>
        public SimpleOperatingMode()
        {
            CreateCore();
            CreateRamps();
            CreateCurves();
            CreateOperatingClosedModes();
            CreateOperatingOpenModes();
        }


        private void CreateCore()
        {
            SystemConfig = new SystemConfig
            {
                ChangedBy = "Test",
                Type = OperatingModeType.Bess,
                ReferenceFrequency = 50,
                NameplateMaximumActivePowerKiloWatt = 1000,
                NameplateMinimumActivePowerKiloWatt = -1000,
                NameplateMaximumReactivePowerKiloVars = 900,
                NameplateMinimumReactivePowerKiloVars = -900
            };
        }

        private void CreateRamps()
        {
            RampRateConfig = new RampRateConfig
            {
                ChangedBy = "Test",
                RampTimeSeconds = 1,
                RampUpRatePerSecond = 2,
                RampDownRatePerSecond = 3,
            };

            RampRatePercentageConfig = new RampRatePercentageConfig
            {
                ChangedBy = "Test",
                RampTimeSeconds = 1,
                RampUpRatePercentPerSecond = 2,
                RampDownRatePercentPerSecond = 3,
            };

            RampTimeConfig = new RampTimeConfig
            {
                ChangedBy = "Test",
                RampTimeSeconds = 1,
                RampUpTimeSeconds = 2,
                RampDownTimeSeconds = 3,
            };

            RampTimeConstantConfig = new RampTimeConstantConfig
            {
                ChangedBy = "Test",
                RampTimeSeconds = 1,
                RampUpTimeConstantSeconds = 2,
                RampDownTimeConstantSeconds = 3,
            };
        }

        private void CreateCurves()
        {
            FrequencyWattCurveConfig = new FrequencyWattCurveConfig
            {
                ChangedBy = "Test",
                Name = "FrequencyWattCurveConfig",
                UseRamp = false,
            };

            CurvePointConfigFWC1 = new CurvePointConfig
            {
                ChangedBy = "Test",
                CurveBaseConfig = FrequencyWattCurveConfig,
                X = 1.1,
                Y = 2.2,
            };

            CurvePointConfigFWC2 = new CurvePointConfig
            {
                ChangedBy = "Test",
                CurveBaseConfig = FrequencyWattCurveConfig,
                X = 1.11,
                Y = 2.22,
            };

            FrequencyWattCurveConfig.Points.Add(CurvePointConfigFWC1);
            FrequencyWattCurveConfig.Points.Add(CurvePointConfigFWC2);

            VoltWattCurveConfig = new VoltWattCurveConfig
            {
                ChangedBy = "Test",
                Name = "VoltWattCurveConfig",
                UseRamp = false,
            };

            CurvePointConfigVWC1 = new CurvePointConfig
            {
                ChangedBy = "Test",
                CurveBaseConfig = VoltWattCurveConfig,
                X = 11,
                Y = 22,
            };

            VoltVarCurveConfig = new VoltVarCurveConfig
            {
                ChangedBy = "Test",
                Name = "VoltVarCurveConfig",
                UseRamp = false,
            };

            CurvePointConfigVVC1 = new CurvePointConfig
            {
                ChangedBy = "Test",
                CurveBaseConfig = VoltVarCurveConfig,
                X = 111,
                Y = 222,
            };
        }


        private void CreateOperatingClosedModes()
        {
            ActivePowerModeConfig = new ActivePowerModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = RampRateConfig!,
            };

            ReactivePowerModeConfig = new ReactivePowerModeConfig
            {
                ChangedBy = "Test",
                Name = "ReactivePowerModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = RampRateConfig!,
            };
        }


        private void CreateOperatingOpenModes()
        {
            MaintenanceModeConfig = new MaintenanceModeConfig
            {
                ChangedBy = "Test",
                Name = "MaintenanceModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = RampRateConfig!,
            };

            MaximumPowerPointTrackingModeConfig = new MaximumPowerPointTrackingModeConfig
            {
                ChangedBy = "Test",
                Name = "MaximumPowerPointTrackingModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = RampRateConfig!,
            };

            ActivePowerFixedModeConfig = new ActivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerFixedModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = RampRateConfig!,
            };

            ReactivePowerFixedModeConfig = new ReactivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ReactivePowerFixedModeConfig",
                IsActive = true,
                Type = OperatingModeType.Bess,
                RampConfig = RampRateConfig!,
            };
        }



    }
}

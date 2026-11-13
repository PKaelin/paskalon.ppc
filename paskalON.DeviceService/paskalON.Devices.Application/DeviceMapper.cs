// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Devices.Application
{
    /// <summary>
    /// Maps domain device objects to their DTO representations.
    /// </summary>
    public class DeviceMapper
    {
        // TODO: Add GMDs

        /// <summary>
        /// Maps a DER domain object to a DER DTO.
        /// </summary>
        public DerDto MapDer(Der der)
        {
            ArgumentNullException.ThrowIfNull(der);

            DerDto dto = new DerDto
            {
                Name = der.Name,
                SystemPowerMeters = der.SystemPowerMeters.Select(MapSystemPowerMeter).ToList(),
                AuxiliaryPowerMeters = der.AuxiliaryPowerMeters.Select(MapAuxiliaryPowerMeter).ToList(),
                ExternalPowerMeters = der.ExternalPowerMeters.Select(MapExternalPowerMeter).ToList()
            };

            dto.DerGroups = der.DerGroups.Select(MapDerGroup).ToList();

            return dto;
        }


        /// <summary>
        /// Maps a DER group domain object to a DER group DTO.
        /// </summary>
        public DerGroupDto MapDerGroup(DerGroup group)
        {
            ArgumentNullException.ThrowIfNull(group);

            return new DerGroupDto { Name = group.Name, DerCircuits = group.DerCircuits.Select(MapDerCircuit).ToList() };
        }

        /// <summary>
        /// Maps a DER circuit domain object to a DER circuit DTO.
        /// </summary>
        public DerCircuitDto MapDerCircuit(DerCircuit circuit)
        {
            ArgumentNullException.ThrowIfNull(circuit);

            return new DerCircuitDto
            {
                Name = circuit.Name,
                DerUnits = circuit.DerUnits.Select(MapDerUnit).ToList(),
                CircuitPowerMeter = circuit.CircuitPowerMeter is null ? null : MapCircuitPowerMeter(circuit.CircuitPowerMeter)
            };
        }


        /// <summary>
        /// Maps a DER unit domain object to its concrete DER unit DTO.
        /// </summary>
        public DerUnitDto MapDerUnit(DerUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);

            return unit switch
            {
                DerBatteryStorageUnit batteryStorageUnit => MapDerBatteryStorageUnit(batteryStorageUnit),
                DerSolarUnit solarUnit => MapDerSolarUnit(solarUnit),
                _ => throw new InvalidOperationException($"Unsupported DER unit type '{unit.GetType().FullName}'.")
            };
        }


        /// <summary>
        /// Maps a battery storage unit domain object to a DTO.
        /// </summary>
        public DerBatteryStorageUnitDto MapDerBatteryStorageUnit(DerBatteryStorageUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);

            DerBatteryStorageUnitDto dto = new DerBatteryStorageUnitDto
            {
                Name = unit.Name,
                PowerConversionSystem = MapPowerConversionSystem(unit.PowerConversionSystem ?? throw new InvalidOperationException("Battery storage unit has no power conversion system.")),
                IncludeBatteryInOperations = unit.IncludeBatteryInOperations,
                IsInMaintenanceMode = unit.IsInMaintenanceMode,
                BatteryBanks = unit.BatteryBanks.Select(MapBatteryBank).ToList()
            };

            return dto;
        }


        /// <summary>
        /// Maps a solar unit domain object to a DTO.
        /// </summary>
        public DerSolarUnitDto MapDerSolarUnit(DerSolarUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);

            return new DerSolarUnitDto
            {
                Name = unit.Name,
                PowerConversionSystem = MapPowerConversionSystem(unit.PowerConversionSystem ?? throw new InvalidOperationException("Solar unit has no power conversion system.")),
                IsInMaintenanceMode = unit.IsInMaintenanceMode,
                SolarPanels = unit.SolarPanels.Select(MapSolarPanel).ToList(),
                NumberOfPanels = unit.NumberOfPanels
            };
        }


        /// <summary>
        /// Maps a PCS domain object, including its definition, core, and detail.
        /// </summary>
        public PcsDto MapPowerConversionSystem(PowerConversionSystemBase pcs)
        {
            ArgumentNullException.ThrowIfNull(pcs);

            PcsDto dto = new PcsDto(MapPowerConversionSystemDefinition(pcs));
            dto.UpdateCore(MapPowerConversionSystemCore(pcs));
            dto.UpdateDetail(MapPowerConversionSystemDetail(pcs));

            return dto;
        }


        /// <summary>
        /// Maps the PCS definition from a domain object.
        /// </summary>
        public PcsDefinitionDto MapPowerConversionSystemDefinition(PowerConversionSystemBase pcs)
        {
            ArgumentNullException.ThrowIfNull(pcs);

            return new PcsDefinitionDto
            {
                DeviceId = pcs.DeviceId,
                Name = pcs.Name,
                TargetAddress = pcs.TargetAddress,
                TargetPort = pcs.TargetPort,
                NameplateMaximumActivePower = pcs.NameplateMaximumActivePower,
                NameplateMaximumReactivePower = pcs.NameplateMaximumReactivePower,
                NameplateMaximumApparentPower = pcs.NameplateMaximumApparentPower,
                NameplateMaximumACCurrent = pcs.NameplateMaximumACCurrent,
                MinimumDCVoltage = pcs.MinimumDCVoltage,
                MaximumDCVoltage = pcs.MaximumDCVoltage,
                ZeroOutputOnCommLoss = pcs.ZeroOutputOnCommLoss,
                StandbyActivePowerWatts = pcs.StandbyActivePowerWatts
            };
        }


        /// <summary>
        /// Maps the PCS core from a domain object.
        /// </summary>
        public PcsCoreDto MapPowerConversionSystemCore(PowerConversionSystemBase pcs)
        {
            ArgumentNullException.ThrowIfNull(pcs);

            return new PcsCoreDto
            {
                DeviceId = pcs.DeviceId,
                State = MapPcsState(pcs.State),
                CommunicationError = pcs.CommunicationError,
                ActivePowerTarget = pcs.ActivePowerTarget,
                ActivePower = pcs.ActivePower,
                ActiveAvailablePower = pcs.ActiveAvailablePower,
                ActiveAvailablePowerMinimum = pcs.ActiveAvailablePowerMinimum,
                ActiveAvailablePowerMaximum = pcs.ActiveAvailablePowerMaximum,
                ReactivePowerTarget = pcs.ReactivePowerTarget,
                ReactivePower = pcs.ReactivePower,
                ReactiveAvailablePower = pcs.ReactiveAvailablePower,
                ApparentPower = pcs.ApparentPower,
                Frequency = pcs.Frequency,
                DCCurrent = pcs.DCCurrent,
                DCVoltage = pcs.DCVoltage,
                ACCurrent = pcs.ACCurrent,
                ACVoltage = pcs.ACVoltage
            };
        }


        /// <summary>
        /// Maps the PCS detail from a domain object.
        /// </summary>
        public PcsDetailDto MapPowerConversionSystemDetail(PowerConversionSystemBase pcs)
        {
            ArgumentNullException.ThrowIfNull(pcs);

            return new PcsDetailDto
            {
                DeviceId = pcs.DeviceId,
                IsInMaintenanceMode = pcs.IsInMaintenanceMode,
                IsACBreakerClosed = pcs.IsACBreakerClosed,
                IsDcContactorClosed = pcs.IsDcContactorClosed,
                FaultStates = new Dictionary<string, bool>(pcs.FaultStates),
                WarningStates = new Dictionary<string, bool>(pcs.WarningStates),
                VendorEvents = new Dictionary<string, bool>(pcs.VendorEvents)
            };
        }


        /// <summary>
        /// Maps a battery bank domain object, including its definition, core, and detail.
        /// </summary>
        public BbDto MapBatteryBank(BatteryBankBase batteryBank)
        {
            ArgumentNullException.ThrowIfNull(batteryBank);

            BbDto dto = new BbDto(MapBatteryBankDefinition(batteryBank));
            dto.UpdateCore(MapBatteryBankCore(batteryBank));
            dto.UpdateDetail(MapBatteryBankDetail(batteryBank));

            return dto;
        }


        /// <summary>
        /// Maps the battery bank definition from a domain object.
        /// </summary>
        public BbDefinitionDto MapBatteryBankDefinition(BatteryBankBase batteryBank)
        {
            ArgumentNullException.ThrowIfNull(batteryBank);

            return new BbDefinitionDto
            {
                DeviceId = batteryBank.DeviceId,
                Name = batteryBank.Name,
                TargetAddress = batteryBank.TargetAddress,
                TargetPort = batteryBank.TargetPort,
                InitiallyConnected = batteryBank.InitiallyConnected,
                NameplateCapacity = batteryBank.NameplateCapacity,
                NameplateMaximumChargeRate = batteryBank.NameplateMaximumChargeRate,
                NameplateMaximumDischargeRate = batteryBank.NameplateMaximumDischargeRate,
                RackCount = batteryBank.RackCount,
                ModulesPerRackCount = batteryBank.ModulesPerRackCount,
                InverterBusNumber = batteryBank.InverterBusNumber,
                AbsoluteMinimumStateOfCharge = batteryBank.AbsoluteMinimumStateOfCharge,
                AbsoluteMaximumStateOfCharge = batteryBank.AbsoluteMaximumStateOfCharge,
                AbsoluteMinimumTemperature = batteryBank.AbsoluteMinimumTemperature,
                AbsoluteMaximumTemperature = batteryBank.AbsoluteMaximumTemperature,
                UsableMinimumStateOfCharge = batteryBank.UsableMinimumStateOfCharge,
                UsableMaximumStateOfCharge = batteryBank.UsableMaximumStateOfCharge,
                PreferredMinimumStateOfCharge = batteryBank.PreferredMinimumStateOfCharge,
                PreferredMaximumStateOfCharge = batteryBank.PreferredMaximumStateOfCharge,
                PreferredMinimumTemperature = batteryBank.PreferredMinimumTemperature,
                PreferredMaximumTemperature = batteryBank.PreferredMaximumTemperature,
                AbsoluteMaxDischargeCurrentAmps = batteryBank.AbsoluteMaxDischargeCurrentAmps,
                AbsoluteMaxChargeCurrentAmps = batteryBank.AbsoluteMaxChargeCurrentAmps,
                MinimumDcVoltage = batteryBank.MinimumDcVoltage,
                MaximumDcVoltage = batteryBank.MaximumDcVoltage,
                ZeroCapacityOnCommLoss = batteryBank.ZeroCapacityOnCommLoss
            };
        }


        /// <summary>
        /// Maps the battery bank core from a domain object.
        /// </summary>
        public BbCoreDto MapBatteryBankCore(BatteryBankBase batteryBank)
        {
            ArgumentNullException.ThrowIfNull(batteryBank);

            return new BbCoreDto
            {
                DeviceId = batteryBank.DeviceId,
                State = MapBatteryBankState(batteryBank.State),
                CommunicationError = batteryBank.CommunicationError,
                BatteryBankFlowDirection = batteryBank.BatteryBankFlowDirection is null ? null : MapBatteryBankFlowDirection(batteryBank.BatteryBankFlowDirection.Value),
                StateOfCharge = batteryBank.StateOfCharge,
                UsableStateOfCharge = batteryBank.UsableStateOfCharge,
                AbsoluteStateOfCharge = batteryBank.AbsoluteStateOfCharge,
                TotalDCVoltage = batteryBank.TotalDCVoltage,
                TotalDCCurrent = batteryBank.TotalDCCurrent
            };
        }


        /// <summary>
        /// Maps the battery bank detail from a domain object.
        /// </summary>
        public BbDetailDto MapBatteryBankDetail(BatteryBankBase batteryBank)
        {
            ArgumentNullException.ThrowIfNull(batteryBank);

            return new BbDetailDto
            {
                DeviceId = batteryBank.DeviceId,
                IsInMaintenanceMode = batteryBank.IsInMaintenanceMode,
                StateOfHealth = batteryBank.StateOfHealth,
                MinimumCellVoltage = batteryBank.MinimumCellVoltage,
                MaximumCellVoltage = batteryBank.MaximumCellVoltage,
                MinimumRackTemperature = batteryBank.MinimumRackTemperature,
                MaximumRackTemperature = batteryBank.MaximumRackTemperature,
                AverageRackTemperature = batteryBank.AverageRackTemperature,
                MinimumStringTemperature = batteryBank.MinimumStringTemperature,
                MaximumStringTemperature = batteryBank.MaximumStringTemperature,
                AverageStringTemperature = batteryBank.AverageStringTemperature,
                FaultStates = new Dictionary<string, bool>(batteryBank.FaultStates),
                WarningStates = new Dictionary<string, bool>(batteryBank.WarningStates),
                VendorEvents = new Dictionary<string, bool>(batteryBank.VendorEvents)
            };
        }


        /// <summary>
        /// Maps a solar panel domain object, including its definition, core, and detail.
        /// </summary>
        public PvDto MapSolarPanel(SolarPanelBase solarPanel)
        {
            ArgumentNullException.ThrowIfNull(solarPanel);

            PvDto dto = new PvDto(MapSolarPanelDefinition(solarPanel));
            dto.UpdateCore(MapSolarPanelCore(solarPanel));
            dto.UpdateDetail(MapSolarPanelDetail(solarPanel));

            return dto;
        }


        /// <summary>
        /// Maps the solar panel definition from a domain object.
        /// </summary>
        public PvDefinitionDto MapSolarPanelDefinition(SolarPanelBase solarPanel)
        {
            ArgumentNullException.ThrowIfNull(solarPanel);

            return new PvDefinitionDto
            {
                DeviceId = solarPanel.DeviceId,
                Name = solarPanel.Name,
                TargetAddress = solarPanel.TargetAddress,
                TargetPort = solarPanel.TargetPort,
                NumberOfPanels = solarPanel.NumberOfPanels,
                MinimumVoltageSum = solarPanel.MinimumVoltageSum,
                MaximumVoltageSum = solarPanel.MaximumVoltageSum,
                MinimumCurrentSum = solarPanel.MinimumCurrentSum,
                MaximumCurrentSum = solarPanel.MaximumCurrentSum
            };
        }


        /// <summary>
        /// Maps the solar panel core from a domain object.
        /// </summary>
        public PvCoreDto MapSolarPanelCore(SolarPanelBase solarPanel)
        {
            ArgumentNullException.ThrowIfNull(solarPanel);

            return new PvCoreDto
            {
                DeviceId = solarPanel.DeviceId,
                State = MapSolarPanelState(solarPanel.State),
                CommunicationError = solarPanel.CommunicationError
            };
        }


        /// <summary>
        /// Maps the solar panel detail from a domain object.
        /// </summary>
        public PvDetailDto MapSolarPanelDetail(SolarPanelBase solarPanel)
        {
            ArgumentNullException.ThrowIfNull(solarPanel);

            return new PvDetailDto
            {
                DeviceId = solarPanel.DeviceId,
                IsInMaintenanceMode = solarPanel.IsInMaintenanceMode
            };
        }


        /// <summary>
        /// Maps a system power meter, including its definition, core, and detail.
        /// </summary>
        public PmSystemDto MapSystemPowerMeter(SystemPowerMeter meter)
        {
            ArgumentNullException.ThrowIfNull(meter);

            PmSystemDto dto = new PmSystemDto(MapPowerMeterDefinition<PmSystemDefinitionDto>(meter));
            dto.UpdateCore(MapPowerMeterCore<PmSystemCoreDto>(meter));
            dto.UpdateDetail(MapPowerMeterDetail<PmSystemDetailDto>(meter));

            return dto;
        }


        /// <summary>
        /// Maps an auxiliary power meter, including its definition, core, and detail.
        /// </summary>
        public PmAuxiliaryDto MapAuxiliaryPowerMeter(AuxiliaryPowerMeter meter)
        {
            ArgumentNullException.ThrowIfNull(meter);

            PmAuxiliaryDto dto = new PmAuxiliaryDto(MapPowerMeterDefinition<PmAuxiliaryDefinitionDto>(meter));
            dto.UpdateCore(MapPowerMeterCore<PmAuxiliaryCoreDto>(meter));
            dto.UpdateDetail(MapPowerMeterDetail<PmAuxiliaryDetailDto>(meter));

            return dto;
        }


        /// <summary>
        /// Maps an external power meter, including its definition, core, and detail.
        /// </summary>
        public PmExternalDto MapExternalPowerMeter(ExternalPowerMeter meter)
        {
            ArgumentNullException.ThrowIfNull(meter);

            PmExternalDto dto = new PmExternalDto(MapPowerMeterDefinition<PmExternalDefinitionDto>(meter));
            dto.UpdateCore(MapPowerMeterCore<PmExternalCoreDto>(meter));
            dto.UpdateDetail(MapPowerMeterDetail<PmExternalDetailDto>(meter));

            return dto;
        }


        /// <summary>
        /// Maps a circuit power meter, including its definition, core, and detail.
        /// </summary>
        public PmCircuitDto MapCircuitPowerMeter(CircuitPowerMeter meter)
        {
            ArgumentNullException.ThrowIfNull(meter);

            PmCircuitDto dto = new PmCircuitDto(MapPowerMeterDefinition<PmCircuitDefinitionDto>(meter));
            dto.UpdateCore(MapPowerMeterCore<PmCircuitCoreDto>(meter));
            dto.UpdateDetail(MapPowerMeterDetail<PmCircuitDetailDto>(meter));

            return dto;
        }


        /// <summary>
        /// Maps the power meter base definition from a domain object.
        /// </summary>
        public TDefinition MapPowerMeterDefinition<TDefinition>(PowerMeterBase meter)
            where TDefinition : PmDefinitionBase, new()
        {
            ArgumentNullException.ThrowIfNull(meter);

            return new TDefinition() with
            {
                DeviceId = meter.DeviceId,
                Name = meter.Name,
                TargetAddress = meter.TargetAddress,
                TargetPort = meter.TargetPort,
                IsReversePowerFlow = meter.IsReversePowerFlow,
                IsCurrentSigned = meter.IsCurrentSigned,
                PowerFactorStandard = meter.PowerFactorStandard
            };
        }


        /// <summary>
        /// Maps the power meter base core from a domain object.
        /// </summary>
        public TCore MapPowerMeterCore<TCore>(PowerMeterBase meter)
            where TCore : PmCoreBase, new()
        {
            ArgumentNullException.ThrowIfNull(meter);

            return new TCore() with
            {
                DeviceId = meter.DeviceId,
                State = MapPowerMeterState(meter.State),
                CommunicationError = meter.CommunicationError,
                ActivePower = meter.ActivePower,
                ReactivePower = meter.ReactivePower,
                ApparentPower = meter.ApparentPower,
                PowerFactor = meter.PowerFactor,
                Frequency = meter.Frequency
            };
        }


        /// <summary>
        /// Maps the power meter base detail from a domain object.
        /// </summary>
        public TDetail MapPowerMeterDetail<TDetail>(PowerMeterBase meter)
            where TDetail : PmDetailBase, new()
        {
            ArgumentNullException.ThrowIfNull(meter);

            return new TDetail() with
            {
                DeviceId = meter.DeviceId,
                VoltageLLAvg = meter.VoltageLLAvg,
                ActivePowerA = meter.ActivePowerA,
                ActivePowerB = meter.ActivePowerB,
                ActivePowerC = meter.ActivePowerC,
                ReactivePowerA = meter.ReactivePowerA,
                ReactivePowerB = meter.ReactivePowerB,
                ReactivePowerC = meter.ReactivePowerC,
                EnergyDelivered = meter.EnergyDelivered,
                EnergyReceived = meter.EnergyReceived,
                ReactiveEnergyDelivered = meter.ReactiveEnergyDelivered,
                ReactiveEnergyReceived = meter.ReactiveEnergyReceived
            };
        }


        /// <summary>
        /// Maps a PCS domain state to its DTO state.
        /// </summary>
        public PcsStateDto MapPcsState(PcsState state)
        {
            return state switch
            {
                PcsState.Unknown => PcsStateDto.Unknown,
                PcsState.Stopped => PcsStateDto.Stopped,
                PcsState.Starting => PcsStateDto.Starting,
                PcsState.Started => PcsStateDto.Started,
                PcsState.Stopping => PcsStateDto.Stopping,
                PcsState.Sleeping => PcsStateDto.Sleeping,
                PcsState.EnteringStandby => PcsStateDto.EnteringStandby,
                PcsState.Standby => PcsStateDto.Standby,
                PcsState.ExitingStandby => PcsStateDto.ExitingStandby,
                PcsState.NightMode => PcsStateDto.NightMode,
                PcsState.Fault => PcsStateDto.Fault,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported PCS state.")
            };
        }


        /// <summary>
        /// Maps a battery bank domain state to its DTO state.
        /// </summary>
        public BatteryBankStateDto MapBatteryBankState(BatteryBankState state)
        {
            return state switch
            {
                BatteryBankState.Unknown => BatteryBankStateDto.Unknown,
                BatteryBankState.Disconnected => BatteryBankStateDto.Disconnected,
                BatteryBankState.Connecting => BatteryBankStateDto.Connecting,
                BatteryBankState.Initializing => BatteryBankStateDto.Initializing,
                BatteryBankState.Connected => BatteryBankStateDto.Connected,
                BatteryBankState.Standby => BatteryBankStateDto.Standby,
                BatteryBankState.Disconnecting => BatteryBankStateDto.Disconnecting,
                BatteryBankState.SocProtection => BatteryBankStateDto.SocProtection,
                BatteryBankState.Fault => BatteryBankStateDto.Fault,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported battery bank state.")
            };
        }


        /// <summary>
        /// Maps a battery bank domain flow direction to its DTO flow direction.
        /// </summary>
        public BatteryBankFlowDirectionDto MapBatteryBankFlowDirection(BatteryBankFlowDirection flowDirection)
        {
            return flowDirection switch
            {
                BatteryBankFlowDirection.Idle => BatteryBankFlowDirectionDto.Idle,
                BatteryBankFlowDirection.Charging => BatteryBankFlowDirectionDto.Charging,
                BatteryBankFlowDirection.Discharging => BatteryBankFlowDirectionDto.Discharging,
                _ => throw new ArgumentOutOfRangeException(nameof(flowDirection), flowDirection, "Unsupported battery bank flow direction.")
            };
        }


        /// <summary>
        /// Maps a solar panel domain state to its DTO state.
        /// </summary>
        public SolarPanelStateDto MapSolarPanelState(SolarPanelState state)
        {
            return state switch
            {
                SolarPanelState.Disconnected => SolarPanelStateDto.Disconnected,
                SolarPanelState.Connecting => SolarPanelStateDto.Connecting,
                SolarPanelState.Connected => SolarPanelStateDto.Connected,
                SolarPanelState.Disconnecting => SolarPanelStateDto.Disconnecting,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported solar panel state.")
            };
        }


        /// <summary>
        /// Maps a power meter domain state to its DTO state.
        /// </summary>
        public PowerMeterStateDto MapPowerMeterState(PowerMeterState state)
        {
            return state switch
            {
                PowerMeterState.Unknown => PowerMeterStateDto.Unknown,
                PowerMeterState.Disconnected => PowerMeterStateDto.Disconnected,
                PowerMeterState.Connecting => PowerMeterStateDto.Connecting,
                PowerMeterState.Connected => PowerMeterStateDto.Connected,
                PowerMeterState.Disconnecting => PowerMeterStateDto.Disconnecting,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported power meter state.")
            };
        }
    }
}

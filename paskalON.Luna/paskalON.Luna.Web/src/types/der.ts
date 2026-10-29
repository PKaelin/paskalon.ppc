/**
 * TypeScript mirror of the DER DTOs (paskalON.Devices.Dto), only the parts shown in the system tab.
 */

import type { ActivePowerDto, ApparentPowerDto, EnergyDto, ReactiveEnergyDto, ReactivePowerDto } from './system';

/** A device (DeviceBase: PmSystemDto, PmAuxiliaryDto, PmExternalDto, PmCircuitDto, PcsDto, BbDto, PvDto). */
export interface DeviceDto {
    deviceId: number;
    name: string;
}

/** PowerMeterState (paskalON.Devices.Domain), by its numeric value. */
export const PowerMeterStateNames: Record<number, string> = {
    0: 'Unknown',
    1: 'Disconnected',
    2: 'Connecting',
    3: 'Connected',
    4: 'Disconnecting'
};

/** PmCoreBase, the frequently updated values of a power meter. Voltages in V, currents in A. */
export interface PmCoreDto {
    /** PowerMeterState - a number, or the name if the service serializes enums as strings. */
    state: number | string;
    communicationError: boolean;
    activePower: ActivePowerDto | null;
    reactivePower: ReactivePowerDto | null;
    apparentPower: ApparentPowerDto | null;
    powerFactor: number | null;
    frequency: number | null;
    voltageA: number | null;
    voltageB: number | null;
    voltageC: number | null;
    currentA: number | null;
    currentB: number | null;
    currentC: number | null;
}

/** PmDetailBase, the less frequently updated values of a power meter. */
export interface PmDetailDto {
    activePowerA: ActivePowerDto | null;
    activePowerB: ActivePowerDto | null;
    activePowerC: ActivePowerDto | null;
    reactivePowerA: ReactivePowerDto | null;
    reactivePowerB: ReactivePowerDto | null;
    reactivePowerC: ReactivePowerDto | null;
    energyDelivered: EnergyDto | null;
    energyReceived: EnergyDto | null;
    reactiveEnergyDelivered: ReactiveEnergyDto | null;
    reactiveEnergyReceived: ReactiveEnergyDto | null;
}

/** The power meter DTO types, all with a PmCoreBase core and a PmDetailBase detail. */
export type PowerMeterType = 'PmSystemDto' | 'PmAuxiliaryDto' | 'PmExternalDto';

/** True if the type is a power meter DTO type. */
export function isPowerMeterType(type: string): type is PowerMeterType {
    return type === 'PmSystemDto' || type === 'PmAuxiliaryDto' || type === 'PmExternalDto';
}

/** PmSystemDto, PmAuxiliaryDto or PmExternalDto. Core and detail are null until the device service delivered them. */
export interface PowerMeterDto extends DeviceDto {
    core: PmCoreDto | null;
    detail: PmDetailDto | null;
}

/** PcsState (paskalON.Devices.Domain), by its numeric value. */
export const PcsStateNames: Record<number, string> = {
    0: 'Unknown',
    1: 'Stopped',
    2: 'Starting',
    3: 'Started',
    4: 'Stopping',
    5: 'Sleeping',
    6: 'EnteringStandby',
    7: 'Standby',
    8: 'ExitingStandby',
    9: 'NightMode',
    99: 'Fault'
};

/** PcsCoreDto, the frequently updated values of a power conversion system. Currents in A, voltages in V. */
export interface PcsCoreDto {
    /** PcsState - a number, or the name if the service serializes enums as strings. */
    state: number | string;
    communicationError: boolean;
    activePowerTarget: ActivePowerDto | null;
    activePower: ActivePowerDto | null;
    activeAvailablePower: ActivePowerDto | null;
    reactivePowerTarget: ReactivePowerDto | null;
    reactivePower: ReactivePowerDto | null;
    reactiveAvailablePower: ReactivePowerDto | null;
    dcCurrent: number | null;
    dcVoltage: number | null;
    acCurrent: number | null;
    acVoltage: number | null;
}

/** PcsDetailDto, the less frequently updated values of a power conversion system. */
export interface PcsDetailDto {
    isInMaintenanceMode: boolean;
    isACBreakerClosed: boolean | null;
    /** Closed state of each DC contactor, null if the PCS does not report it. */
    isDcContactorClosed: boolean[] | null;
}

/** PcsDto, a power conversion system. Core and detail are null until the device service delivered them. */
export interface PcsDto extends DeviceDto {
    core: PcsCoreDto | null;
    detail: PcsDetailDto | null;
}

/** BatteryBankState (paskalON.Devices.Domain), by its numeric value. */
export const BatteryBankStateNames: Record<number, string> = {
    0: 'Unknown',
    1: 'Disconnected',
    2: 'Connecting',
    3: 'Initializing',
    4: 'Connected',
    5: 'Standby',
    6: 'Disconnecting',
    7: 'SocProtection',
    99: 'Fault'
};

/** BatteryBankFlowDirection (paskalON.Devices.Domain), by its numeric value. */
export const BatteryBankFlowDirectionNames: Record<number, string> = {
    0: 'Idle',
    1: 'Charging',
    2: 'Discharging'
};

/** BbCoreDto, the frequently updated values of a battery bank. State of charge in %, voltage in V, current in A. */
export interface BbCoreDto {
    /** BatteryBankState - a number, or the name if the service serializes enums as strings. */
    state: number | string;
    communicationError: boolean;
    /** BatteryBankFlowDirection - a number, or the name if the service serializes enums as strings. */
    batteryBankFlowDirection: number | string | null;
    stateOfCharge: number | null;
    totalDCVoltage: number | null;
    totalDCCurrent: number | null;
}

/** BbDetailDto, the less frequently updated values of a battery bank. State of health in %, cell voltages in V, temperatures in °C. */
export interface BbDetailDto {
    isInMaintenanceMode: boolean;
    stateOfHealth: number | null;
    minimumCellVoltage: number | null;
    maximumCellVoltage: number | null;
    minimumRackTemperature: number | null;
    maximumRackTemperature: number | null;
}

/** BbDto, a battery bank. Core and detail are null until the device service delivered them. */
export interface BbDto extends DeviceDto {
    core: BbCoreDto | null;
    detail: BbDetailDto | null;
}

/** DerBatteryStorageUnitDto, unit type discriminator "battery". */
export interface DerBatteryStorageUnitDto {
    unitType: 'battery';
    name: string;
    powerConversionSystem: PcsDto;
    batteryBanks: BbDto[];
}

/** DerSolarUnitDto, unit type discriminator "solar". */
export interface DerSolarUnitDto {
    unitType: 'solar';
    name: string;
    powerConversionSystem: PcsDto;
    solarPanels: DeviceDto[];
}

/** DerUnitDto, serialized polymorphic with the "unitType" discriminator. */
export type DerUnitDto = DerBatteryStorageUnitDto | DerSolarUnitDto;

/** DerCircuitDto. */
export interface DerCircuitDto {
    name: string;
    derUnits: DerUnitDto[];
    /** Optional circuit power meter. */
    circuitPowerMeter: DeviceDto | null;
}

/** DerGroupDto. */
export interface DerGroupDto {
    name: string;
    derCircuits: DerCircuitDto[];
}

/** DerDto, the DER configuration of the system. */
export interface DerDto {
    name: string;
    derGroups: DerGroupDto[];
    systemPowerMeters: PowerMeterDto[];
    auxiliaryPowerMeters: PowerMeterDto[];
    externalPowerMeters: PowerMeterDto[];
}

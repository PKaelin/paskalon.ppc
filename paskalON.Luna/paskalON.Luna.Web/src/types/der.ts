/**
 * TypeScript mirror of the DER DTOs (paskalON.Devices.Dto), only the parts shown in the object tree.
 */

/** A device (DeviceBase: PmSystemDto, PmAuxiliaryDto, PmExternalDto, PmCircuitDto, PcsDto, BbDto, PvDto). */
export interface DeviceDto {
    deviceId: number;
    name: string;
}

/** DerBatteryStorageUnitDto, unit type discriminator "battery". */
export interface DerBatteryStorageUnitDto {
    unitType: 'battery';
    name: string;
    powerConversionSystem: DeviceDto;
    batteryBanks: DeviceDto[];
}

/** DerSolarUnitDto, unit type discriminator "solar". */
export interface DerSolarUnitDto {
    unitType: 'solar';
    name: string;
    powerConversionSystem: DeviceDto;
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
    systemPowerMeters: DeviceDto[];
    auxiliaryPowerMeters: DeviceDto[];
    externalPowerMeters: DeviceDto[];
}

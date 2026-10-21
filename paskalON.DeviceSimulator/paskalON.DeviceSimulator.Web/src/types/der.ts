/*
 * TypeScript mirror of the DeviceSimulator DTO contract (paskalON.DeviceSimulator).
 *
 * Property names are camelCase because ASP.NET Core serializes with
 * JsonNamingPolicy.CamelCase by default. If your service is configured to keep the
 * PascalCase names, either switch the service to camelCase or set
 * "pascalCaseApi": true in public/config.json (see src/api/http.ts).
 */

/** Modbus data types. Serialized as string (JsonStringEnumConverter) or as number. */
export type ModbusDataType = string | number;

/** C37 signal types. Serialized as string (JsonStringEnumConverter) or as number. */
export type C37SignalType = string | number;

/** Marker for every device that can be addressed by the simulator. */
export interface IDevice {
    deviceId: number;
    name: string;
}

export interface ModbusEndpointDto {
    name: string;
    address: number;
    dataType: ModbusDataType;
    /** Parsed endpoint value - number, boolean or string depending on dataType. */
    value: unknown;
    isReadOnly: boolean;
}

export interface C37EndpointDto {
    name: string;
    endpoint: string;
    signalType: C37SignalType;
    value: number | null;
    isReadOnly: boolean;
}

/**
 * Device types of the simulator control API (SimulationDeviceType on the server).
 * Device ids are only unique within one device type.
 */
export const SimulationDeviceType = {
    PowerConversionSystem: 0,
    BatteryBank: 1
} as const;

export type SimulationDeviceTypeValue = (typeof SimulationDeviceType)[keyof typeof SimulationDeviceType];

/** Simulation state of a simulated PCS or battery bank. */
export interface SimulationDto {
    /** Serialized as number or string (JsonStringEnumConverter). */
    deviceType: number | string;
    /** Vendor independent state, e.g. Off, On, Standby, Fault, Disconnected, PreCharging, Connected. */
    state: string;
    /** False while a communication loss is simulated. */
    isResponding: boolean;
    hasWarningRegister: boolean;
    hasFaultRegister: boolean;
    hasVendorEventRegister: boolean;
    /** 16 bit masks - null when the device has no such register. */
    warning?: number | null;
    fault?: number | null;
    vendorEvent?: number | null;
    /** PCS only: maximum power change per second in percent of the nameplate (0 = instant). */
    slowActivePowerByPercent?: number | null;
    slowReactivePowerByPercent?: number | null;
    /** Battery bank only. */
    usableStateOfCharge?: number | null;
}

export interface PcsDto extends IDevice {
    endpoints: ModbusEndpointDto[];
    simulation?: SimulationDto | null;
}

export interface BbDto extends IDevice {
    endpoints: ModbusEndpointDto[];
    simulation?: SimulationDto | null;
}

export interface PvDto extends IDevice {
    endpoints: ModbusEndpointDto[];
}

export interface PmBaseDto extends IDevice {
    endpoints: C37EndpointDto[];
}

export type PmSystemDto = PmBaseDto;
export type PmAuxiliaryDto = PmBaseDto;
export type PmExternalDto = PmBaseDto;
export type PmCircuitDto = PmBaseDto;

/** Discriminator values of DerUnitDto (see [JsonPolymorphic] on the server). */
export type DerUnitType = 'battery' | 'solar';

export interface DerUnitBaseDto {
    unitType: DerUnitType;
    name: string;
    isInMaintenanceMode: boolean;
}

export interface DerBatteryStorageUnitDto extends DerUnitBaseDto {
    unitType: 'battery';
    powerConversionSystem: PcsDto;
    batteryBanks: BbDto[];
}

export interface DerSolarUnitDto extends DerUnitBaseDto {
    unitType: 'solar';
    powerConversionSystem: PcsDto;
    solarPanels: PvDto[];
    numberOfPanels: number;
}

export type DerUnitDto = DerBatteryStorageUnitDto | DerSolarUnitDto;

export interface DerCircuitDto {
    name: string;
    derUnits: DerUnitDto[];
    circuitPowerMeter?: PmCircuitDto | null;
}

export interface DerGroupDto {
    name: string;
    derCircuits: DerCircuitDto[];
}

export interface DerDto {
    name: string;
    derGroups: DerGroupDto[];
    systemPowerMeters: PmSystemDto[];
    auxiliaryPowerMeters: PmAuxiliaryDto[];
    externalPowerMeters: PmExternalDto[];
}

/** Type guards for the polymorphic unit. */
export function isBatteryUnit(unit: DerUnitDto): unit is DerBatteryStorageUnitDto {
    return unit.unitType === 'battery';
}

export function isSolarUnit(unit: DerUnitDto): unit is DerSolarUnitDto {
    return unit.unitType === 'solar';
}

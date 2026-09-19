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

export interface PcsDto extends IDevice {
    endpoints: ModbusEndpointDto[];
}

export interface BbDto extends IDevice {
    endpoints: ModbusEndpointDto[];
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

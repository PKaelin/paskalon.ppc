import type {
    C37EndpointDto,
    DerDto,
    DerUnitDto,
    ModbusEndpointDto,
    PmBaseDto,
    BbDto,
    PcsDto,
    PvDto
} from '@/types/der';
import { isBatteryUnit, isSolarUnit } from '@/types/der';

/**
 * Applies updates that arrive over SignalR to the loaded DerDto.
 *
 * Keys (as agreed with the service):
 *   unit   -> Name
 *   device -> DeviceId + Name
 *
 * Everything is merged IN PLACE. The endpoint objects themselves are reused whenever the
 * endpoint already exists, which means:
 *   - Vue only patches the changed cells instead of re-creating the grid, and
 *   - the input the user is currently typing in keeps its DOM node (and its focus).
 */

type ModbusDevice = PcsDto | BbDto | PvDto;
type AnyDevice = ModbusDevice | PmBaseDto;

/** Payload of the DeviceUpdated hub message. */
export interface DeviceUpdate {
    deviceId: number;
    name: string;
    endpoints: (ModbusEndpointDto | C37EndpointDto)[];
}

export function deviceKey(deviceId: number, name: string): string {
    return `${deviceId}::${name}`;
}

function isModbusEndpoint(
    endpoint: ModbusEndpointDto | C37EndpointDto
): endpoint is ModbusEndpointDto {
    return typeof (endpoint as ModbusEndpointDto).address === 'number';
}

/** Merges an endpoint list by endpoint name (plus address for Modbus). */
function mergeEndpoints(
    target: (ModbusEndpointDto | C37EndpointDto)[],
    source: (ModbusEndpointDto | C37EndpointDto)[]
): void {
    if (!Array.isArray(target) || !Array.isArray(source)) {
        return;
    }

    const index = new Map<string, ModbusEndpointDto | C37EndpointDto>();

    for (const endpoint of target) {
        index.set(endpointKey(endpoint), endpoint);
    }

    const seen = new Set<string>();

    for (const incoming of source) {
        const key = endpointKey(incoming);
        seen.add(key);

        const existing = index.get(key);

        if (!existing) {
            target.push(incoming);
            continue;
        }

        // Update in place - do not replace the object.
        existing.value = incoming.value as never;
        existing.isReadOnly = incoming.isReadOnly;

        if (isModbusEndpoint(existing) && isModbusEndpoint(incoming)) {
            existing.address = incoming.address;
            existing.dataType = incoming.dataType;
        } else if (!isModbusEndpoint(existing) && !isModbusEndpoint(incoming)) {
            existing.signalType = incoming.signalType;
        }
    }

    // Drop endpoints the service no longer reports.
    for (let i = target.length - 1; i >= 0; i--) {
        const endpoint = target[i];

        if (endpoint && !seen.has(endpointKey(endpoint))) {
            target.splice(i, 1);
        }
    }
}

function endpointKey(endpoint: ModbusEndpointDto | C37EndpointDto): string {
    return isModbusEndpoint(endpoint) ? `m:${endpoint.address}:${endpoint.name}` : `c:${endpoint.name}`;
}

/** Walks the DerDto and returns every device indexed by deviceId + name. */
export function indexDevices(der: DerDto): Map<string, AnyDevice> {
    const index = new Map<string, AnyDevice>();

    const add = (device: AnyDevice | null | undefined): void => {
        if (device) {
            index.set(deviceKey(device.deviceId, device.name), device);
        }
    };

    for (const group of der.derGroups ?? []) {
        for (const circuit of group.derCircuits ?? []) {
            for (const unit of circuit.derUnits ?? []) {
                add(unit.powerConversionSystem);

                if (isBatteryUnit(unit)) {
                    (unit.batteryBanks ?? []).forEach(add);
                } else if (isSolarUnit(unit)) {
                    (unit.solarPanels ?? []).forEach(add);
                }
            }

            add(circuit.circuitPowerMeter);
        }
    }

    (der.systemPowerMeters ?? []).forEach(add);
    (der.auxiliaryPowerMeters ?? []).forEach(add);
    (der.externalPowerMeters ?? []).forEach(add);

    return index;
}

/** Returns every unit of the DerDto indexed by its name. */
export function indexUnits(der: DerDto): Map<string, DerUnitDto> {
    const index = new Map<string, DerUnitDto>();

    for (const group of der.derGroups ?? []) {
        for (const circuit of group.derCircuits ?? []) {
            for (const unit of circuit.derUnits ?? []) {
                index.set(unit.name, unit);
            }
        }
    }

    return index;
}

/**
 * Applies a single device update. Returns false when the device is unknown, which tells
 * the caller that a full reload is needed (the configuration has changed).
 */
export function applyDeviceUpdate(der: DerDto, update: DeviceUpdate): boolean {
    const target = indexDevices(der).get(deviceKey(update.deviceId, update.name));

    if (!target) {
        return false;
    }

    mergeEndpoints(target.endpoints, update.endpoints ?? []);
    return true;
}

/**
 * Applies a whole unit (DerUnitDto). Scalar properties are copied, the PCS and the
 * battery banks / solar panels below the unit are merged as devices.
 */
export function applyUnitUpdate(der: DerDto, update: DerUnitDto): boolean {
    const target = indexUnits(der).get(update.name);

    if (!target) {
        return false;
    }

    target.isInMaintenanceMode = update.isInMaintenanceMode;

    if (update.powerConversionSystem && target.powerConversionSystem) {
        mergeDevice(target.powerConversionSystem, update.powerConversionSystem);
    }

    if (isBatteryUnit(target) && isBatteryUnit(update)) {
        mergeDeviceList(target.batteryBanks, update.batteryBanks ?? []);
    } else if (isSolarUnit(target) && isSolarUnit(update)) {
        target.numberOfPanels = update.numberOfPanels;
        mergeDeviceList(target.solarPanels, update.solarPanels ?? []);
    }

    return true;
}

function mergeDevice(target: AnyDevice, source: AnyDevice): void {
    mergeEndpoints(target.endpoints, source.endpoints ?? []);
}

function mergeDeviceList<T extends AnyDevice>(target: T[], source: T[]): void {
    if (!Array.isArray(target)) {
        return;
    }

    const index = new Map(target.map((device) => [deviceKey(device.deviceId, device.name), device]));

    for (const incoming of source) {
        const existing = index.get(deviceKey(incoming.deviceId, incoming.name));

        if (existing) {
            mergeDevice(existing, incoming);
        } else {
            target.push(incoming);
        }
    }
}

/**
 * Applies a complete DerDto (hub message "DerUpdated"). The structure is replaced only
 * where it really changed, so expanded sections and focused inputs survive.
 */
export function applyDerUpdate(der: DerDto, update: DerDto): boolean {
    const units = indexUnits(update);
    const devices = indexDevices(update);

    let applied = true;
    const ownUnits = indexUnits(der);

    for (const [name, unit] of units) {
        if (ownUnits.has(name)) {
            applyUnitUpdate(der, unit);
        } else {
            applied = false;
        }
    }

    const ownDevices = indexDevices(der);

    for (const [key, device] of devices) {
        const target = ownDevices.get(key);

        if (target) {
            mergeDevice(target, device);
        } else {
            applied = false;
        }
    }

    return applied;
}

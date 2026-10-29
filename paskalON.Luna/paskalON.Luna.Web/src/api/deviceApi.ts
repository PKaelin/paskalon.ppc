import { absoluteUrl, config } from './config';
import type { DerDto } from '../types/der';

/** Gets the DER configuration (GET /v1/device/getder). Throws if the request fails. */
export async function getDer(signal?: AbortSignal): Promise<DerDto> {
    const response = await fetch(`${config().apiBaseUrl}/v1/device/getder`, { signal, headers: { Accept: 'application/json' } });

    if (response.ok === false) {
        throw new Error(`The DER configuration could not be loaded (HTTP ${response.status}).`);
    }

    return (await response.json()) as DerDto;
}

/**
 * Posts a device command to the Luna web service (POST /v1/device/{action}), which forwards it to the device service.
 * Throws if the service cannot be reached or rejects the command.
 */
async function postDeviceCommand(action: string, body: object): Promise<void> {
    let response: Response;

    try {
        response = await fetch(`${config().apiBaseUrl}/v1/device/${action}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });
    } catch (error) {
        const reason = error instanceof Error ? error.message : String(error);
        throw new Error(`The Luna service could not be reached: ${reason}`);
    }

    if (response.ok === false) {
        throw new Error(`The command was rejected (HTTP ${response.status}).`);
    }
}

/** Puts a DER unit into maintenance (intoMaintenance = true) or takes it out of maintenance (false). */
export function setMaintenance(unitName: string, intoMaintenance: boolean): Promise<void> {
    return postDeviceCommand('setmaintenance', { unitName, intoMaintenance });
}

/** Starts a PCS. */
export function startPcs(deviceId: number): Promise<void> {
    return postDeviceCommand('startpcs', { deviceId });
}

/** Stops a PCS. */
export function stopPcs(deviceId: number): Promise<void> {
    return postDeviceCommand('stoppcs', { deviceId });
}

/** Puts a PCS into standby. */
export function standbyPcs(deviceId: number): Promise<void> {
    return postDeviceCommand('standbypcs', { deviceId });
}

/** Connects a battery bank. */
export function connectBatteryBank(deviceId: number): Promise<void> {
    return postDeviceCommand('connectbatterybank', { deviceId });
}

/** Disconnects a battery bank. */
export function disconnectBatteryBank(deviceId: number): Promise<void> {
    return postDeviceCommand('disconnectbatterybank', { deviceId });
}

/** Absolute URL of the device hub (resolves a relative path against the current origin). */
export function deviceHubUrl(): string {
    return absoluteUrl(config().deviceHubUrl);
}

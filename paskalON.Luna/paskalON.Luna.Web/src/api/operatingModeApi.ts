import { config } from './config';
import type { OperatingModeStackDto, SystemSetpointDto } from '../types/operatingModes';

/** Error of a request to the Luna web service, with the HTTP status (0 if the service was not reached). */
export class ApiError extends Error {
    constructor(message: string, public readonly status: number) {
        super(message);
        this.name = 'ApiError';
    }
}

function url(action: string): string {
    return `${config().apiBaseUrl}/v1/operatingmode/${action}`;
}

async function request(action: string, init?: RequestInit): Promise<Response> {
    let response: Response;

    try {
        response = await fetch(url(action), init);
    } catch (error) {
        const reason = error instanceof Error ? error.message : String(error);
        throw new ApiError(`The Luna service could not be reached: ${reason}`, 0);
    }

    if (!response.ok) {
        throw new ApiError(`The Luna service answered with HTTP ${response.status}.`, response.status);
    }

    return response;
}

async function getJson<T>(action: string, signal?: AbortSignal): Promise<T> {
    const response = await request(action, { signal, headers: { Accept: 'application/json' } });

    return (await response.json()) as T;
}

/** GET /v1/operatingmode/getavailableoperatingmodes - the stackable operating modes that can be added to the stack. */
export function getAvailableOperatingModes(signal?: AbortSignal): Promise<OperatingModeStackDto[]> {
    return getJson<OperatingModeStackDto[]>('getavailableoperatingmodes', signal);
}

/** GET /v1/operatingmode/getactiveoperatingmodes - the operating modes of the stack in ascending priority order. */
export function getActiveOperatingModes(signal?: AbortSignal): Promise<OperatingModeStackDto[]> {
    return getJson<OperatingModeStackDto[]>('getactiveoperatingmodes', signal);
}

/** GET /v1/operatingmode/getsetpoint - the supervisory setpoints of the system in watts and vars. */
export function getSetpoint(signal?: AbortSignal): Promise<SystemSetpointDto> {
    return getJson<SystemSetpointDto>('getsetpoint', signal);
}

/** POST /v1/operatingmode/setsetpoint - sets the supervisory setpoints of the system in watts and vars. */
export async function setSetpoint(setpoint: SystemSetpointDto): Promise<void> {
    await request('setsetpoint', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(setpoint)
    });
}

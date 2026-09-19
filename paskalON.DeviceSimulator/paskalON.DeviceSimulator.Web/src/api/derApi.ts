import { config } from './config';
import type { DerDto } from '@/types/der';

export class ApiError extends Error {
    public readonly status: number;
    public readonly url: string;

    constructor(message: string, status: number, url: string) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.url = url;
    }
}

/**
 * Recursively converts PascalCase keys to camelCase.
 * Only used when config().pascalCaseApi is true.
 */
function toCamelCase(value: unknown): unknown {
    if (Array.isArray(value)) {
        return value.map(toCamelCase);
    }

    if (value !== null && typeof value === 'object') {
        const result: Record<string, unknown> = {};

        for (const [key, item] of Object.entries(value as Record<string, unknown>)) {
            const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
            result[camelKey] = toCamelCase(item);
        }

        return result;
    }

    return value;
}

function url(path: string): string {
    const base = config().apiBaseUrl.replace(/\/+$/, '');
    const suffix = path.startsWith('/') ? path : `/${path}`;
    return `${base}${suffix}`;
}

/**
 * GET {apiBaseUrl}/v1/der/getder
 */
export async function getDer(signal?: AbortSignal): Promise<DerDto> {
    const requestUrl = url(config().derEndpoint);

    const response = await fetch(requestUrl, {
        method: 'GET',
        headers: { Accept: 'application/json' },
        cache: 'no-store',
        signal
    });

    if (!response.ok) {
        throw new ApiError(
            `The web service answered with ${response.status} ${response.statusText}.`,
            response.status,
            requestUrl
        );
    }

    const payload = (await response.json()) as unknown;
    const normalized = config().pascalCaseApi ? toCamelCase(payload) : payload;

    return normalized as DerDto;
}

/** Identifies a single endpoint for the write-back call. */
export interface EndpointWriteRequest {
    /** Device id of the owning device (PCS, battery bank, PV panel, power meter). */
    deviceId: number;
    /** Endpoint name. */
    name: string;
    /** Modbus address - undefined for C37 endpoints. */
    address?: number;
    /** The new value. */
    value: unknown;
}

/**
 * POST {apiBaseUrl}/v1/der/setendpointvalue
 *
 * Disabled by default (config.enableWriteBack === false) because the write-back
 * contract is not part of the supplied DTO definition. Enable it in config.json as
 * soon as the web service exposes the endpoint, and adjust writeBackEndpoint/the body
 * below to match the real contract.
 */
export async function writeEndpointValue(request: EndpointWriteRequest): Promise<void> {
    if (!config().enableWriteBack) {
        return;
    }

    const requestUrl = url(config().writeBackEndpoint);

    const response = await fetch(requestUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
        body: JSON.stringify(request)
    });

    if (!response.ok) {
        throw new ApiError(
            `Writing "${request.name}" failed with ${response.status} ${response.statusText}.`,
            response.status,
            requestUrl
        );
    }
}

/**
 * Body of POST {apiBaseUrl}/v1/der/setexpanded
 *
 * Mirrors the server side SetExpandedRequest:
 *   public class SetExpandedRequest
 *   {
 *       public List<string> ExpandedUnits   { get; set; } = new List<string>();
 *       public List<string> ExpandedDevices { get; set; } = new List<string>();
 *   }
 *
 * The lists always contain ALL currently expanded units/devices - not a delta - so the
 * service can simply replace its subscription set with what it receives.
 */
export interface SetExpandedRequest {
    expandedUnits: string[];
    expandedDevices: string[];
}

/**
 * Tells the web service which units and devices are currently expanded in the UI, so it
 * only has to simulate/push values for what the user can actually see.
 */
export async function setExpanded(
    request: SetExpandedRequest,
    signal?: AbortSignal
): Promise<void> {
    if (!config().enableExpansionSync) {
        return;
    }

    const requestUrl = url(config().setExpandedEndpoint);

    const response = await fetch(requestUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
        body: JSON.stringify(request),
        signal
    });

    if (!response.ok) {
        throw new ApiError(
            `Sending the expanded sections failed with ${response.status} ${response.statusText}.`,
            response.status,
            requestUrl
        );
    }
}

/** Absolute URL of the SignalR hub (resolves a relative path against the current origin). */
export function hubUrl(): string {
    const configured = config().signalRHubUrl;

    if (/^https?:\/\//i.test(configured)) {
        return configured;
    }

    return new URL(configured, window.location.origin).toString();
}

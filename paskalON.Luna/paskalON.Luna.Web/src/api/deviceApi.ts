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

/** Absolute URL of the device hub (resolves a relative path against the current origin). */
export function deviceHubUrl(): string {
    return absoluteUrl(config().deviceHubUrl);
}

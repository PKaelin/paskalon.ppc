import { absoluteUrl, config } from './config';
import type { SystemNameplateDto } from '../types/system';

/** The system commands of the Luna SystemController. */
export type SystemCommand = 'SystemStartCommand' | 'SystemStopCommand' | 'ClearShutdownCommand';

/** Outcome of a system command as shown to the operator. */
export interface SystemCommandOutcome {
    /** True if the web service accepted the command. */
    ok: boolean;
    /** Message of the web service, or a description of the failure. */
    message: string;
}

/** Body of the SystemController responses (SystemCommandResponseDto). */
interface SystemCommandResponse {
    message?: string;
}

/**
 * Sends a system command to the Luna web service (POST /v1/system/{command}).
 * Never throws - a network failure or a rejected command is returned as outcome with ok = false.
 */
export async function sendSystemCommand(command: SystemCommand): Promise<SystemCommandOutcome> {
    let response: Response;

    try {
        response = await fetch(`${config().apiBaseUrl}/v1/system/${command.toLowerCase()}`, { method: 'POST' });
    } catch (error) {
        const reason = error instanceof Error ? error.message : String(error);
        return { ok: false, message: `The Luna service could not be reached: ${reason}` };
    }

    const body = await readBody(response);
    const message = body?.message ?? (response.ok ? 'Command sent.' : `The command failed (HTTP ${response.status}).`);

    return { ok: response.ok, message };
}

/** Gets the system nameplate ratings (GET /v1/system/getnameplate). Throws if the request fails. */
export async function getNameplate(signal?: AbortSignal): Promise<SystemNameplateDto> {
    const response = await fetch(`${config().apiBaseUrl}/v1/system/getnameplate`, { signal });

    if (response.ok === false) {
        throw new Error(`The system nameplate could not be loaded (HTTP ${response.status}).`);
    }

    return (await response.json()) as SystemNameplateDto;
}

/** Absolute URL of the system hub (resolves a relative path against the current origin). */
export function systemHubUrl(): string {
    return absoluteUrl(config().systemHubUrl);
}

async function readBody(response: Response): Promise<SystemCommandResponse | null> {
    try {
        return (await response.json()) as SystemCommandResponse;
    } catch {
        // Empty or non JSON body (e.g. a proxy error page).
        return null;
    }
}

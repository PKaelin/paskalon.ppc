import { absoluteUrl, config } from './config';
import type { LocalizedTextsDto, PpcAlarmDto, PpcAlarmFilter } from '../types/alarm';

/** Absolute URL of the alarm hub (resolves a relative path against the current origin). */
export function alarmHubUrl(): string {
    return absoluteUrl(config().alarmHubUrl);
}

/** The "detail" of a problem details response, or null if there is none. */
async function readProblemDetail(response: Response): Promise<string | null> {
    try {
        const problem = (await response.json()) as { detail?: unknown };

        return typeof problem.detail === 'string' ? problem.detail : null;
    } catch {
        return null;
    }
}

/**
 * GET /v1/alarm/getppcalarms - the PPC alarms triggered within the period that match the filter, the latest triggered first.
 * Only the set criteria are sent. Throws if the service cannot be reached or answers with an error.
 */
export async function getPpcAlarms(filter: PpcAlarmFilter, signal?: AbortSignal): Promise<PpcAlarmDto[]> {
    const query = new URLSearchParams({ triggeredWithinHours: String(filter.triggeredWithinHours) });

    if (filter.severityLevel !== null) {
        query.set('severityLevel', String(filter.severityLevel));
    }

    if (filter.alarmSequenceState !== null) {
        query.set('alarmSequenceState', String(filter.alarmSequenceState));
    }

    if (filter.deviceName.trim() !== '') {
        query.set('deviceName', filter.deviceName.trim());
    }

    let response: Response;

    try {
        response = await fetch(`${config().apiBaseUrl}/v1/alarm/getppcalarms?${query.toString()}`, {
            signal,
            headers: { Accept: 'application/json' }
        });
    } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
            throw error;
        }

        const reason = error instanceof Error ? error.message : String(error);
        throw new Error(`The Luna service could not be reached: ${reason}`);
    }

    if (response.ok === false) {
        throw new Error(await readProblemDetail(response) ?? `The alarms could not be loaded (HTTP ${response.status}).`);
    }

    return (await response.json()) as PpcAlarmDto[];
}

/**
 * GET /v1/alarm/getlocalizedtexts - the texts of the localization keys of the alarm configurations in the language.
 * Throws if the service cannot be reached or answers with an error.
 */
export async function getAlarmTexts(language: string, signal?: AbortSignal): Promise<LocalizedTextsDto> {
    const query = new URLSearchParams({ language });
    let response: Response;

    try {
        response = await fetch(`${config().apiBaseUrl}/v1/alarm/getlocalizedtexts?${query.toString()}`, {
            signal,
            headers: { Accept: 'application/json' }
        });
    } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
            throw error;
        }

        const reason = error instanceof Error ? error.message : String(error);
        throw new Error(`The Luna service could not be reached: ${reason}`);
    }

    if (response.ok === false) {
        throw new Error(await readProblemDetail(response) ?? `The alarm texts could not be loaded (HTTP ${response.status}).`);
    }

    return (await response.json()) as LocalizedTextsDto;
}

/**
 * POST /v1/alarm/acknowledgealarms - acknowledges the alarms, alarms that cannot be acknowledged
 * (not acknowledgeable or already acknowledged) are skipped by the alarm service.
 * Returns the number of acknowledged alarms. Throws if the service cannot be reached or answers with an error.
 */
export async function acknowledgeAlarms(alarmIds: number[]): Promise<number> {
    let response: Response;

    try {
        response = await fetch(`${config().apiBaseUrl}/v1/alarm/acknowledgealarms`, {
            method: 'POST',
            headers: { Accept: 'application/json', 'Content-Type': 'application/json' },
            body: JSON.stringify({ alarmIds })
        });
    } catch (error) {
        const reason = error instanceof Error ? error.message : String(error);
        throw new Error(`The Luna service could not be reached: ${reason}`);
    }

    if (response.ok === false) {
        throw new Error(await readProblemDetail(response) ?? `The alarms could not be acknowledged (HTTP ${response.status}).`);
    }

    const result = (await response.json()) as { acknowledgedCount: number };

    return result.acknowledgedCount;
}

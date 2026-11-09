import { absoluteUrl, config } from './config';

/** Absolute URL of the alarm hub (resolves a relative path against the current origin). */
export function alarmHubUrl(): string {
    return absoluteUrl(config().alarmHubUrl);
}

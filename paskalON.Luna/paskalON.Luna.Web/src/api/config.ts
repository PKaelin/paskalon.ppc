/**
 * Runtime configuration.
 *
 * The settings are read from /config.json AFTER the bundle was built. That makes it
 * possible to point the same container image at a different web service without
 * rebuilding - the Docker entrypoint rewrites config.json from environment variables
 * (see docker/40-app-config.sh).
 */
export interface AppConfig {
    /** Base address of the Luna web service. Default: '/api' (reverse proxied). */
    apiBaseUrl: string;
    /** Absolute or relative URL of the SignalR system hub. Default: '/hubs/system' (reverse proxied). */
    systemHubUrl: string;
    /** Absolute or relative URL of the SignalR device hub. Default: '/hubs/device' (reverse proxied). */
    deviceHubUrl: string;
}

const defaultConfig: AppConfig = {
    apiBaseUrl: '/api',
    systemHubUrl: '/hubs/system',
    deviceHubUrl: '/hubs/device'
};

let current: AppConfig = defaultConfig;

export async function loadConfig(): Promise<AppConfig> {
    try {
        const response = await fetch(`${import.meta.env.BASE_URL}config.json`, {
            cache: 'no-store'
        });

        if (response.ok) {
            const fromFile = (await response.json()) as Partial<AppConfig>;
            current = { ...defaultConfig, ...fromFile };
        }
    } catch {
        // config.json is optional - fall back to the defaults.
        current = defaultConfig;
    }

    return current;
}

export function config(): AppConfig {
    return current;
}

/** Absolute URL of a configured URL (resolves a relative path against the current origin). */
export function absoluteUrl(configured: string): string {
    if (/^https?:\/\//i.test(configured)) {
        return configured;
    }

    return new URL(configured, window.location.origin).toString();
}

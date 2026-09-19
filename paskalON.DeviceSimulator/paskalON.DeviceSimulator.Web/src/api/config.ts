/**
 * Runtime configuration.
 *
 * The settings are read from /config.json AFTER the bundle was built. That makes it
 * possible to point the same container image at a different web service without
 * rebuilding - the Docker entrypoint rewrites config.json from environment variables
 * (see docker/docker-entrypoint.sh).
 */
export interface AppConfig {
    /** Base address of the DeviceSimulator web service. Default: '/api' (reverse proxied). */
    apiBaseUrl: string;
    /** Relative path of the GET endpoint that returns the DerDto. */
    derEndpoint: string;
    /** When true, changed endpoint values are POSTed back to the web service. */
    enableWriteBack: boolean;
    /** Relative path of the write-back endpoint. */
    writeBackEndpoint: string;
    /** Automatic reload interval in seconds. 0 disables auto refresh. */
    autoRefreshSeconds: number;
    /** Relative path of the endpoint that receives the currently expanded units/devices. */
    setExpandedEndpoint: string;
    /** When true, expanding/collapsing a unit or device is reported to the web service. */
    enableExpansionSync: boolean;
    /** Delay in milliseconds before the expansion state is posted (coalesces fast clicking). */
    expansionSyncDebounceMs: number;
    /** When true, the SPA subscribes to the SignalR hub for live DTO updates. */
    enableSignalR: boolean;
    /** Absolute or relative URL of the SignalR hub. */
    signalRHubUrl: string;
    /** Set to true if the web service serializes PascalCase instead of camelCase. */
    pascalCaseApi?: boolean;
}

const defaultConfig: AppConfig = {
    apiBaseUrl: '/api',
    derEndpoint: '/v1/der/getder',
    enableWriteBack: false,
    writeBackEndpoint: '/v1/der/setendpointvalue',
    autoRefreshSeconds: 0,
    setExpandedEndpoint: '/v1/der/setexpanded',
    enableExpansionSync: true,
    expansionSyncDebounceMs: 250,
    enableSignalR: true,
    signalRHubUrl: '/hubs/der',
    pascalCaseApi: false
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

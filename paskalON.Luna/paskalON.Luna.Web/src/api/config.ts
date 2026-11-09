/**
 * Runtime configuration.
 *
 * The settings are read from /config.json AFTER the bundle was built. That makes it
 * possible to point the same container image at a different web service without
 * rebuilding - the Docker entrypoint rewrites config.json from environment variables
 * (see docker/40-app-config.sh).
 */
import type { GrafanaConfig } from '../types/grafana';

/** The Grafana dashboards of the Dashboard section. */
export interface DashboardSectionConfig {
    /** Names of the dashboards in the selection, each must be configured in grafana.dashboards. */
    dashboards: string[];
    /** Name of the dashboard that is selected at start. */
    defaultDashboard: string;
}

export interface AppConfig {
    /** Base address of the Luna web service. Default: '/api' (reverse proxied). */
    apiBaseUrl: string;
    /** Absolute or relative URL of the SignalR system hub. Default: '/hubs/system' (reverse proxied). */
    systemHubUrl: string;
    /** Absolute or relative URL of the SignalR device hub. Default: '/hubs/device' (reverse proxied). */
    deviceHubUrl: string;
    /** Absolute or relative URL of the SignalR dependency hub. Default: '/hubs/dependency' (reverse proxied). */
    dependencyHubUrl: string;
    /** Absolute or relative URL of the SignalR alarm hub. Default: '/hubs/alarm' (reverse proxied). */
    alarmHubUrl: string;
    /** Connection to Grafana and the dashboards that can be embedded. */
    grafana: GrafanaConfig;
    /** The Grafana dashboards of the Dashboard section. */
    dashboardSection: DashboardSectionConfig;
}

const systemMeterOverview = 'System Meter Overview';
const systemOperatingModeControl = 'System Operating Mode Control';

const defaultConfig: AppConfig = {
    apiBaseUrl: '/api',
    systemHubUrl: '/hubs/system',
    deviceHubUrl: '/hubs/device',
    dependencyHubUrl: '/hubs/dependency',
    alarmHubUrl: '/hubs/alarm',
    grafana: {
        url: 'http://localhost:41006',
        authMode: 'anonymous',
        orgId: 1,
        theme: 'dark',
        kiosk: true,
        hideLogo: true,
        autoFitPanels: true,
        dashboards: [
            { name: systemMeterOverview, uid: 'ppc-system-meter-overview' },
            { name: systemOperatingModeControl, uid: 'ppc-system-operating-mode-control' }
        ]
    },
    dashboardSection: {
        dashboards: [systemMeterOverview, systemOperatingModeControl],
        defaultDashboard: systemMeterOverview
    }
};

let current: AppConfig = defaultConfig;

export async function loadConfig(): Promise<AppConfig> {
    try {
        const response = await fetch(`${import.meta.env.BASE_URL}config.json`, {
            cache: 'no-store'
        });

        if (response.ok) {
            const fromFile = (await response.json()) as Partial<AppConfig>;
            current = {
                ...defaultConfig,
                ...fromFile,
                grafana: { ...defaultConfig.grafana, ...fromFile.grafana },
                dashboardSection: { ...defaultConfig.dashboardSection, ...fromFile.dashboardSection }
            };
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

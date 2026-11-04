/**
 * Grafana integration.
 *
 * The only place that knows how Grafana is reached: the address, the authentication, how a
 * dashboard name maps to its UID and the URL parameters of an embedded dashboard. Components
 * reference dashboards by name only.
 */
import { absoluteUrl, config } from './config';
import type { GrafanaDashboardParameters, GrafanaDashboardReference } from '../types/grafana';

/** A dashboard cannot be shown, the message is meant for the user. */
export class GrafanaError extends Error {
    constructor(message: string) {
        super(message);
        this.name = 'GrafanaError';
    }
}

/** Base address of Grafana without a trailing slash. */
function baseUrl(): string {
    const configured = config().grafana.url.trim();

    if (configured === '') {
        throw new GrafanaError('The Grafana address is not configured.');
    }

    return absoluteUrl(configured).replace(/\/+$/, '');
}

/** The configured dashboard with the name (case insensitive), undefined if there is none. */
export function findDashboard(name: string): GrafanaDashboardReference | undefined {
    const wanted = name.trim().toLowerCase();

    return config().grafana.dashboards.find((dashboard) => dashboard.name.trim().toLowerCase() === wanted);
}

/** URL of the embedded dashboard with the name, throws a GrafanaError if it is not configured. */
export function dashboardUrl(name: string, parameters: GrafanaDashboardParameters = {}): string {
    const dashboard = findDashboard(name);

    if (dashboard === undefined) {
        throw new GrafanaError(`The Grafana dashboard '${name}' is not configured.`);
    }

    const grafana = config().grafana;
    const query = new URLSearchParams();

    query.set('orgId', String(grafana.orgId));
    query.set('theme', grafana.theme);

    if (grafana.hideLogo) {
        query.set('hideLogo', 'true');
    }

    if (grafana.autoFitPanels) {
        query.set('autofitpanels', 'true');
    }

    if (parameters.from !== undefined) {
        query.set('from', parameters.from);
    }

    if (parameters.to !== undefined) {
        query.set('to', parameters.to);
    }

    if (parameters.refresh !== undefined) {
        query.set('refresh', parameters.refresh);
    }

    for (const [variable, value] of Object.entries(parameters.variables ?? {})) {
        query.set(`var-${variable}`, value);
    }

    // The anonymous mode adds no credentials: Grafana allows anonymous access or the browser
    // sends its own Grafana session cookie. Further modes add their credentials here.

    // Grafana only treats a bare 'kiosk' (no value) as full kiosk mode.
    const kiosk = grafana.kiosk ? '&kiosk' : '';

    return `${baseUrl()}/d/${encodeURIComponent(dashboard.uid)}?${query.toString()}${kiosk}`;
}

/** Throws a GrafanaError if Grafana cannot be reached from the browser. */
export async function ensureReachable(signal: AbortSignal): Promise<void> {
    try {
        // Grafana sends no CORS headers: the opaque response only tells that it answered,
        // a network error that it cannot be reached.
        await fetch(`${baseUrl()}/api/health`, { mode: 'no-cors', cache: 'no-store', signal });
    } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
            throw error;
        }

        if (error instanceof GrafanaError) {
            throw error;
        }

        throw new GrafanaError(`Grafana cannot be reached at ${config().grafana.url}.`);
    }
}

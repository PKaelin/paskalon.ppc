/**
 * How Luna authenticates against Grafana.
 *
 * 'anonymous' - Luna sends no credentials. Grafana either allows anonymous (Viewer) access or the
 *               browser already holds a Grafana session cookie (e.g. single sign-on).
 */
export type GrafanaAuthMode = 'anonymous';

/** Maps the name a content section uses to the dashboard UID in Grafana. */
export interface GrafanaDashboardReference {
    /** Name of the dashboard, the title in Grafana. */
    name: string;
    /** UID of the dashboard in Grafana (the "uid" of the dashboard JSON). */
    uid: string;
}

/** Connection to Grafana and the dashboards that can be embedded. */
export interface GrafanaConfig {
    /** Address of Grafana as seen FROM THE BROWSER, e.g. 'http://localhost:41006'. */
    url: string;
    /** How Luna authenticates against Grafana. */
    authMode: GrafanaAuthMode;
    /** Grafana organisation of the dashboards. */
    orgId: number;
    /** Theme of the embedded dashboards. */
    theme: 'dark' | 'light';
    /** True hides the Grafana navigation, so only the panels are shown. */
    kiosk: boolean;
    /** True hides the "Powered by Grafana" branding footer. */
    hideLogo: boolean;
    /** True stretches the panels to fill the height of the frame. */
    autoFitPanels: boolean;
    /** The dashboards that can be embedded, referenced by name. */
    dashboards: GrafanaDashboardReference[];
}

/** Optional parameters of an embedded dashboard, unset ones use the dashboard defaults. */
export interface GrafanaDashboardParameters {
    /** Start of the time range, e.g. 'now-15m'. */
    from?: string;
    /** End of the time range, e.g. 'now'. */
    to?: string;
    /** Refresh interval, e.g. '5s'. */
    refresh?: string;
    /** Values of dashboard variables by variable name (without the 'var-' prefix). */
    variables?: Record<string, string>;
}

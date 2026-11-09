#!/bin/sh
# Writes the runtime configuration of the SPA from environment variables.
# Runs automatically at container start (nginx image entrypoint hook), which means
# the same image can be pointed at a different web service without a rebuild.
# The target file can be passed as first argument (used by docker/dev-entrypoint.sh).
set -e

CONFIG_FILE=${1:-/usr/share/nginx/html/config.json}

: "${APP_API_BASE_URL:=/api}"
: "${APP_SYSTEM_HUB_URL:=/hubs/system}"
: "${APP_DEVICE_HUB_URL:=/hubs/device}"
: "${APP_DEPENDENCY_HUB_URL:=/hubs/dependency}"
: "${APP_ALARM_HUB_URL:=/hubs/alarm}"

# Grafana as seen FROM THE BROWSER (not from the container).
# TODO: Add security to this functionality!
: "${APP_GRAFANA_URL:=http://localhost:41006}"
: "${APP_GRAFANA_AUTH_MODE:=anonymous}"
: "${APP_GRAFANA_ORG_ID:=1}"
: "${APP_GRAFANA_THEME:=dark}"
: "${APP_GRAFANA_KIOSK:=true}"
: "${APP_GRAFANA_HIDE_LOGO:=true}"
: "${APP_GRAFANA_AUTO_FIT_PANELS:=true}"
# JSON array of the embeddable dashboards: [{ "name": "<dashboard title>", "uid": "<dashboard uid>" }, ...]
[ -n "${APP_GRAFANA_DASHBOARDS}" ] || APP_GRAFANA_DASHBOARDS='[{ "name": "System Meter Overview", "uid": "ppc-system-meter-overview" }, { "name": "System Operating Mode Control", "uid": "ppc-system-operating-mode-control" }]'
# JSON array of the dashboard names in the Dashboard section and the one selected at start.
[ -n "${APP_DASHBOARD_SECTION_DASHBOARDS}" ] || APP_DASHBOARD_SECTION_DASHBOARDS='[ "System Meter Overview", "System Operating Mode Control" ]'
: "${APP_DASHBOARD_SECTION_DEFAULT:=System Meter Overview}"

cat > "$CONFIG_FILE" <<JSON
{
  "apiBaseUrl": "${APP_API_BASE_URL}",
  "systemHubUrl": "${APP_SYSTEM_HUB_URL}",
  "deviceHubUrl": "${APP_DEVICE_HUB_URL}",
  "dependencyHubUrl": "${APP_DEPENDENCY_HUB_URL}",
  "alarmHubUrl": "${APP_ALARM_HUB_URL}",
  "grafana": {
    "url": "${APP_GRAFANA_URL}",
    "authMode": "${APP_GRAFANA_AUTH_MODE}",
    "orgId": ${APP_GRAFANA_ORG_ID},
    "theme": "${APP_GRAFANA_THEME}",
    "kiosk": ${APP_GRAFANA_KIOSK},
    "hideLogo": ${APP_GRAFANA_HIDE_LOGO},
    "autoFitPanels": ${APP_GRAFANA_AUTO_FIT_PANELS},
    "dashboards": ${APP_GRAFANA_DASHBOARDS}
  },
  "dashboardSection": {
    "dashboards": ${APP_DASHBOARD_SECTION_DASHBOARDS},
    "defaultDashboard": "${APP_DASHBOARD_SECTION_DEFAULT}"
  }
}
JSON

echo "40-app-config.sh: wrote $CONFIG_FILE (api=${APP_API_BASE_URL}, hub=${APP_SYSTEM_HUB_URL}, device hub=${APP_DEVICE_HUB_URL}, dependency hub=${APP_DEPENDENCY_HUB_URL}, alarm hub=${APP_ALARM_HUB_URL}, grafana=${APP_GRAFANA_URL}, upstream=${LUNA_API_URL:-unset})"

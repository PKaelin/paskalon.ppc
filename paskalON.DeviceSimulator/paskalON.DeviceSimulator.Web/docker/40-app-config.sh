#!/bin/sh
# Writes the runtime configuration of the SPA from environment variables.
# Runs automatically at container start (nginx image entrypoint hook), which means
# the same image can be pointed at a different web service without a rebuild.
set -e

CONFIG_FILE=/usr/share/nginx/html/config.json

: "${APP_API_BASE_URL:=/api}"
: "${APP_DER_ENDPOINT:=/v1/der/getder}"
: "${APP_ENABLE_WRITE_BACK:=false}"
: "${APP_WRITE_BACK_ENDPOINT:=/v1/der/setendpointvalue}"
: "${APP_AUTO_REFRESH_SECONDS:=0}"
: "${APP_PASCAL_CASE_API:=false}"
: "${APP_SET_EXPANDED_ENDPOINT:=/v1/der/setexpanded}"
: "${APP_ENABLE_EXPANSION_SYNC:=true}"
: "${APP_EXPANSION_SYNC_DEBOUNCE_MS:=250}"
: "${APP_ENABLE_SIGNALR:=true}"
: "${APP_SIGNALR_HUB_URL:=/hubs/der}"

cat > "$CONFIG_FILE" <<JSON
{
  "apiBaseUrl": "${APP_API_BASE_URL}",
  "derEndpoint": "${APP_DER_ENDPOINT}",
  "enableWriteBack": ${APP_ENABLE_WRITE_BACK},
  "writeBackEndpoint": "${APP_WRITE_BACK_ENDPOINT}",
  "autoRefreshSeconds": ${APP_AUTO_REFRESH_SECONDS},
  "setExpandedEndpoint": "${APP_SET_EXPANDED_ENDPOINT}",
  "enableExpansionSync": ${APP_ENABLE_EXPANSION_SYNC},
  "expansionSyncDebounceMs": ${APP_EXPANSION_SYNC_DEBOUNCE_MS},
  "enableSignalR": ${APP_ENABLE_SIGNALR},
  "signalRHubUrl": "${APP_SIGNALR_HUB_URL}",
  "pascalCaseApi": ${APP_PASCAL_CASE_API}
}
JSON

echo "40-app-config.sh: wrote $CONFIG_FILE (api=${APP_API_BASE_URL}, upstream=${DER_API_URL:-unset})"

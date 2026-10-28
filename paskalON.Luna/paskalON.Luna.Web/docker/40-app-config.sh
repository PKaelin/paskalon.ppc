#!/bin/sh
# Writes the runtime configuration of the SPA from environment variables.
# Runs automatically at container start (nginx image entrypoint hook), which means
# the same image can be pointed at a different web service without a rebuild.
# The target file can be passed as first argument (used by docker/dev-entrypoint.sh).
set -e

CONFIG_FILE=${1:-/usr/share/nginx/html/config.json}

: "${APP_API_BASE_URL:=/api}"
: "${APP_SYSTEM_HUB_URL:=/hubs/system}"

cat > "$CONFIG_FILE" <<JSON
{
  "apiBaseUrl": "${APP_API_BASE_URL}",
  "systemHubUrl": "${APP_SYSTEM_HUB_URL}"
}
JSON

echo "40-app-config.sh: wrote $CONFIG_FILE (api=${APP_API_BASE_URL}, hub=${APP_SYSTEM_HUB_URL}, upstream=${LUNA_API_URL:-unset})"

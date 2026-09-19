#!/bin/sh
# Runs inside the dev container. node_modules lives in its own named volume (see
# docker-compose.dev.yml) so it survives "docker compose down" and is only (re)installed
# when package.json/package-lock.json actually changed - not on every container start.
set -e

MARKER=node_modules/.install-stamp

if [ ! -f "$MARKER" ] || [ package.json -nt "$MARKER" ] || [ package-lock.json -nt "$MARKER" ]; then
    echo "dev-entrypoint: installing npm packages..."
    npm ci
    touch "$MARKER"
fi

# Optional: regenerate public/config.json from the same APP_* environment variables the
# production image reads (docker/40-app-config.sh) - so a compose override for this dev
# target can be configured with the exact same variables as your production service.
#
# OFF by default. public/config.json is bind-mounted from your host, so turning this on
# overwrites that file ON DISK too - fine if you only ever run this in the container,
# mildly surprising if you also run "npm run dev" directly on the host sometimes, since
# whichever mode ran last "wins" the file. If unsure, leave this off and edit
# public/config.json by hand for local values instead.
if [ "${GENERATE_CONFIG_FROM_ENV:-false}" = "true" ]; then
    echo "dev-entrypoint: writing public/config.json from environment variables"
    sh docker/40-app-config.sh public/config.json
fi

echo "dev-entrypoint: starting Vite dev server (http://localhost:5173, HMR enabled)"
exec npm run dev -- --host 0.0.0.0

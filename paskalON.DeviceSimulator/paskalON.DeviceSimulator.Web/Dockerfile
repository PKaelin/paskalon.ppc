# =============================================================================
# paskalON.DeviceSimulator.Web
#
# Multi-stage build with three named targets - which one gets built is chosen by
# `target:` in whichever compose file builds this image, NOT by editing this file:
#
#   dev    - Vite dev server with Hot Module Replacement. No COPY step: the source is
#            bind-mounted at runtime by the compose file that selects this target, so
#            editing a .vue/.ts/.css file needs no rebuild and no recreated image.
#   build  - intermediate stage that produces the optimized static bundle. Never
#            selected directly - only used as a source for the "final" stage below.
#   final  - nginx serving the built bundle. This is the LAST stage in the file, so a
#            plain `docker build` (or a compose service with no `target:` at all, like
#            your existing production service) builds this one - nothing changes there.
#
# nginx (in "final") reverse proxies /api and /hubs to the DeviceSimulator web service,
# so the browser only ever talks to one origin and no CORS setup is required.
# =============================================================================

FROM node:24-alpine AS base
WORKDIR /app

# ---------- dev ----------
FROM base AS dev
EXPOSE 5173
ENTRYPOINT ["sh", "docker/dev-entrypoint.sh"]

# ---------- build ----------
FROM base AS build

# Copy the manifests first so the npm layer is cached between builds.
COPY package.json package-lock.json* ./
RUN npm ci

COPY . .
RUN npm run build

# ---------- final (production - this is what you ship) ----------
FROM nginx:1.27-alpine AS final

# Address of the DeviceSimulator web service as seen FROM THE CONTAINER.
# host.docker.internal points at the Docker host (Windows/macOS, and Linux when
# extra_hosts is set - see docker-compose.yml).
ENV DER_API_URL=http://host.docker.internal:45500
ENV APP_ENABLE_WRITE_BACK=false
ENV APP_AUTO_REFRESH_SECONDS=0
ENV APP_ENABLE_EXPANSION_SYNC=true
ENV APP_ENABLE_SIGNALR=true

COPY docker/00-websocket.conf.template /etc/nginx/templates/00-websocket.conf.template
COPY docker/nginx.conf.template /etc/nginx/templates/default.conf.template
COPY docker/40-app-config.sh /docker-entrypoint.d/40-app-config.sh
RUN chmod +x /docker-entrypoint.d/40-app-config.sh

COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s \
    CMD wget -q -O /dev/null http://localhost:8080/healthz || exit 1

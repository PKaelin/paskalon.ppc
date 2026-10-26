import { fileURLToPath, URL } from 'node:url';
import { readFileSync } from 'node:fs';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

const pkg = JSON.parse(
    readFileSync(new URL('./package.json', import.meta.url), 'utf-8')
) as { version: string; displayName?: string; name: string };

/**
 * The address of the Luna web service.
 * Overridable with the LUNA_API_URL environment variable (used by the dev proxy only,
 * the container uses the nginx reverse proxy - see docker/nginx.conf.template).
 */
const lunaApiUrl = process.env.LUNA_API_URL ?? 'http://localhost:45550';

export default defineConfig({
    plugins: [vue()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    define: {
        __APP_VERSION__: JSON.stringify(pkg.version),
        __APP_DISPLAY_NAME__: JSON.stringify(pkg.displayName ?? pkg.name),
        __BUILD_DATE__: JSON.stringify(new Date().toISOString())
    },
    server: {
        port: 5173,
        strictPort: false,
        open: false,
        // Listen on all interfaces, not just localhost - required so the dev server is
        // reachable from outside the container when it runs there (docker-compose.override.yml).
        // Harmless when run directly on the host (F5 in Visual Studio).
        host: true,
        // Bind mounts on Windows/macOS don't always deliver filesystem change events to
        // Linux containers, so polling is used as a fallback. Only enabled inside Docker
        // (see docker-compose.override.yml).
        watch: {
            usePolling: process.env.VITE_USE_POLLING === 'true'
        },
        // Everything under /api is forwarded to the Luna web service, so the browser
        // always talks to a single origin and no CORS configuration is needed.
        proxy: {
            '/api': {
                target: lunaApiUrl,
                changeOrigin: true,
                secure: false
            }
        }
    },
    preview: {
        port: 5173,
        proxy: {
            '/api': { target: lunaApiUrl, changeOrigin: true, secure: false }
        }
    },
    build: {
        outDir: 'dist',
        emptyOutDir: true,
        sourcemap: true
    }
});

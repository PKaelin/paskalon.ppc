# paskalON.DeviceSimulator.Web

Single page application (Vue 3 + TypeScript + Bootstrap 5, built with Vite) that renders the
device tree of the paskalON DeviceSimulator and lets you edit the writeable endpoint values.

* Loads `GET http://localhost:45500/api/v1/der/getder` at startup.
* Shows the `DerDto.Name` as the application title (header + browser tab).
* Nested accordions: **Group → Circuit → Unit (battery/solar) → PCS / Battery bank / Solar panel**,
  plus the system, auxiliary, external and circuit power meters.
* Modbus grids (Address / Name / Data type / Value) and C37 grids (Name / Signal type / Value).
* Search-as-you-type box under the header that filters the whole section tree by name.

---

## 1. Prerequisites

| Tool | Version used | Notes |
|------|--------------|-------|
| Visual Studio | 2026 | Workload **Node.js development** (gives you the JavaScript/TypeScript project system for `.esproj`) |
| Node.js | v24.21.0 | The project requires >= 20.19 |
| npm | 12.0.2 | The project requires >= 10 |
| Docker Desktop | current | only needed for the container run |


## 2. Open in Visual Studio

1. Open `paskalON.DeviceSimulator.Web.sln`.
2. Visual Studio restores the npm packages automatically (or right-click the project → *npm* → *Install missing npm packages*; on the command line: `npm install` inside `src\paskalON.DeviceSimulator.Web`).
3. Press **F5** / **Ctrl+F5**. That runs `npm run dev` (Vite dev server) on <http://localhost:5173>.

The dev server proxies everything under `/api` to `http://localhost:45500`, so the browser only
ever talks to one origin — **no CORS configuration is needed on the web service**. Override the
target with the `DER_API_URL` environment variable if your service listens somewhere else.

### No web service at hand?

A mock of the web service is included:

```
node mock-api\server.mjs          (or: npm run mock-api, or: start-dev.cmd)
```

It serves realistic `DerDto` data (2 groups, battery + solar units, banks, panels, meters,
read-only and writeable endpoints) on `http://localhost:45500/api/v1/der/getder`.

## 3. Run in Docker

```powershell
docker compose up --build            # or: start-docker.cmd
```

→ <http://localhost:8080>

The image is a two-stage build: Node builds the bundle, nginx serves it and reverse proxies
`/api/` to `DER_API_URL` (default `http://host.docker.internal:45500`, i.e. the service running on
your Windows host).

To run the app *and* the mock service in containers:

```powershell
docker compose -f docker-compose.yml -f docker-compose.mock.yml up --build   # or: start-docker-with-mock.cmd
```

### Container settings (environment variables)

| Variable | Default | Meaning |
|----------|---------|---------|
| `DER_API_URL` | `http://host.docker.internal:45500` | Upstream web service for the nginx proxy |
| `APP_API_BASE_URL` | `/api` | Base address the SPA calls |
| `APP_DER_ENDPOINT` | `/v1/der/getder` | Path of the GET endpoint |
| `APP_ENABLE_WRITE_BACK` | `false` | POST changed values back to the service |
| `APP_WRITE_BACK_ENDPOINT` | `/v1/der/setendpointvalue` | Path of the write endpoint |
| `APP_AUTO_REFRESH_SECONDS` | `0` | Periodic reload (0 = off) |
| `APP_PASCAL_CASE_API` | `false` | Set to `true` if the service serializes PascalCase |

These are written into `/usr/share/nginx/html/config.json` at container start, so one image can be
re-pointed at another service **without a rebuild**. Outside Docker the same file lives in
`public/config.json`.

## 4. Solution layout

```
paskalON.DeviceSimulator.Web.sln
docker-compose.yml / docker-compose.mock.yml
mock-api/server.mjs                     mock of the DeviceSimulator web service
src/paskalON.DeviceSimulator.Web/
    paskalON.DeviceSimulator.Web.esproj Visual Studio project (F5 = npm run dev)
    Dockerfile                          multi stage build -> nginx
    docker/nginx.conf.template          SPA fallback + /api reverse proxy
    docker/40-app-config.sh             writes config.json from env vars at start
    public/config.json                  runtime configuration
    public/images/paskalOn-logo.svg     placeholder logo -> replace with your own
    src/types/der.ts                    TypeScript mirror of the DTO contract
    src/types/tree.ts                   view model of one accordion section
    src/api/derApi.ts                   fetch client (+ optional write back)
    src/utils/treeBuilder.ts            DerDto  ->  accordion tree
    src/utils/treeFilter.ts             search filter + hit highlighting
    src/composables/                    expand/collapse state, search term, notifications
    src/components/                     header, footer, search box, accordions, grids
    src/styles/bootstrap.css            YOUR custom stylesheet (theme untouched)
    src/styles/site.css                 layout + accordion/grid styling in the same palette
```

### Why one recursive component

`treeBuilder.ts` converts the `DerDto` into a uniform `TreeNode` structure (groups, circuits,
units, devices and meters all look the same to the UI). `TreeSection.vue` renders a node and calls
itself for the children, so nesting depth, search filtering and expand/collapse are handled in one
place instead of in five near-identical components. The endpoint arrays are kept **by reference**,
so an edit in a grid updates the loaded model directly.

The accordions are driven by Vue (not by the Bootstrap collapse plugin) — that is what allows the
search to force the matching sections open without fighting the plugin over the DOM. Bootstrap's
JavaScript is still used for the navbar toggler and the Admin dropdown.

## 5. Notes and small things you may want to change

* **Logo**: `public/images/paskalOn-logo.svg` is a placeholder. Drop in `paskalOn-logo.png` and
  change the `<img src>` in `src/components/AppHeader.vue`.
* **Version in the header/footer**: taken from `version` in `package.json` at build time
  (`__APP_VERSION__`), the equivalent of `Assembly.GetName().Version` in the Razor sample.
* **Writing values back**: the DTOs you supplied have no write contract, so the app updates the
  value locally and tells you it did. Set `enableWriteBack` to `true` in `config.json` (or
  `APP_ENABLE_WRITE_BACK=true`) and adapt `writeEndpointValue()` in `src/api/derApi.ts` to your
  real endpoint once the service exposes one.
* **Enum serialization**: `ModbusDataType` / `C37SignalType` are typed as `string | number`, so the
  grids work whether or not the service uses `JsonStringEnumConverter`.
* **Casing**: ASP.NET Core serializes camelCase by default, which is what the app expects. If yours
  keeps PascalCase, set `"pascalCaseApi": true` in `config.json`.
* **Keyboard**: `Esc` clears the search box; `Enter` commits a value field.

## 6. Scripts

| Command | Purpose |
|---------|---------|
| `npm run dev` | Vite dev server with hot reload (port 5173) |
| `npm run build` | type check (`vue-tsc`) + production build into `dist/` |
| `npm run preview` | serve the production build locally |
| `npm run type-check` | type check only |
| `npm run mock-api` | start the mock web service on port 45500 |

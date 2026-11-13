# User Interface Instructions


## General Guidelines
- **Consistency:** Reuse existing components, styles and wording before creating new ones. A new UI element must look and behave like comparable existing elements (spacing, colors, icons, labels, etc.).
- **Modularity:** Ensure that a modular and reusable design for UI components is followed to facilitate maintainability and scalability.
- **Loading states:** Every component that shows remote data must handle all four states explicitly: loading, no data, error/disconnected, and data.
- **Text:** Use short, operator-oriented wording in English. Error messages state what failed and, if possible, what to do next; never show raw exceptions or stack traces.


## Luna Web

### Architecture & Data Access
- All requests from Luna Web go through the Luna Web API (`paskalON.Luna.Service`) via REST or SignalR, never directly to other services or databases. Grafana embedding is the only exception.
- Base URLs and hub URLs come from the runtime configuration (`config()` in `src/api/config.ts`). Never hard-code hosts, ports or paths. New settings are added to `AppConfig`, `defaultConfig` and the Docker config script.
- TypeScript types in `src/types/` mirror the DTOs in `paskalON.Luna.Dto`. When a DTO changes, update the matching type in the same change.

### Real-Time Data (SignalR)
- **Publish on demand:** Data is published to Luna Web only while the page or component that needs it is shown. E.g. on the "Dashboard" page only dashboard data is published, not the setpoints of the "Operating Modes" page.
    - Exception: data shown in the always-visible app header (alarm summary, dependency status) may be published to all connected clients.
- **Server subscriptions:** The server only publishes to connections that want the data:
    - A hub carries one topic (e.g. system meter summary or setpoints), or tracks per connection which topic/device is subscribed (pattern: `DeviceSubscriptions`).
    - Never send a topic with `Clients.All` on a hub that also serves other topics.
    - Publishers skip the work (reading and serializing the data) when no connection is subscribed.
    - Subscriptions are removed in once the component is unmounted.
- **Initial value:** On connect or subscribe, the hub sends the current value to the caller immediately, so the client does not wait for the next publishing interval.
- **Connection status & stale values:** Show the hub connection status (Connecting…, Live, Reconnecting…, Disconnected) next to the data (pattern: `DeviceSummary`). The composable exposes the status and a `received` flag (false until the first value arrived).
    - Not yet received: show a loading indicator, not empty or zero values.
    - Connection lost (reconnecting/disconnected): keep the last values visible but show the warning hint "The Luna service cannot be reached, the values are not updated." Never show the last values without this hint.

### Values & Formatting
- Always display the unit with a value (kW, kVAr, %, V, Hz ...) and use a consistent number of decimals per quantity.
- Display missing values as `–` rather than `0`.
  
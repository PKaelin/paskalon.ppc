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
- Data is published to Luna Web only while the page or component that needs it is shown. E.g. on the "Dashboard" page only dashboard data is published.
- A component subscribes on mount and unsubscribes on unmount via a `use...` composable (pattern: `useDeviceSubscription`). Subscriptions must be re-established after a reconnect.
- The server tracks subscriptions per connection and publishes only to subscribed connections. Unsubscribed or disconnected clients cause no publishing work.
- Show the hub connection status. When the connection is lost or values are not yet received, mark the values as stale/unavailable instead of showing the last values as if they were current

### Values & Formatting
- Always display the unit with a value (kW, kVAr, %, V, Hz ...) and use a consistent number of decimals per quantity.
- Display missing values as `–` rather than `0`.
  
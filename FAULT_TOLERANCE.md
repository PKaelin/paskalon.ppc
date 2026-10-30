# Fault Tolerance & Resilience


## Dependency health design


### Who talks to whom
Operating Modes DependencyHealthMonitor tracks three dependencies and each one shows it is alive in its own way:
- Power Control: each new heartbeat counts as seen. Outdated or duplicate heartbeats are ignored.
- Device Service: each received core device data message counts as seen.
- Redis: the monitor checks the connection itself and an open connection counts as seen.
A dependency is Lost from startup until it is first seen and again whenever it has not been seen for its loss timeout.

![Who talks to whom](./Docs/Dependency%20Health%20Who%20Talks%20To%20Whom.drawio.svg)


### Dependency health states
- **Dependency state:** shows how a service decides whether a dependency is alive or not.
- **Operating Mode control:** shows how an outage is kept from causing wrong setpoints or a sudden jump when the dependency returns.
- **Heartbeat publishing:** shows how a service keeps signaling that it is alive during a message broker outage.

![Dependency health states](./Docs/Dependency%20Health%20States.drawio.svg)


# Power Controls Client
The power controls client is used as a subscriber for all the microservices that need to receive the power targets of the power controls service.

Each consuming service implements `IPowerTargetHandler` to decide what to do with a received power target, e.g. the device service applies it to the power conversion system (PCS).

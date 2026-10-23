# Operating Modes Client
The operating modes client is used as a subscriber for all the microservices that need to receive the power setpoints of the operating modes service.

Each consuming service implements `IPowerSetpointHandler` to decide what to do with received system and DER unit power setpoints.
E.g. the power controls service applies them to its power control manager.

# Operating Modes Domain Configs
This project contains all persistent configuration related code.

## General concept overview
- Every operating mode shall have at least one type assigned but can be assigned multiple types.
- Every operating mode has configuration data. 
- Each operating mode must include a ramp configuration.
- Each operating mode may include a curve configuration. 
- Each operating mode may include a custom configuration for future or unplanned configuration types, allowing the database schema to remain unchanged as project specific configurations are introduced.

![Operating Modes Domain Overview](./Docs/Operating%20Mode%20Config%20Overivew.drawio.svg)


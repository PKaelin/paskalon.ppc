# Equipments
How specific equipment and manufacturer implementations are organized.

Back to the [Design Overview](./Overview.md).


## Folder Structure
Equipment types: PowerConversionSystems, BatteryBanks, etc.
Manufacturers within equipment types: PowerElectronics, SMA, etc.

The Modbus polling engine and the C37 transmission engine read the data for the equipments, see [Dataface](./Dataface.md).


## Special Manufacturer Simple
Simple which is just a class that inherits from base for tests, simulations and analysis.


## File Naming
- [DeviceShortform][Series/Type][Version]Description: For device specific information like Codes, Warnings, Errors, States, Registers, etc.
- [DeviceShortform][Series/Type][Version]Proxy: For additional properties to base implementation and for communication to the devices and data endpoint updater to the domain instances.


## Conventions
Do not use base classes for a equipment that share a common type and or manufacturer.
Duplicated code in this project is not just ok it is encouraged to keep equipment and versions completely independent.
E.g. PcsHemkV3Description and PcsHemkV4Description might have almost the same registers but at the end you want a clear separation
of those two so that the V3 always works and wont break once somebody changes something in the common description class.
This might lead to duplicated code but I strongly believe that this is the best and cleanest approach as I have witness the mess a base class can cause.


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)

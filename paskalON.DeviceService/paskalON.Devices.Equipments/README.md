# Devices Equipments
This project contains all code required for specific equipment and manufacturer implementation.   


## Folder structure
Equipment types: PowerConversionSystems, BatteryBanks, etc.
Manufacturers within equipment types: PowerElectronics, SMA, etc.

## Special manufacturer Simple 
Simple which is just a class that inherits from base for tests, simulations and analysis.


## File naming
- [DeviceShortform][Series/Type][Version]Description: For device specific information like Codes, Warnings, Errors, States, Registers, etc.
- [DeviceShortform][Series/Type][Version]Proxy: For additional properties to base implementation and for communication to the devices and data endpoint updater to the domain instances.


## Conventions
Do not use base classes for a equipment that share a common type and or manufacturer.
E.g. PcsHemkV3Description and PcsHemkV4Description might have almost the same registers but at the end you want a clear separation
of those two so that the V3 always works and wont break once somebody changes something in the common description class.
This might lead to duplicated code but I strongly believe that this is the best and cleanest approach as I have witness the mess a base class can cause.

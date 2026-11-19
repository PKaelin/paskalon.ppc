# Dataface
How the device domains are decoupled from the actual communications.

Back to the [Design Overview](./Overview.md).


## Concept
The dataface registration pattern is used for loose coupling the domains from the actual communications.
At this point only Modbus and C37 communications are supported but the dataface should make it easy to add 
additional communications in the future.


## Dataface Registration Design Overview
![Dataface Registration Design Overview](../Assets/Diagrams/Dataface%20Registration%20Design%20Overview.drawio.svg)


## Dataface Data Design Overview
The getting of the data is outsourced to an engine. Both the equipment and the engine have a IClient and an IDataface injected to achieve loose coupling.
Below is an abstract design of the idea. 
The ModbusEquipment could be a battery bank, power conversion system, etc. 
The C37Equipment could be a system power meter, circuit power meter, etc.

![Dataface Data Design Overview](../Assets/Diagrams/Dataface%20Data%20Design%20Overview.drawio.svg)


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)

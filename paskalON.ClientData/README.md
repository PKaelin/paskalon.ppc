# Client Data
Loads client data or sample data into the microservices databases.

## How it works

- All client data projects are in one solution.
    - ./paskalON.ClientData/paskalON.ClientData.slnx
    - Each project is in its own subfolder of ./paskalON.ClientData.
    - In the initial phase all required project references in the namespace paskalON are under a folder named: "InitialDevelopment" instead of nuget packages.

- The common project contains common functionalities like CSV parsing, Schema version selection, Property type conversion, etc.
    - paskalON.ClientData.Common.csproj
    - The common project is a class library that is referenced by all other Data projects.

- Each microservice type has one project that reads in a file and creates the data defined in that file.
    - paskalON.Devices.Data.csproj
    - paskalON.DeviceSimulator.Data.csproj
    - paskalON.PowerControls.Data.csproj
    - paskalON.OperatingModes.Data.csproj
    - paskalON.Alarms.Data.csproj
- Each of the Data projects is a console application.
- Each of the Data projects takes the following arguments described in section below.


### The Data projects input arguments

| Input Argument | Description |
|----------------|-------------|
| ClientProjectName | The name of the client project (E.g. DemoSuperSimpleBattery, DemoSuperSimpleSolar, ZuerichNorth, etc.) |
| ClientProjectFile | The path to the client project file (E.g. C:\Projects\DemoSuperSimpleBattery\DemoSuperSimpleBattery.csv) |
| SchemaVersion | The version of the schema to use (E.g. 1.0, 2.0, etc. or if empty it will take the latest version) |


## Client Project Data (CSV) File Location
- The client project data files shall be in a folder within the client project folder.
- The root data folder shall be named "ClientData" and the subfolders shall be named after the specific project or "Demo" if its demo/development configuration.
    - E.g. Devices: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv
    - E.g. DeviceSimulator: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv
    - E.g. PowerControls: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv
    - E.g. OperatingModes: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv
    - E.g. Alarms: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv


## Client Project Data (CSV) File Definition
- The "ClientProjectFile" is a CSV file that contains the definitions of the microservices data. 
- If the first character of any line is a '#' then it will be ignored as a comment line.

- Every line in the CSV file represents a domain entity and its properties without a header row.
- The first column shall always be Type of the domain entity. E.g. Type=ModbusConfig
- The second column shall always be Name of the domain entity. E.g. Name=ModbusConPcs1
- Duplicated entities (Names) shall result in a validation error.
- Remaining columns represent property names and values. E.g. DeviceFactorCore=1, PublisherTopicPcsCore="ppc:device:pcs:core",
- Property names shall match the actual domain property names.
- Property values shall be converted to the corresponding .NET property types (see Property conversions table below).
- Unknown properties shall result in a validation error.
- Errors should identify the file, line number, entity name and property.

- All the domains have a ChangedBy property that is automatically set to "System Init".
- All the domains have a ChangedDate property that is automatically set to DateTimeOffset.UtcNow.
- The CSV should not be allowed to override ChangedBy or ChangedDate properties.

- A reference property shall reference an existing domain entity of the expected type.
- The referenced entity is identified by its Name property.
- Reference property shall reference an entity previously defined in the CSV file. Reference entities defined later in the file are not supported and shall result in a validation error.
- If the reference value is null the reference property remains null.
- If a reference cannot be resolved loading fails with a descriptive error and how errors should be identified as described above.
- A reference to an entity of the wrong domain type shall be rejected.
- A list of references (e.g. Constraints) contains the names of the referenced entities separated by a semicolon. Each referenced entity is added to the list. Lists of values other than references are not supported.
- Each line shall: Create the domain object, Populate their scalar properties, Resolve their references, Add them to the DbContext.



## Property conversions

| .NET property type | CSV value | Result |
|--------------------|-----------|--------|
| string | "Battery01" | "Battery01" |
| int | 100 | 100 |
| double | 3.14 | 3.14 |
| bool | true | true |
| DateTimeOffset | 2026-11-01T10:00:00Z | DateTimeOffset |
| GUID | "GUID string" | GUID |
| Enum | Connected | Enum value |
| Reference | ModbusCon1 | Referenced entity |
| Nullable reference | null | null |
| List of references | Constraint1;Constraint2 | Referenced entities added to the list |


## Alarms client data
- Alarm actions (AlarmActionConfig) are shared: one action can be listed by many alarm configurations.
- Define the action and its parameters (AlarmActionParameterConfig) before the alarm configurations that list it in AlarmActions (several separated by ;).
- The parameter values can contain placeholders that are replaced with the values of the alarm when the action is sent:
  {{AlarmId}}, {{PpcAlarmType}}, {{SeverityLevel}}, {{DeviceId}} and {{PropertyName}}.
- E.g. one ShowAlarm action shared by the communication error configurations of all device types.


## Application exit codes
Application exit codes are used to indicate the result of the execution of the Data project. 
The following table describes the exit codes and their meanings:

| Exit code | Meaning |
|-----------|---------|
| 0 | Successfully imported |
| 1 | General execution error |
| 2 | Invalid arguments |
| 3 | CSV validation error |
| 4 | Database error |

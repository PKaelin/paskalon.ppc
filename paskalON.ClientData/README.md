# Client Data
Loads client data or sample data into the microservices databases.

## How it works

- For all client data projects we shall have one common solution.
    - ./paskalON.ClientData/Common/paskalON.ClientData.Common.slnx
- The common solution shall have one project that contains common functionalities like CSV parsing, Schema version selection, Property type conversion, etc.
    - Solution in "./paskalON.ClientData/Common" has a project: paskalON.ClientData.Common.csproj
    - The common project is a class library that is referenced by all other Data projects.
    - In the initial phase all required project references in the namespace paskalON are under a folder named: "InitialDevelopment" instead of nuget packages.

- Each microservice type shall have one visual studio solution.
    - ./paskalON.ClientData/Devices/paskalON.ClientData.Devices.slnx
    - ./paskalON.ClientData/DeviceSimulator/paskalON.ClientData.DeviceSimulator.slnx
    - ./paskalON.ClientData/PlantControls/paskalON.ClientData.PlantControls.slnx
    - ./paskalON.ClientData/OperatingModes/paskalON.ClientData.OperatingModes.slnx
  
- Each solution shall have one visual studio project that reads in a file and creates the data defined in that file.
    - Solution in "./paskalON.ClientData/Devices" has a project: paskalON.Devices.Data.csproj
    - Solution in "./paskalON.ClientData/DeviceSimulator" has a project: paskalON.DeviceSimulator.Data.csproj
    - Solution in "./paskalON.ClientData/PlantControls" has a project: paskalON.PlantControls.Data.csproj
    - Solution in "./paskalON.ClientData/OperatingModes" has a project: paskalON.OperatingModes.Data.csproj
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
    - E.g. PlantControls: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv
    - E.g. OperatingModes: ClientData\Demo\DemoSuperSimpleBattery.csv or ClientData\TheClient\ClientSite.csv


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

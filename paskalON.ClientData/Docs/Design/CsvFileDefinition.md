# CSV File Definition
Format of the client project data (CSV) files and the property conversions.

Back to the [Design Overview](./Overview.md).


## File Definition
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


## Property Conversions

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


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)

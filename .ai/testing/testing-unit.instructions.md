# Unit Testing Instructions
This document defines the strict constraints, patterns, and style requirements for writing unit tests in this repository.


## Technology & Conventions
- Use MSTest by Microsoft.
- Use test framework nuget package Moq without fluent-style APIs.


## Scope of Unit Tests
- **Isolation:** Ensure unit tests are entirely isolated, pure, and run completely in-memory.
- **Dependencies:** Strictly prohibit all hardware-bound or external dependencies (e.g. Web-Service, Databases, File access, etc.).
- **External Boundaries:** Use mocked interfaces or in-memory doubles to simulate external layer boundaries.
- **Test Target:** Target the smallest piece of code that can be usefully and independently tested.


## Test File Structure & Naming
- **Unit Test Project:** Always end with .UnitTest
- **Unit Test Class Names:** Shall begin with the name of the class that is tested and end with Test (e.g. `class Device` results in unit test `class DeviceTest`). 
- **Unit Test Method Names:** Shall start with the class name it tests, followed by description what it tests and end with Test (e.g. class Device method Run results in unit test Run[Description]Test).    
- **ILogger Interface:** Use `Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance` for simple ILoggers and `Microsoft.Extensions.Logging.Testing.FakeLogger<T>` when testing of logging makes sense.

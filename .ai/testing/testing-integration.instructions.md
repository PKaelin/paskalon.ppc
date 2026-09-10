# Integration Testing Instructions
This document defines the strict constraints, patterns, and style requirements for writing integration tests in this repository.


## Technology & Conventions
- Use MSTest by Microsoft.


## Scope of Integration Tests
- Write real integration tests that verify multiple components work together.
- Do not mock the system under test.
- Do not mock the database when the purpose of the test is database integration.
- Do not mock HTTP services when the purpose of the test is HTTP integration unless the external dependency is intentionally replaced by a test server/fake.
- Mock only external boundaries that cannot or should not be exercised in the integration environment.
- Reuse the application's real dependency injection configuration whenever practical.


## Test File Structure & Naming
- **Integration Test Project:** Always end with .IntegrationTest
- **Integration Test Class Names:** Shall begin with the name of the class that is tested and end with Test (e.g. `class Device` results in integration test `class DeviceTest`). 
- **Integration Test Method Names:** Shall start with the class name it tests, followed by description what it tests and end with Test (e.g. class Device method Run results in integration test Run[Description]Test).    
- **ILogger Interface:** Use `Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance` for simple ILoggers and `Microsoft.Extensions.Logging.Testing.FakeLogger<T>` when testing of logging makes sense.      

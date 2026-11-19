# Test
Testing process: standards, guidelines, unit, integration, performance and hardware-in-the-loop (HIL) testing.


## Test Levels
- **Unit tests** (`*.UnitTest` projects): isolated and in-memory, external boundaries are mocked with Moq. See [testing-unit.instructions.md](/.ai/testing/testing-unit.instructions.md).
- **Integration tests** (`*.IntegrationTest` projects): verify that components work together. Database tests use a PostgreSQL Testcontainer, so Docker must be running. See [testing-integration.instructions.md](/.ai/testing/testing-integration.instructions.md).
- Performance and HIL tests (TBD).

All tests use MSTest. General rules are in [testing-general.instructions.md](/.ai/testing/testing-general.instructions.md).



## Repository Documentation
- [Repository README](/README.md)
- [Documentation Structure And Definition](/Docs/Documentation.md)

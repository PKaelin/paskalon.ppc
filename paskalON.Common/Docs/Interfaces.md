# Common Interfaces
The main public types of the Common libraries.

Back to the [Overview](./Overview.md).


## paskalON.Dataface
- `IModbusDataface`, `IModbusRegister`, `ModbusRegisterEntry`, `ModbusDataType`, `ModbusScale`: Modbus register definitions.
- `IC37Dataface`, `IC37Register`, `C37RegisterEntry`, `C37SignalType`: C37 register definitions.

How the dataface is used is described in the [Device Service Dataface design](/paskalON.DeviceService/Docs/Design/Dataface.md).


## paskalON.Hosting
- `StartupRetry`, `StartupRetryOptions`: retries a startup step while a dependency is not reachable. Configured with `STARTUP_RETRY_COUNT` and `STARTUP_RETRY_INTERVAL_SECONDS`.
- `DatabaseTransientExceptionFilter`, `HttpTransientExceptionFilter`: decide which exceptions are retried.


## paskalON.Infrastructure
- `IRepository<TContext, TEntity>`, `Repository`, `RepositoryBase`: generic EF Core repositories.


## paskalON.Infrastructure.PostgreSql
- `PostgresFixture`: starts and stops the PostgreSQL container once per test assembly (`StartAsync`, `StopAsync`).
- `PostgresTestBase<TContext>`: creates a temporary database per test.

See the [integration testing instructions](/.ai/testing/testing-integration.instructions.md#database-integration-tests).


## paskalON.Maths
- Curves: `PiecewiseFunction`, `LinearPointFunction`, `ExponentialFunction`, `Exponential2PointFunction`, `LogarithmicEasingFunction`, `LogarithmicScaledShiftedFunction` (implement `ICalculateOutputFunction`).
- Sums: `RiemannSum`.
- Coordinates: `EuclideanDistance`, `VectorGenerator`.
- Randoms: `RandomWalker`, `RandomExtension`.


## paskalON.PhysicalUnits
- Powers: `ActivePower`, `ReactivePower`, `ApparentPower`, `ComplexPower`, `Voltage`, `CurrentScalar`, `IecPowerFactor`, `IeeePowerFactor`.
- Energies: `Energy`, `ReactiveEnergy`.
- `Temperature`, `StateOfChargeCalculator`, `MetricPrefix`.


## paskalON.Telemetry
- `IMetricsPublisher`, `MetricsPublisher`, `MetricType`: metrics publisher, see [Telemetry](/Docs/03-Design/Telemetry.md#metrics-publisher).


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)

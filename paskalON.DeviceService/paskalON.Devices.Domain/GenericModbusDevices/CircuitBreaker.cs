using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
using paskalON.Domains.Telemetry;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Circuit breaker is a safety threshold that automatically pauses or stops the circuit when a predefined
    /// limit is reached. It acts as a fail-safe.
    /// </summary>
    public class CircuitBreaker : GenericModbusDeviceBase
    {
        /// <summary>
        /// Circuit breaker configuration.
        /// </summary>
        private readonly CircuitBreakerConfig _config;


        /// <summary>
        /// Constructor of <see cref="CircuitBreaker"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The circuit breaker configuration.</param>
        /// <param name="genericModbusEntries">List of generic Modbus entries.</param>
        /// <param name="metricsPublisher">Metrics publisher interface.</param>
        public CircuitBreaker(ILogger logger, CircuitBreakerConfig config, List<GenericModbusEntryBase> genericModbusEntries, IMetricsPublisher<CircuitBreaker> metricsPublisher)
            : base(logger, config, genericModbusEntries, (IMetricsPublisher<GenericModbusDeviceBase>)metricsPublisher)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}

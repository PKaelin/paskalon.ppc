using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
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
        public CircuitBreaker(ILogger logger, CircuitBreakerConfig config) : base(logger, config)
        {
            _config = config;
        }
    }
}

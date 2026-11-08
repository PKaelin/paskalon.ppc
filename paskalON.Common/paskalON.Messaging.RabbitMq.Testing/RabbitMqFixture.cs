// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Testcontainers.RabbitMq;

namespace paskalON.Messaging.RabbitMq.Testing
{
    /// <summary>
    /// A fixture class for managing a RabbitMQ container for integration tests.
    /// </summary>
    public static class RabbitMqFixture
    {
        /// <summary>
        /// The RabbitMQ container instance used for integration tests.
        /// </summary>
        private static RabbitMqContainer? _container;


        /// <summary>
        /// Gets the AMQP connection URI of the RabbitMQ container.
        /// </summary>
        public static Uri ConnectionUri => new Uri(_container?.GetConnectionString()
            ?? throw new InvalidOperationException("RabbitMQ container has not been started."));


        /// <summary>
        /// Starts the RabbitMQ container for integration tests.
        /// </summary>
        /// <param name="image">The Docker image, the same version as in the infrastructure docker compose.</param>
        /// <returns>Task.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a container has already been started. Call <see cref="StopAsync"/> first.
        /// </exception>
        /// <remarks>
        /// The container is only registered after it started successfully.
        /// If the start fails, the container is disposed and the fixture stays stopped.
        /// </remarks>
        public static async Task StartAsync(string image = "rabbitmq:4.3.3")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(image);

            if (_container is not null)
            {
                throw new InvalidOperationException("RabbitMQ container has already been started. Call StopAsync before starting a new container.");
            }

            RabbitMqContainer container = new RabbitMqBuilder(image).Build();

            try
            {
                await container.StartAsync();
            }
            catch
            {
                await container.DisposeAsync();

                throw;
            }

            _container = container;
        }


        /// <summary>
        /// Closes all client connections from the broker side, like a broker restart or network failure,
        /// so that the clients recover their connections.
        /// </summary>
        /// <returns>Task.</returns>
        public static async Task CloseAllConnectionsAsync()
        {
            RabbitMqContainer container = _container ?? throw new InvalidOperationException("RabbitMQ container has not been started.");
            string[] command = new string[] { "rabbitmqctl", "close_all_connections", "Closed by the test." };

            if (await container.ExecAsync(command) is { ExitCode: not 0 } failedResult)
            {
                throw new InvalidOperationException($"Closing the connections failed: {failedResult.Stderr}");
            }
        }


        /// <summary>
        /// Stops and disposes of the RabbitMQ container.
        /// </summary>
        /// <returns>Task.</returns>
        public static async Task StopAsync()
        {
            if (_container is not null)
            {
                await _container.DisposeAsync();
                _container = null;
            }
        }
    }
}

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

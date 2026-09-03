using Microsoft.Extensions.Logging;
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Protocols.C37118;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.Meters.PowerMeters.Simples
{
    public class SystemPowerMeterSimpleV1Proxy : SystemPowerMeter, IDisposable
    {
        /// <summary>
        /// C37 client communication.
        /// </summary>
        private readonly IC37Client _client;

        /// <summary>
        /// Constructor of <see cref="SystemPowerMeterSimpleV1Proxy"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The power meter configuration.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The data face interface.</param>
        /// <param name="client">The C37 client interface.</param>
        public SystemPowerMeterSimpleV1Proxy(ILogger logger, SystemPowerMeterConfig config, IMetricsPublisher publisher,
            IC37Dataface dataface, IC37Client client) : base(logger, config, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(dataface);

            _client = client;
            _client.OnCommunicationError += OnCommunicationError;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task ConnectAsync()
        {
            if (_client.State != C37ClientState.Connected || _client.State != C37ClientState.Connecting)
            {
                await _client.StartStreamingAsync();
            }

            await base.ConnectAsync();
            await _client.SendCommandAsync(C37CommandType.TurnOnTransmission);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task DisconnectAsync()
        {
            await base.DisconnectAsync();
            await _client.SendCommandAsync(C37CommandType.TurnOffTransmission);
        }


        /// <summary>
        ///  Triggered on client communication error.
        /// </summary>
        /// <param name="sender">The communication client.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCommunicationError(object? sender, EventArgs e)
        {
            // Logging and even invocation is done in the setter of the CommunicationError property
            CommunicationError = true;
        }


        /// <summary>
        /// Dispose instance.
        /// </summary>
        public void Dispose()
        {
            _client.OnCommunicationError -= OnCommunicationError;
        }
    }
}

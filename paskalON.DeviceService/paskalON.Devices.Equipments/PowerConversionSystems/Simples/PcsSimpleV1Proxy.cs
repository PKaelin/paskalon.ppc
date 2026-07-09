// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Communication.Protocols.Modbus;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.Modbus;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.PowerConversionSystems.Simples
{

    // TODO: Implement PcsSimpleV1Proxy


    public class PcsSimpleV1Proxy : PowerConversionSystemBase, IModbusPollingEngine
    {
        /// <summary>
        /// Modbus data face for updating the domain data.
        /// </summary>
        private readonly IModbusDataface _dataface;


        /// <summary>
        /// Modbus client communication.
        /// </summary>
        private readonly IModbusClient _client;


        /// <summary>
        /// The Modbus polling engine.
        /// </summary>
        private readonly ModbusPollingEngine _pollingEngine;


        /// <summary>
        /// Constructor of <see cref="PcsSimpleV1Proxy"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The power conversion system configuration.</param>
        /// <param name="derUnit">The parent DER unit.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="device">The device interface.</param>
        /// <param name="dataface">The data face interface.</param>
        /// <param name="client">The Modbus client interface.</param>
        public PcsSimpleV1Proxy(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher publisher,
            IPowerConversionSystem device, IModbusDataface dataface, IModbusClient client) : base(logger, config, derUnit, publisher, device)
        {
            ArgumentNullException.ThrowIfNull(dataface);
            ArgumentNullException.ThrowIfNull(client);

            _dataface = dataface;
            _client = client;
            _pollingEngine = new ModbusPollingEngine(client, dataface);
        }



        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public async Task PollAsync(int interval, CancellationToken cancellationToken)
        {
            await _pollingEngine.PollAsync(interval, cancellationToken);
        }



        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        protected override void RegisterDataface()
        {
            // _config.ModbusConfig.ModbusConnectionConfig.PollingIntervalMilliseconds

            // Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.Register<PcsSimpleV1Proxy, PcsState>(this, nameof(State), (x, v) => x.State = v, (int)PcsSimpleV1Description.Register.CurrentStatus, 1));

            // Dataface.Register<PcsSimpleV1Proxy, IModbusRegister>(r => r.RegisterRange()
        }



        /// <summary>
        /// Convert the device status with the domain status.
        /// </summary>
        /// <param name="status">The device status.</param>
        private void SetStatus(int status)
        {
            // Check whether we have defined all status
            if (Enum.IsDefined(typeof(PcsSimpleV1Description.Status), status) == false)
            {
                _logger.LogError($"Undefined status reported by the {nameof(PcsSimpleV1Proxy)}. Status: {status}");
            }

            switch (status)
            {
                case (int)PcsSimpleV1Description.Status.Initialization:
                    State = PcsState.Starting;
                    break;
                case (int)PcsSimpleV1Description.Status.Off:
                case (int)PcsSimpleV1Description.Status.Stop:
                case (int)PcsSimpleV1Description.Status.Fault:
                    State = PcsState.Stopped;
                    break;
                case (int)PcsSimpleV1Description.Status.Standby:
                    State = PcsState.Standby;
                    break;
                case (int)PcsSimpleV1Description.Status.NightMode:
                    State = PcsState.NightMode;
                    break;
                default:
                    // Do not change the state
                    break;
            }
        }
    }
}

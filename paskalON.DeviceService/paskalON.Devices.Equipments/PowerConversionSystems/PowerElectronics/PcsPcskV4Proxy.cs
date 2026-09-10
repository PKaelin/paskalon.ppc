// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.PowerConversionSystems.PowerElectronics
{
    public class PcsPcskV4Proxy : PowerConversionSystemBase
    {
        // TODO: Implement PcsPcskV4Proxy


        /// <summary>
        /// Modbus client communication.
        /// </summary>
        private readonly IModbusClient _client;



        public PcsPcskV4Proxy(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher publisher,
            IDataface dataface, IModbusClient client) : base(logger, config, derUnit, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(dataface);

            _client = client;
            _client.OnCommunicationError += OnCommunicationError;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task StartAsync()
        {
            if (_client.State != ModbusClientState.Connected || _client.State != ModbusClientState.Connecting)
            {
                await _client.ConnectAsync();
            }

            await base.StartAsync();
            await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState, 1, ModbusDataType.MbInt16);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task StopAsync()
        {
            if (_client.State == ModbusClientState.Connected)
            {
                await base.StopAsync();
                await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState, 0, ModbusDataType.MbInt16);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override async Task StandbyAsync(double? standbyActivePower = null)
        {
            if (_client.State == ModbusClientState.Connected)
            {
                await base.StandbyAsync(standbyActivePower);

                if (standbyActivePower != null)
                {
                    await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.PReference, (double)standbyActivePower, ModbusDataType.MbInt16);
                }

                await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.SelectorState, 3, ModbusDataType.MbInt16);
            }
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public override async Task SetActivePowerTargetAsync(double? value)
        {
            if (_client.State == ModbusClientState.Connected)
            {
                await base.SetActivePowerTargetAsync(value);

                if (ActivePowerTarget.HasValue)
                {
                    await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.PReference, ActivePowerTarget.Value.KiloWatts, ModbusDataType.MbInt16);
                }
            }
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public override async Task SetReactivePowerTargetAsync(double? value)
        {
            if (_client.State == ModbusClientState.Connected)
            {
                await base.SetReactivePowerTargetAsync(value);

                if (ReactivePowerTarget.HasValue)
                {
                    await _client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.QReference, ReactivePowerTarget.Value.KiloVoltAmperesReactive, ModbusDataType.MbInt16);
                }
            }
        }


        protected override void RegisterDataface()
        {
            throw new NotImplementedException();
        }
    }
}

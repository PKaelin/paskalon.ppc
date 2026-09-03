// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus;

namespace paskalON.Devices.Equipments.Modbus
{
    /// <summary>
    /// Implementation of the Modbus polling engine <see cref="IModbusPollingEngine"/>.
    /// </summary>
    public class ModbusPollingEngine : IModbusPollingEngine
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// The Modbus client.
        /// </summary>
        private readonly IModbusClient _client;

        /// <summary>
        /// The Modbus dataface for loos coupling.
        /// </summary>
        private readonly IModbusDataface _dataface;


        /// <summary>
        /// Constructor of <see cref="ModbusPollingEngine"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="client">The Modbus client interface.</param>
        /// <param name="dataface">The Modbus data interface.</param>
        public ModbusPollingEngine(ILogger logger, IModbusClient client, IModbusDataface dataface)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(dataface);

            _logger = logger;
            _client = client;
            _dataface = dataface;
            _logger.LogInformation("Modbus polling engine created for: {Name} {Address} {Port}", dataface.Name, client.ServerAddress, client.ServerPort);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task PollAsync(int currentInterval, CancellationToken cancellationToken)
        {
            try
            {
                foreach (ModbusPollingRangeEntry range in _dataface.PollingRanges)
                {
                    if (currentInterval % range.Interval == 0)
                    {
                        ushort startAddress = range.From;
                        ushort endAddress = range.To;
                        ushort[]? rawShortData = null;
                        bool[]? rawBoolData = null;

                        switch (range.RegistryType)
                        {
                            // Fetch raw data via the decoupled client.
                            case ModbusRegistryType.Coil:
                                rawBoolData = await _client.ReadCoilsAsync(startAddress, endAddress, cancellationToken);
                                break;
                            case ModbusRegistryType.DiscreteInput:
                                rawBoolData = await _client.ReadDiscreteInputsAsync(startAddress, endAddress, cancellationToken);
                                break;
                            case ModbusRegistryType.InputRegister:
                                rawShortData = await _client.ReadInputRegistersAsync(startAddress, endAddress, cancellationToken);
                                break;
                            case ModbusRegistryType.HoldingRegister:
                                rawShortData = await _client.ReadHoldingRegistersAsync(startAddress, endAddress, cancellationToken);
                                break;

                        }

                        // Filter registers that fall within this range
                        IEnumerable<IModbusRegisterEntry> registers = _dataface.Registers.Where(r => r.Register >= startAddress && r.Register <= endAddress).OrderBy(n => n.Register);

                        // Map, scale, handle word order and call update
                        foreach (IModbusRegisterEntry register in registers)
                        {
                            if (rawShortData != null)
                            {
                                object? parsedValue = _client.ConvertRawData(rawShortData, register, startAddress);
                                // Only update if there is a value.
                                if (parsedValue != null)
                                {
                                    register.Update(parsedValue);
                                }
                            }
                            else if (rawBoolData != null)
                            {
                                object parsedValue = _client.ConvertRawData(rawBoolData, register, startAddress);
                                register.Update(parsedValue);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occurred in Modbus poll. {ModbusError}:", ex.Message);
            }
        }
    }
}
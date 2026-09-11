// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Converters;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Helpers for reading and writing simulated Modbus registers.
    /// </summary>
    internal static class RegisterSimulation
    {
        /// <summary>
        /// Modbus value converter.
        /// </summary>
        private static readonly ModbusDataConverter _converter = new ModbusDataConverter();


        /// <summary>
        /// Reads one holding register.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="address">Register address.</param>
        /// <returns>The register value.</returns>
        public static ushort Read(IModbusDataStore store, int address)
        {
            return Read<ushort>(store, address, ModbusDataType.MbUint16);
        }


        /// <summary>
        /// Reads a typed value from holding registers.
        /// </summary>
        /// <typeparam name="T">Expected value type.</typeparam>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="address">First register address.</param>
        /// <param name="dataType">Modbus data type.</param>
        /// <param name="scale">Modbus scale.</param>
        /// <returns>The decoded value.</returns>
        public static T Read<T>(IModbusDataStore store, int address, ModbusDataType dataType, double scale = 1)
        {
            ArgumentNullException.ThrowIfNull(store);

            int registerCount = _converter.GetRegisterLength(dataType);
            ushort[] registers = store.HoldingRegisters.ReadPoints((ushort)address, (ushort)registerCount);
            ModbusRegisterEntry<object, object> entry = new ModbusRegisterEntry<object, object>(new object(), "Simulation",
                (_, _) => { }, address, scale, dataType, 0);

            object? value = _converter.ConvertRawData(registers, entry, (ushort)address);

            if (value is null)
            {
                throw new InvalidOperationException($"Register at address {address} did not contain a value.");
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }


        /// <summary>
        /// Writes one holding register.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="address">Register address.</param>
        /// <param name="value">Register value.</param>
        public static void Write(IModbusDataStore store, int address, ushort value)
        {
            Write(store, address, value, ModbusDataType.MbUint16);
        }


        /// <summary>
        /// Writes a typed value to holding registers.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="address">First register address.</param>
        /// <param name="value">Value to encode.</param>
        /// <param name="dataType">Modbus data type.</param>
        /// <param name="scale">Modbus scale.</param>
        public static void Write(IModbusDataStore store, int address, double value, ModbusDataType dataType, double scale = 1)
        {
            ArgumentNullException.ThrowIfNull(store);

            ushort[] registers = _converter.RegisterArrayFromValue(value, dataType, scale);
            store.HoldingRegisters.WritePoints((ushort)address, registers);
        }
    }
}
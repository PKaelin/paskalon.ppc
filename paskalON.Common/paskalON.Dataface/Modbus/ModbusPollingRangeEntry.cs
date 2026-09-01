// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// Implementation of the Modbus polling range entry.
    /// </summary>
    public class ModbusPollingRangeEntry
    {
        /// <summary>
        /// Modbus register from.
        /// </summary>
        public ushort From { get; init; }


        /// <summary>
        /// Modbus register to.
        /// </summary>
        public ushort To { get; init; }


        /// <summary>
        /// Modbus register type <see cref="ModbusRegistryType"/>.
        /// </summary>
        public ModbusRegistryType RegistryType { get; init; }


        /// <summary>
        /// The interval based on a polling definition.
        /// </summary>
        public int Interval
        {
            get;
            init { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value); field = value; }
        }


        /// <summary>
        /// Constructor of <see cref="ModbusPollingRangeEntry"/>.
        /// </summary>
        /// <param name="from">Modbus register from.</param>
        /// <param name="to">Modbus register to.</param>
        /// <param name="registryType">Modbus register type <see cref="ModbusRegistryType"/>.</param>
        /// <param name="interval">The interval based on a polling definition.</param>
        public ModbusPollingRangeEntry(ushort from, ushort to, ModbusRegistryType registryType, int interval)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(to, from);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(from, to);

            From = from;
            To = to;
            RegistryType = registryType;
            Interval = interval;
        }
    }
}

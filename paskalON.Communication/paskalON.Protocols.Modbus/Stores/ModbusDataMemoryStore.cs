// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.Modbus.Stores
{
    /// <summary>
    /// Modbus data store implementation that uses in-memory storage.
    /// </summary>
    public class ModbusDataMemoryStore : IModbusDataStore
    {
        /// <summary>
        /// Data lock object
        /// </summary>
        private readonly object _dataLock = new();


        /// <summary>
        /// Array of coil registers.
        /// </summary>
        protected readonly bool[] _coils;


        /// <summary>
        /// Array of discrete input registers.
        /// </summary>
        protected readonly bool[] _discreteInputs;


        /// <summary>
        /// Array of holding registers.
        /// </summary>
        protected readonly ushort[] _holdingRegisters;


        /// <summary>
        /// Array of input registers.
        /// </summary>
        protected readonly ushort[] _inputRegisters;


        /// <summary>
        /// Constructor of <see cref="ModbusDataMemoryStore"/>.
        /// </summary>
        /// <param name="coilCount">The coil register buffer size.</param>
        /// <param name="discreteInputCount">The discrete input register buffer size.</param>
        /// <param name="holdingRegisterCount">The holding register buffer size.</param>
        /// <param name="inputRegisterCount">The input register buffer size.</param>
        public ModbusDataMemoryStore(int coilCount = 65535, int discreteInputCount = 65535, int holdingRegisterCount = 65535, int inputRegisterCount = 65535)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(coilCount);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(coilCount, 65535);
            ArgumentOutOfRangeException.ThrowIfNegative(discreteInputCount);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(discreteInputCount, 65535);
            ArgumentOutOfRangeException.ThrowIfNegative(holdingRegisterCount);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(holdingRegisterCount, 65535);
            ArgumentOutOfRangeException.ThrowIfNegative(inputRegisterCount);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(inputRegisterCount, 65535);

            _coils = new bool[coilCount];
            _discreteInputs = new bool[discreteInputCount];
            _holdingRegisters = new ushort[holdingRegisterCount];
            _inputRegisters = new ushort[inputRegisterCount];
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool[] ReadCoils(ushort startAddress, ushort endAddress)
        {
            lock (_dataLock)
            {
                ValidateRange(startAddress, endAddress, _coils.Length);
                return GetRange(_coils, startAddress, endAddress);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool[] ReadDiscreteInputs(ushort startAddress, ushort endAddress)
        {
            lock (_dataLock)
            {
                ValidateRange(startAddress, endAddress, _discreteInputs.Length);
                return GetRange(_discreteInputs, startAddress, endAddress);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ushort[] ReadHoldingRegisters(ushort startAddress, ushort endAddress)
        {
            lock (_dataLock)
            {
                ValidateRange(startAddress, endAddress, _holdingRegisters.Length);
                return GetRange(_holdingRegisters, startAddress, endAddress);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ushort[] ReadInputRegisters(ushort startAddress, ushort endAddress)
        {
            lock (_dataLock)
            {
                ValidateRange(startAddress, endAddress, _inputRegisters.Length);
                return GetRange(_inputRegisters, startAddress, endAddress);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void WriteCoils(ushort startAddress, bool[] values)
        {
            lock (_dataLock)
            {
                ValidateWriteRange(startAddress, values.Length, _coils.Length);
                Array.Copy(values, 0, _coils, startAddress, values.Length);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void WriteDiscreteInputs(ushort startAddress, bool[] values)
        {
            lock (_dataLock)
            {
                ValidateWriteRange(startAddress, values.Length, _discreteInputs.Length);
                Array.Copy(values, 0, _discreteInputs, startAddress, values.Length);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void WriteHoldingRegisters(ushort startAddress, ushort[] values)
        {
            lock (_dataLock)
            {
                ValidateWriteRange(startAddress, values.Length, _holdingRegisters.Length);
                Array.Copy(values, 0, _holdingRegisters, startAddress, values.Length);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void WriteInputRegisters(ushort startAddress, ushort[] values)
        {
            lock (_dataLock)
            {
                ValidateWriteRange(startAddress, values.Length, _inputRegisters.Length);
                Array.Copy(values, 0, _inputRegisters, startAddress, values.Length);
            }
        }


        /// <summary>
        /// Validates the range using start and end address and compares it to the capacity.
        /// </summary>
        /// <param name="startAddress">The start address.</param>
        /// <param name="endAddress">The end address.</param>
        /// <param name="capacity">The capacity.</param>
        private void ValidateRange(ushort startAddress, ushort endAddress, int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(endAddress, startAddress, "End address must be greater than or equal to start address.");
            int length = endAddress - startAddress + 1;
            ArgumentOutOfRangeException.ThrowIfGreaterThan(startAddress + length, capacity, "Requested range exceeds the data store.");
        }


        /// <summary>
        /// Validates the range using start address and count (lenght) and compares it to the capacity.
        /// </summary>
        /// <param name="startAddress">The start address.</param>
        /// <param name="count">The count (lenght).</param>
        /// <param name="capacity">The capacity.</param>
        private void ValidateWriteRange(ushort startAddress, int count, int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0, "At least one value is required.");
            ArgumentOutOfRangeException.ThrowIfGreaterThan((long)startAddress + count, capacity);
        }


        /// <summary>
        /// Gets an array of boolean values from a start to end address
        /// </summary>
        /// <param name="source">The source to get the values from.</param>
        /// <param name="startAddress">The start address.</param>
        /// <param name="endAddress">The end address.</param>
        /// <returns>Array of boolean values.</returns>
        private bool[] GetRange(bool[] source, ushort startAddress, ushort endAddress)
        {
            bool[] result = new bool[endAddress - startAddress + 1];
            Array.Copy(source, startAddress, result, 0, result.Length);

            return result;
        }


        /// <summary>
        /// Gets an array of ushort values from a start to end address
        /// </summary>
        /// <param name="source">The source to get the values from.</param>
        /// <param name="startAddress">The start address.</param>
        /// <param name="endAddress">The end address.</param>
        /// <returns>Array of ushort values.</returns>
        private ushort[] GetRange(ushort[] source, ushort startAddress, ushort endAddress)
        {
            ushort[] result = new ushort[endAddress - startAddress + 1];
            Array.Copy(source, startAddress, result, 0, result.Length);

            return result;
        }
    }
}

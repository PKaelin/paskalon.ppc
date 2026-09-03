// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using NModbus;

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
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// These are called Coils in the Modbus standard.
        /// </remarks>
        public IPointSource<bool> CoilDiscretes { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// These are called DiscreteInputs in the Modbus standard.
        /// </remarks>
        public IPointSource<bool> CoilInputs { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IPointSource<ushort> HoldingRegisters { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IPointSource<ushort> InputRegisters { get; init; }


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

            CoilDiscretes = new MemoryPointSource<bool>(
                (startAddress, numberOfPoints) => ReadPoints(_coils, startAddress, numberOfPoints),
                (startAddress, values) => WritePoints(_coils, startAddress, values));
            CoilInputs = new MemoryPointSource<bool>(
                (startAddress, numberOfPoints) => ReadPoints(_discreteInputs, startAddress, numberOfPoints),
                (startAddress, values) => WritePoints(_discreteInputs, startAddress, values));
            HoldingRegisters = new MemoryPointSource<ushort>(
                (startAddress, numberOfPoints) => ReadPoints(_holdingRegisters, startAddress, numberOfPoints),
                (startAddress, values) => WritePoints(_holdingRegisters, startAddress, values));
            InputRegisters = new MemoryPointSource<ushort>(
                (startAddress, numberOfPoints) => ReadPoints(_inputRegisters, startAddress, numberOfPoints),
                (startAddress, values) => WritePoints(_inputRegisters, startAddress, values));

        }


        /// <summary>
        /// Read points from a source.
        /// </summary>
        /// <typeparam name="T">Type of return values.</typeparam>
        /// <param name="source">The data source to read from.</param>
        /// <param name="startAddress">The start address to read from.</param>
        /// <param name="numberOfPoints">The number of points to read.</param>
        /// <returns>Array of point of type T.</returns>
        private T[] ReadPoints<T>(T[] source, ushort startAddress, ushort numberOfPoints)
        {
            long endAddress = (long)startAddress + numberOfPoints - 1;
            ValidateRange(startAddress, (ushort)endAddress, source.Length);

            lock (_dataLock)
            {
                T[] result = new T[numberOfPoints];
                Array.Copy(source, startAddress, result, 0, numberOfPoints);

                return result;
            }
        }


        /// <summary>
        /// Writes points to a source.
        /// </summary>
        /// <typeparam name="T">Type of points to write.</typeparam>
        /// <param name="destination">The destination to write to.</param>
        /// <param name="startAddress">The start address to write to.</param>
        /// <param name="values">The values to write.</param>
        private void WritePoints<T>(T[] destination, ushort startAddress, T[] values)
        {
            ArgumentNullException.ThrowIfNull(values, "Write points doesn't contain any values.");

            lock (_dataLock)
            {
                ValidateWriteRange(startAddress, values.Length, destination.Length);
                Array.Copy(values, 0, destination, startAddress, values.Length);
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
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using NModbus;

namespace paskalON.Protocols.Modbus.Stores
{
    /// <summary>
    /// Memory based implementation of the IPointSource interface for Modbus data storage.
    /// </summary>
    /// <typeparam name="T">Type of the point source.</typeparam>
    internal sealed class MemoryPointSource<T> : IPointSource<T>
    {
        /// <summary>
        /// Read function delegate for reading points from the memory source.
        /// </summary>
        private readonly Func<ushort, ushort, T[]> _read;


        /// <summary>
        /// Write function delegate for writing points to the memory source.
        /// </summary>
        private readonly Action<ushort, T[]> _write;


        /// <summary>
        /// Constructor of <see cref="MemoryPointSource"/>.
        /// </summary>
        /// <param name="read">Read function delegate for reading points from the memory source.</param>
        /// <param name="write">Write function delegate for writing points to the memory source.</param>
        public MemoryPointSource(Func<ushort, ushort, T[]> read, Action<ushort, T[]> write)
        {
            _read = read;
            _write = write;
        }


        /// <summary>
        /// Reads points from the memory source starting at the specified address and for the specified number of points.
        /// </summary>
        /// <param name="startAddress">The start address.</param>
        /// <param name="numberOfPoints">The number of points.</param>
        /// <returns></returns>
        public T[] ReadPoints(ushort startAddress, ushort numberOfPoints)
        {
            return _read(startAddress, numberOfPoints);
        }


        /// <summary>
        /// Writes points to the memory source starting at the specified address.
        /// </summary>
        /// <param name="startAddress">The start address.</param>
        /// <param name="points">The points to write.</param>
        public void WritePoints(ushort startAddress, T[] points)
        {
            _write(startAddress, points);
        }
    }
}

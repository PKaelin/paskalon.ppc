// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using NModbus;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Modbus data store of a simulated device that records the holding registers written by Modbus clients.
    /// </summary>
    /// <remarks>
    /// Modbus servers and clients use this store. Every holding register they write is recorded, even when the
    /// written value equals the current value. The simulation writes its own values through <see cref="Device"/>,
    /// which bypasses the recording. This allows a simulation to react on repeated commands (e.g. a start command
    /// that is written again to leave the fault state).
    /// </remarks>
    public sealed class SimulationModbusDataStore : IModbusDataStore
    {
        /// <summary>
        /// Holding register addresses written by clients and not yet taken by the simulation.
        /// </summary>
        private readonly HashSet<ushort> _writtenAddresses = new HashSet<ushort>();


        /// <summary>
        /// Data lock object.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Underlying data store the simulation writes to without recording.
        /// </summary>
        public IModbusDataStore Device { get; }


        /// <inheritdoc/>
        public IPointSource<bool> CoilDiscretes { get => Device.CoilDiscretes; }


        /// <inheritdoc/>
        public IPointSource<bool> CoilInputs { get => Device.CoilInputs; }


        /// <inheritdoc/>
        public IPointSource<ushort> HoldingRegisters { get; }


        /// <inheritdoc/>
        public IPointSource<ushort> InputRegisters { get => Device.InputRegisters; }


        /// <summary>
        /// Constructor of <see cref="SimulationModbusDataStore"/>.
        /// </summary>
        /// <param name="device">Underlying data store.</param>
        public SimulationModbusDataStore(IModbusDataStore device)
        {
            ArgumentNullException.ThrowIfNull(device);

            Device = device;
            HoldingRegisters = new RecordingPointSource(this);
        }


        /// <summary>
        /// Takes the write record of a holding register address.
        /// </summary>
        /// <param name="address">Holding register address.</param>
        /// <returns>True if a client wrote the address since the last call, otherwise false.</returns>
        public bool TryTakeWrite(int address)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(address);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(address, ushort.MaxValue);

            lock (_dataLock)
            {
                return _writtenAddresses.Remove((ushort)address);
            }
        }


        /// <summary>
        /// Records the written holding register addresses.
        /// </summary>
        /// <param name="startAddress">First written address.</param>
        /// <param name="count">Number of written registers.</param>
        private void RecordWrite(ushort startAddress, int count)
        {
            lock (_dataLock)
            {
                for (int offset = 0; offset < count && startAddress + offset <= ushort.MaxValue; offset++)
                {
                    _writtenAddresses.Add((ushort)(startAddress + offset));
                }
            }
        }


        /// <summary>
        /// Holding register point source that records client writes.
        /// </summary>
        private sealed class RecordingPointSource : IPointSource<ushort>
        {
            /// <summary>
            /// Owning store.
            /// </summary>
            private readonly SimulationModbusDataStore _owner;


            /// <summary>
            /// Constructor of <see cref="RecordingPointSource"/>.
            /// </summary>
            /// <param name="owner">Owning store.</param>
            public RecordingPointSource(SimulationModbusDataStore owner)
            {
                _owner = owner;
            }


            /// <inheritdoc/>
            public ushort[] ReadPoints(ushort startAddress, ushort numberOfPoints)
            {
                return _owner.Device.HoldingRegisters.ReadPoints(startAddress, numberOfPoints);
            }


            /// <inheritdoc/>
            public void WritePoints(ushort startAddress, ushort[] points)
            {
                ArgumentNullException.ThrowIfNull(points);

                _owner.Device.HoldingRegisters.WritePoints(startAddress, points);
                _owner.RecordWrite(startAddress, points.Length);
            }
        }
    }
}

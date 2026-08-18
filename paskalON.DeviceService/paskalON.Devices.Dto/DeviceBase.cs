// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto
{
    /// <summary>
    /// The base class for all device DTOs.
    /// </summary>
    /// <typeparam name="TDefinition">Type of the definition DTO.</typeparam>
    /// <typeparam name="TCore">Type of the core DTO.</typeparam>
    /// <typeparam name="TDetail">Type of the detail DTO.</typeparam>
    public abstract class DeviceBase<TDefinition, TCore, TDetail>
            where TDefinition : class, IDeviceDefinition
            where TCore : class, IDevice
            where TDetail : class, IDevice
    {
        /// <summary>
        /// DTO definition object.
        /// </summary>
        private TDefinition _definition;


        /// <summary>
        /// DTO core object.
        /// </summary>
        private TCore? _core;


        /// <summary>
        /// DTO detail object.
        /// </summary>
        private TDetail? _detail;


        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int DeviceId { get => _definition.DeviceId; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Name { get => _definition.Name; }


        /// <summary>
        /// DTO definition object.
        /// </summary>
        public TDefinition Definition
        {
            get { lock (dataLock) { return _definition; } }
        }


        /// <summary>
        /// DTO core object.
        /// </summary>
        public TCore? Core
        {
            get { lock (dataLock) { return _core; } }
        }


        /// <summary>
        /// DTO detail object.
        /// </summary>
        public TDetail? Detail
        {
            get { lock (dataLock) { return _detail; } }
        }


        /// <summary>
        /// Constructor of <see cref="DeviceBase"/>.
        /// </summary>
        /// <param name="definition">DTO definition object.</param>
        public DeviceBase(TDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            _definition = definition;
        }


        /// <summary>
        /// Update the DTOs definition.
        /// </summary>
        /// <param name="definition">DTO definition object.</param>
        /// <exception cref="InvalidOperationException">Throws exception when device IDs are not the same.</exception>
        public void UpdateDefinition(TDefinition definition)
        {
            if (definition.DeviceId != DeviceId)
            {
                throw new InvalidOperationException($"Message belongs to device {definition.DeviceId}, " + $"but this is device {DeviceId}.");
            }

            lock (dataLock)
            {
                _definition = definition;
            }
        }



        /// <summary>
        /// Update the DTOs core.
        /// </summary>
        /// <param name="core">DTO core object.</param>
        /// <exception cref="InvalidOperationException">Throws exception when device IDs are not the same.</exception>
        public void UpdateCore(TCore core)
        {
            if (core.DeviceId != DeviceId)
            {
                throw new InvalidOperationException($"Message belongs to device {core.DeviceId}, " + $"but this is device {DeviceId}.");
            }

            lock (dataLock)
            {
                _core = core;
            }
        }


        /// <summary>
        /// Update the DTOs detail.
        /// </summary>
        /// <param name="detail">DTO detail object.</param>
        /// <exception cref="InvalidOperationException">Throws exception when device IDs are not the same.</exception>
        public void UpdateDetail(TDetail detail)
        {
            if (detail.DeviceId != DeviceId)
            {
                throw new InvalidOperationException($"Message belongs to device {detail.DeviceId}, " + $"but this is device {DeviceId}.");
            }

            lock (dataLock)
            {
                _detail = detail;
            }
        }
    }
}
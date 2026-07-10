// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// ModbusRegister implementation for <see cref="IModbusRegister"/> and <see cref="IModbusDataface"/> interfaces.
    /// </summary>
    public class ModbusRegister : IModbusRegister, IModbusDataface
    {
        /// <summary>
        /// <inheritdoc/>
        /// IModbusDataface implementation of Registers <see cref="IModbusDataface"/>.
        /// </summary>
        public List<IModbusRegisterEntry> Registers { get; } = new List<IModbusRegisterEntry>();


        /// <summary>
        /// <inheritdoc/>
        /// IModbusDataface implementation of PollingRanges <see cref="IModbusDataface"/>.
        /// </summary>
        public List<ModbusPollingRangeEntry> PollingRanges { get; } = new List<ModbusPollingRangeEntry>();


        /// <summary>
        /// <inheritdoc/>
        /// IDataface implementation of Register <see cref="IDataface"/>.
        /// </summary>
        public void Register<TDevice, TCom>(Action<TCom> com)
        {
            ArgumentNullException.ThrowIfNull(com);

            if (this is not TCom typedCom)
            {
                // At this point it should be IModbusRegister
                throw new ArgumentException($"Register type {typeof(TCom).Name} is not implemented by this class");
            }

            com.Invoke(typedCom);
        }


        /// <summary>
        /// <inheritdoc/>
        /// IModbusRegister implementation of Register <see cref="IModbusRegister"/>.
        /// </summary>
        public void Register<TDevice, TProperty>(TDevice instance, string name, Action<TDevice, TProperty> setter, int register,
            double scale, ModbusDataType dataType, int offset = 0)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(setter);
            ArgumentOutOfRangeException.ThrowIfLessThan(offset, 0);

            if (Registers.Any(r => r.Name == name) == true)
            {
                throw new ArgumentException($"Register with name {name} is already registered");
            }

            if (Registers.Any(r => r.Register == register) == true)
            {
                throw new ArgumentException($"Register with register {register} is already registered");
            }


            Registers.Add(new ModbusRegisterEntry<TDevice, TProperty>(instance, name, setter, register, scale, dataType, offset));
        }


        /// <summary>
        /// <inheritdoc/>
        /// IModbusRegister implementation of RegisterRange <see cref="IModbusRegister"/>.
        /// </summary>
        public void RegisterRange(ushort from, ushort to, ModbusRegistryType registryType, int interval)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(to, from);
            ArgumentOutOfRangeException.ThrowIfLessThan(interval, 0);

            if (PollingRanges.Any(r => r.From == from) == true)
            {
                throw new ArgumentException($"Register range with from register {from} is already registered");
            }

            PollingRanges.Add(new ModbusPollingRangeEntry(from, to, registryType, interval));
        }
    }
}

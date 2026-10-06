// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// ModbusRegister implementation for <see cref="IModbusRegister"/> and <see cref="IModbusDataface"/> interfaces.
    /// </summary>
    public class ModbusRegister : IModbusRegister, IModbusDataface
    {
        /// <summary>
        /// List of IModbusRegisterEntry registrations.
        /// </summary>
        private List<IModbusRegisterEntry> _registers = new List<IModbusRegisterEntry>();


        /// <summary>
        /// List of Modbus polling ranges.
        /// </summary>
        private List<ModbusPollingRangeEntry> _pollingRanges = new List<ModbusPollingRangeEntry>();


        /// <inheritdoc/>
        public string Name { get; init; }


        /// <inheritdoc/>
        public IReadOnlyList<IModbusRegisterEntry> Registers
        {
            get => _registers.AsReadOnly();
        }


        /// <inheritdoc/>
        public IReadOnlyList<ModbusPollingRangeEntry> PollingRanges
        {
            get => _pollingRanges.AsReadOnly();
        }


        /// <summary>
        /// Constructor of <see cref="ModbusRegister"/>.
        /// </summary>
        /// <param name="name">A name and good identifier of the data face.</param>
        public ModbusRegister(string name)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

            Name = name;
        }


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
        public void Register<TDevice, TProperty>(TDevice instance, string name, Action<TDevice, TProperty?> setter, int register,
            double scale, ModbusDataType dataType, int offset = 0)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(setter);
            ArgumentOutOfRangeException.ThrowIfLessThan(offset, 0);
            ArgumentOutOfRangeException.ThrowIfZero(scale);

            if (_registers.Any(r => r.Name.ToLower() == name.ToLower()) == true)
            {
                throw new ArgumentException($"Register with name {name} is already registered");
            }

            if (_registers.Any(r => r.Register == register) == true)
            {
                throw new ArgumentException($"Register with register {register} is already registered");
            }


            _registers.Add(new ModbusRegisterEntry<TDevice, TProperty?>(instance, name, setter, register, scale, dataType, offset));
        }


        /// <summary>
        /// <inheritdoc/>
        /// IModbusRegister implementation of RegisterRange <see cref="IModbusRegister"/>.
        /// </summary>
        public void RegisterRange(ushort from, ushort to, ModbusRegistryType registryType, int interval)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(to, from);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(interval);

            if (_pollingRanges.Any(r => r.From == from) == true)
            {
                throw new ArgumentException($"Register range with from register {from} is already registered");
            }

            _pollingRanges.Add(new ModbusPollingRangeEntry(from, to, registryType, interval));
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// Implementation of <see cref="IModbusRegisterEntry"/>.
    /// </summary>
    public class ModbusRegisterEntry<TDevice, TProperty> : IModbusRegisterEntry
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object Instance { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Name { get; init; }


        /// <summary>
        /// The action method that update the value defined in the registered action.
        /// </summary>
        public Action<TDevice, TProperty> Setter { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Register { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public double Scale { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ModbusDataType DataType { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Offset { get; init; }


        /// <summary>
        /// Constructor of <see cref="ModbusRegisterEntry"/>.
        /// </summary>
        /// <param name="instance">Instance the entry is member of.</param>
        /// <param name="name">Name of the entry.</param>
        /// <param name="setter">The action method that update the value defined in the registered action.</param>
        /// <param name="register">Modbus register number.</param>
        /// <param name="scale">Scale that is applied to the register value.</param>
        /// <param name="dataType">The register data type.</param>
        /// <param name="offset">The offset applied to the register entry.</param>
        public ModbusRegisterEntry(object instance, string name, Action<TDevice, TProperty> setter, int register, double scale, ModbusDataType dataType, int offset)
        {
            Instance = instance;
            Name = name;
            Setter = setter;
            Register = register;
            Scale = scale;
            DataType = dataType;
            Offset = offset;
        }


        /// <summary>
        /// Updates the value using the registered action.
        /// </summary>
        /// <param name="value">Value to update.</param>
        /// <exception cref="ArgumentException">Throws an exception when the instance doesn't match the registered type or property.</exception>
        public void Update(object value)
        {
            if (Instance is not TDevice typedDevice)
            {
                throw new ArgumentException($"{nameof(IModbusRegisterEntry)} must be of type {typeof(TDevice).Name}", nameof(Instance));
            }

            if (value is not TProperty typedValue)
            {
                throw new ArgumentException($"Value must be of type {typeof(TProperty).Name}", nameof(value));
            }

            Setter(typedDevice, typedValue);
        }
    }
}

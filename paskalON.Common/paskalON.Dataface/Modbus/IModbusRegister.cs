// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// Interface for registering Modbus registers.
    /// </summary>
    public interface IModbusRegister
    {
        /// <summary>
        /// Register a Modbus entry.
        /// </summary>
        /// <typeparam name="TDevice">The device type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="instance">Instance the property is member of.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="setter">Setter action.</param>
        /// <param name="register">Modbus register number.</param>
        /// <param name="scale">Scale that is applied to the register value.</param>
        /// <param name="dataType">The Modbus register data type.</param>
        /// <param name="offset">The offset applied to the Modbus register.</param>
        void Register<TDevice, TProperty>(TDevice instance, string name, Action<TDevice, TProperty?> setter, int register, double scale,
            ModbusDataType dataType, int offset = 0);


        /// <summary>
        /// Register a Modbus range.
        /// </summary>
        /// <param name="from">Modbus register from.</param>
        /// <param name="to">Modbus register to.</param>
        /// <param name="registryType">Modbus register type <see cref="ModbusRegistryType"/>.</param>
        /// <param name="interval">The interval based on a polling definition.</param>
        void RegisterRange(ushort from, ushort to, ModbusRegistryType registryType, int interval);
    }
}

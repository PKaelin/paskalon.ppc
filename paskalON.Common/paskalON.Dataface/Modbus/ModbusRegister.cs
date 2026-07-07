// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    public class ModbusRegister : IModbusRegister, IDataface
    {
        public List<IModbusRegisterEntry> Registers { get; } = new List<IModbusRegisterEntry>();

        public List<Tuple<ushort, ushort, ModbusPollingClass>> PollingRange { get; } = new List<Tuple<ushort, ushort, ModbusPollingClass>>();


        public void Register<TDevice, TProperty>(TDevice instance, string name, Action<TDevice, TProperty> setter, int register, double scale, WordOrder wordOrder = WordOrder.None, int offset = 0)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (Registers.Any(r => r.Name == name) == true)
            {
                throw new ArgumentException($"Register with name {name} is already registered");
            }

            if (Registers.Any(r => r.Register == register) == true)
            {
                throw new ArgumentException($"Register with register {register} is already registered");
            }


            Registers.Add(new ModbusRegisterEntry<TDevice, TProperty>(instance, name, setter, register, scale, wordOrder, offset));
        }

        public void Register<TDevice, TCom>(Action<TCom> com)
        {
            if (this is not TCom typedCom)
            {
                throw new ArgumentException($"Register type {typeof(TCom).Name} is not implemented by this class");
            }

            com.Invoke(typedCom);
        }

        public void RegisterRange(ushort from, ushort to, ModbusPollingClass pollingClass)
        {
            PollingRange.Add(new Tuple<ushort, ushort, ModbusPollingClass>(from, to, pollingClass));
        }

    }
}

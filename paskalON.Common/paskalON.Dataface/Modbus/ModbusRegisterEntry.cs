// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    public class ModbusRegisterEntry<TDevice, TProperty> : IModbusRegisterEntry
    {
        public object Instance { get; init; }

        public string Name { get; init; }

        public Action<TDevice, TProperty> Setter { get; init; }

        public int Register { get; init; }

        public double Scale { get; init; }

        public WordOrder WordOrder { get; init; }

        public int Offset { get; init; }

        public ModbusRegisterEntry(object instance, string name, Action<TDevice, TProperty> setter, int register, double scale, WordOrder wordOrder, int offset)
        {
            Instance = instance;
            Name = name;
            Setter = setter;
            Register = register;
            Scale = scale;
            WordOrder = wordOrder;
            Offset = offset;
        }


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

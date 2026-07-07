// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    public class C37RegisterEntry<TDevice, TProperty> : IC37RegisterEntry
    {
        public object Instance { get; init; }

        public string Name { get; init; }

        Action<TDevice, TProperty> Setter { get; init; }

        public C37RegisterEntry(object instance, string name, Action<TDevice, TProperty> setter)
        {
            Instance = instance;
            Name = name;
            Setter = setter;
        }

        public void Update(object value)
        {
            if (Instance is not TDevice typedDevice)
            {
                throw new ArgumentException($"{nameof(IC37RegisterEntry)} must be of type {typeof(TDevice).Name}", nameof(Instance));
            }

            if (value is not TProperty typedValue)
            {
                throw new ArgumentException($"Value must be of type {typeof(TProperty).Name}", nameof(value));
            }

            Setter(typedDevice, typedValue);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    public class C37Register : IC37Register, IDataface, IC37Dataface
    {
        public List<IC37RegisterEntry> Registers { get; } = new List<IC37RegisterEntry>();

        public void Register<TDevice, TProperty>(TDevice instance, string name, Action<TDevice, TProperty> setter)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (Registers.Any(r => r.Name == name) == true)
            {
                throw new ArgumentException($"Register with name {name} is already registered");
            }

            Registers.Add(new C37RegisterEntry<TDevice, TProperty>(instance, name, setter));
        }

        public void Register<TDevice, TCom>(Action<TCom> com)
        {
            if (com is not TCom typedCom)
            {
                throw new ArgumentException($"Register type {typeof(TCom).Name} is not implemented by this class");
            }

            com.Invoke(typedCom);
        }
    }
}

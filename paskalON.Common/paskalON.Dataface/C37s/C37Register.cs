// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// C37Register implementation for <see cref="IC37Register"/> and <see cref="IC37Dataface"/> interfaces.
    /// </summary>
    public class C37Register : IC37Register, IC37Dataface
    {
        /// <summary>
        /// IC37Dataface implementation of Registers <see cref="IC37Dataface"/>.
        /// </summary>
        public List<IC37RegisterEntry> Registers { get; } = new List<IC37RegisterEntry>();


        /// <summary>
        /// IDataface implementation of Register <see cref="IDataface"/>.
        /// </summary>
        public void Register<TDevice, TProperty>(TDevice instance, string name, C37SignalType signalType, Action<TDevice, TProperty> setter)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (Registers.Any(r => r.Name == name) == true)
            {
                throw new ArgumentException($"Register with name {name} is already registered");
            }

            Registers.Add(new C37RegisterEntry<TDevice, TProperty>(instance, name, signalType, setter));
        }


        /// <summary>
        /// IC37Register implementation of Register <see cref="IC37Register"/>.
        /// </summary>
        public void Register<TDevice, TCom>(Action<TCom> com)
        {
            if (this is not TCom typedCom)
            {
                // At this point it should be IC37Register
                throw new ArgumentException($"Register type {typeof(TCom).Name} is not implemented by this class");
            }

            com.Invoke(typedCom);
        }
    }
}

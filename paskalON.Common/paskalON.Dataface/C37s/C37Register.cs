// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// C37Register implementation for <see cref="IC37Register"/> and <see cref="IC37Dataface"/> interfaces.
    /// </summary>
    public class C37Register : IC37Register, IC37Dataface
    {
        /// <summary>
        /// List of IC37RegisterEntry registrations.
        /// </summary>
        private List<IC37RegisterEntry> _registers = new List<IC37RegisterEntry>();


        /// <inheritdoc/>
        public string Name { get; init; }


        /// <inheritdoc/>
        public IReadOnlyList<IC37RegisterEntry> Registers
        {
            get => _registers.AsReadOnly();
        }


        /// <summary>
        /// Constructor of <see cref="C37Register"/>.
        /// </summary>
        /// <param name="name">A name and good identifier of the data face.</param>
        public C37Register(string name)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

            Name = name;
        }


        /// <summary>
        /// IDataface implementation of Register <see cref="IDataface"/>.
        /// </summary>
        public void Register<TDevice, TProperty>(TDevice instance, string name, C37SignalType signalType, Action<TDevice, TProperty?> setter)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(setter);

            if (_registers.Any(r => r.Name.ToLower() == name.ToLower()) == true)
            {
                throw new ArgumentException($"Register with name {name} is already registered");
            }

            _registers.Add(new C37RegisterEntry<TDevice, TProperty>(instance, name, signalType, setter));
        }


        /// <summary>
        /// IC37Register implementation of Register <see cref="IC37Register"/>.
        /// </summary>
        public void Register<TDevice, TCom>(Action<TCom> com)
        {
            ArgumentNullException.ThrowIfNull(com);

            if (this is not TCom typedCom)
            {
                // At this point it should be IC37Register
                throw new ArgumentException($"Register type {typeof(TCom).Name} is not implemented by this class");
            }

            com.Invoke(typedCom);
        }
    }
}

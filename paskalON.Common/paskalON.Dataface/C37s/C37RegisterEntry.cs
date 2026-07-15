// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// Implementation of <see cref="C37RegisterEntry"/>.
    /// </summary>
    public class C37RegisterEntry<TDevice, TProperty> : IC37RegisterEntry
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
        /// <inheritdoc/>
        /// </summary>
        public C37SignalType SignalType { get; init; }


        /// <summary>
        /// Setter action.
        /// </summary>
        private Action<TDevice, TProperty> _setter { get; init; }


        /// <summary>
        /// Constructor of <see cref="C37RegisterEntry"/>.
        /// </summary>
        /// <param name="instance">The instance to update the value for.></param>
        /// <param name="name">The register entry name.</param>
        /// <param name="signalType">The C37 signal type.</param>
        /// <param name="setter">The setter action.</param>
        public C37RegisterEntry(object instance, string name, C37SignalType signalType, Action<TDevice, TProperty> setter)
        {
            Instance = instance;
            Name = name;
            SignalType = signalType;
            _setter = setter;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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

            _setter(typedDevice, typedValue);
        }
    }
}

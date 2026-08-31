// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// Implementation of <see cref="C37RegisterEntry"/>.
    /// </summary>
    public class C37RegisterEntry<TDevice, TProperty> : IC37RegisterEntry
    {
        /// <summary>
        /// Setter action.
        /// </summary>
        private Action<TDevice, TProperty?> _setter { get; init; }


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
        /// Constructor of <see cref="C37RegisterEntry"/>.
        /// </summary>
        /// <param name="instance">The instance to update the value for.></param>
        /// <param name="name">The register entry name.</param>
        /// <param name="signalType">The C37 signal type.</param>
        /// <param name="setter">The setter action.</param>
        public C37RegisterEntry(object instance, string name, C37SignalType signalType, Action<TDevice, TProperty?> setter)
        {
            _setter = setter;
            Instance = instance;
            Name = name;
            SignalType = signalType;
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

            Type targetType = typeof(TProperty);
            Type? underlyingType = Nullable.GetUnderlyingType(targetType);

            if (value == null)
            {
                if (underlyingType != null)
                {
                    _setter(typedDevice, default);
                    return;
                }

                throw new ArgumentNullException(nameof(value), "Value cannot be null for non-nullable properties.");
            }

            try
            {
                // Use the underlying primitive type if nullable, otherwise use targetType
                Type conversionType = underlyingType ?? targetType;
                TProperty typedValue = (TProperty)Convert.ChangeType(value, conversionType);
                _setter(typedDevice, typedValue);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Value {value} {value.GetType().Name} cannot be converted to target type {typeof(TProperty).Name} or updated to {Name}", ex);
            }
        }
    }
}

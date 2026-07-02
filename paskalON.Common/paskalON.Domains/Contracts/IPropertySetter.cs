namespace paskalON.Domains.Contracts
{
    public interface IPropertySetter<T>
    {
        /// <summary>
        /// Registers a property with the specified name, setter function.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to register.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="setter">A function to set the value of the property on an instance of T.</param>
        void Register<TProperty>(string name, Action<T, TProperty> setter);
    }
}
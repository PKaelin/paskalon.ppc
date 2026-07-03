namespace paskalON.Domains.Contracts
{
    public interface IValueGetterInterval<T>
    {
        /// <summary>
        /// Registers a property with the specified name, getter function and optional interval.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to register.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getter">A function to get the value of the property from an instance of T.</param>
        /// <param name="interval">The interval at which to publish the property if publishing is required.</param>
        /// /// <remarks>
        /// Syntax func: nameof(property/field), x => x.PropertyName/x.FieldName;
        /// </remarks>
        void Register<TProperty>(string name, Func<T, TProperty> getter, int interval = 1);
    }
}

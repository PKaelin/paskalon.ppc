namespace paskalON.Domains.Contracts
{
    /// <summary>
    /// Interface to completely loose couple the data source from the data consumer.
    /// </summary>
    /// <remarks>
    /// This is used for better naming of interfaces rather than definition.
    /// </remarks>
    public interface IDataface<T> : IPropertySetter<T>
    {
    }
}

namespace paskalON.Domains.Contracts
{
    /// <summary>
    /// Interface to completely loose couple the data source from the data consumer.
    /// </summary>
    public interface IDataface<T>
    {
        void Register<TCom>(Action<TCom> com);
    }
}

namespace paskalON.Domains.Telemetry
{
    public interface IMetricsPublisher<T>
    {
        void Register<TProperty>(string name, Func<T, TProperty> getter, int interval);

        void Publish(T instance, int interval);
    }
}

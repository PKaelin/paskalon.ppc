# Telemetry

TODO: Add more infos

### Metrics publisher
Any class can publish metrics as long as a metrics publisher interface gets injected to the class and an instance (MetricsPublisher) of that injected interface is kept within the class. E.g. 
```
public IMetricsPublisher MetricsPublisher { get; init; }
```

Any class than can register its metric entries via the interface. E.g. 
```
// Step 1 - Initialise metric:
IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>
{
    { "Name", _config.Name },
    { "DeviceId", _config.DeviceId }
};
MetricsPublisher.Initialize("MeasurementName", tags);

// Step 2 - Register one to many metric entries:
MetricsPublisher.Register<Device, double>(this, nameof(Power), MetricType.Gauge, x => x.Power, MetricsFactorClass1);
```

![Metrics Publisher Design](./Docs/Metrics%20Publisher%20Design.drawio.svg)

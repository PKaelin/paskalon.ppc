// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using paskalON.Telemetry.UnitTest.TestDoubles;
using System.Diagnostics.Metrics;

namespace paskalON.Telemetry.UnitTest
{
    [TestClass]
    public sealed class MetricsPublisherTest
    {
        private const string Measurement = "BatteryBank01";


        [TestMethod]
        public void InitializeCreatesMeterWithMeasurementNameTest()
        {
            using MetricsPublisher publisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);

            publisher.Initialize(Measurement, _tags);

            Assert.IsNotNull(publisher.Meter);
            Assert.AreEqual(Measurement, publisher.Meter.Name);
        }


        [TestMethod]
        public void InitializeTwiceThrowsAndKeepsFirstMeterTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            Meter? firstMeter = publisher.Meter;

            Action act = () => publisher.Initialize("BatteryBank02", _tags);

            Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(firstMeter, publisher.Meter);
        }


        [TestMethod]
        public void InitializeNullMeasurementThrowsTest()
        {
            using MetricsPublisher publisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);

            Action act = () => publisher.Initialize(null!, _tags);

            Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.IsNull(publisher.Meter);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void InitializeEmptyOrWhiteSpaceMeasurementThrowsTest(string measurement)
        {
            using MetricsPublisher publisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);

            Action act = () => publisher.Initialize(measurement, _tags);

            Assert.ThrowsExactly<ArgumentException>(act);
            Assert.IsNull(publisher.Meter);
        }


        [TestMethod]
        public void InitializeNullTagsThrowsTest()
        {
            using MetricsPublisher publisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);

            Action act = () => publisher.Initialize(Measurement, null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.IsNull(publisher.Meter);
        }


        [TestMethod]
        public void RegisterNotInitializedThrowsTest()
        {
            using MetricsPublisher publisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => publisher.Register<TestBatteryBank, long>(batteryBank, nameof(TestBatteryBank.ChargedEnergy), MetricType.Counter, x => x.ChargedEnergy);

            Assert.ThrowsExactly<ApplicationException>(act);
        }


        [TestMethod]
        public void RegisterNullInstanceThrowsTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();

            Action act = () => publisher.Register<TestBatteryBank, double>(null!, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void RegisterNullNameThrowsTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => publisher.Register<TestBatteryBank, double>(batteryBank, null!, MetricType.Gauge, x => x.StateOfCharge);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void RegisterEmptyOrWhiteSpaceNameThrowsTest(string name)
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => publisher.Register<TestBatteryBank, double>(batteryBank, name, MetricType.Gauge, x => x.StateOfCharge);

            Assert.ThrowsExactly<ArgumentException>(act);
        }


        [TestMethod]
        public void RegisterNullGetterThrowsTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        [DataRow("StateOfCharge")]
        [DataRow("stateofcharge")]
        [DataRow("STATEOFCHARGE")]
        public void RegisterDuplicateNameIgnoringCaseThrowsTest(string duplicateName)
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);

            Action act = () => publisher.Register<TestBatteryBank, double>(batteryBank, duplicateName, MetricType.Histogram, x => x.CellTemperature);

            Assert.ThrowsExactly<ArgumentException>(act);
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void RegisterIntervalNotPositiveThrowsAndDoesNotReserveNameTest(int interval)
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge, interval);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge, 1);
        }


        [TestMethod]
        public void RegisterUndefinedMetricTypeThrowsTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), (MetricType)99, x => x.StateOfCharge);

            Assert.ThrowsExactly<NotImplementedException>(act);
        }


        [TestMethod]
        public void RegisterUnsupportedValueTypeThrowsTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => publisher.Register<TestBatteryBank, bool>(batteryBank, nameof(TestBatteryBank.IsOnline), MetricType.Gauge, x => x.IsOnline);

            Assert.ThrowsExactly<InvalidOperationException>(act);
        }


        [TestMethod]
        public void PublishNotInitializedThrowsTest()
        {
            using MetricsPublisher publisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);

            Action act = () => publisher.Publish(1);

            Assert.ThrowsExactly<ApplicationException>(act);
        }


        [TestMethod]
        public void PublishNotInitializedAndDisabledDoesNotThrowTest()
        {
            using MetricsPublisher publisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);
            publisher.IsEnabled = false;

            publisher.Publish(1);

            Assert.IsNull(publisher.Meter);
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void PublishIntervalNotPositiveThrowsTest(int currentInterval)
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();

            Action act = () => publisher.Publish(currentInterval);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }


        [TestMethod]
        public void PublishWithoutRegistrationsDoesNotThrowTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();

            publisher.Publish(1);

            Assert.IsNotNull(publisher.Meter);
        }


        [TestMethod]
        public void PublishAllMetricTypesRecordsCurrentValuesTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, long>(batteryBank, nameof(TestBatteryBank.ChargedEnergy), MetricType.Counter, x => x.ChargedEnergy);
            publisher.Register<TestBatteryBank, int>(batteryBank, nameof(TestBatteryBank.StoredEnergyDelta), MetricType.UpDownCounter, x => x.StoredEnergyDelta);
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.CellTemperature), MetricType.Histogram, x => x.CellTemperature);
            using MetricCollector<long> counterCollector = new MetricCollector<long>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.ChargedEnergy)));
            using MetricCollector<int> upDownCollector = new MetricCollector<int>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StoredEnergyDelta)));
            using MetricCollector<double> gaugeCollector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));
            using MetricCollector<double> histogramCollector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.CellTemperature)));

            publisher.Publish(1);

            Assert.HasCount(1, counterCollector.GetMeasurementSnapshot());
            Assert.AreEqual(125000L, counterCollector.LastMeasurement!.Value);
            Assert.HasCount(1, upDownCollector.GetMeasurementSnapshot());
            Assert.AreEqual(-250, upDownCollector.LastMeasurement!.Value);
            Assert.HasCount(1, gaugeCollector.GetMeasurementSnapshot());
            Assert.AreEqual(67.5, gaugeCollector.LastMeasurement!.Value);
            Assert.HasCount(1, histogramCollector.GetMeasurementSnapshot());
            Assert.AreEqual(31.2, histogramCollector.LastMeasurement!.Value);
        }


        [TestMethod]
        public void PublishInstrumentNameIsLowerCaseMeasurementAndNameTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, double>(batteryBank, "StateOfCharge", MetricType.Gauge, x => x.StateOfCharge);
            using MetricCollector<double> collector = new MetricCollector<double>(publisher.Meter!, "batterybank01_stateofcharge");

            publisher.Publish(1);

            Assert.IsNotNull(collector.Instrument);
            Assert.AreEqual("batterybank01_stateofcharge", collector.Instrument.Name);
            Assert.HasCount(1, collector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public void PublishAttachesInitializeTagsToMeasurementsTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);
            using MetricCollector<double> collector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));

            publisher.Publish(1);

            IReadOnlyDictionary<string, object?> tags = collector.LastMeasurement!.Tags;
            Assert.HasCount(2, tags);
            Assert.AreEqual("BatteryBank01", tags["Name"]);
            Assert.AreEqual(1, tags["DeviceId"]);
        }


        [TestMethod]
        public void PublishNullValueIsSkippedTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            batteryBank.ActivePower = null;
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower), MetricType.Gauge, x => x.ActivePower);
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);
            using MetricCollector<double> activePowerCollector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.ActivePower)));
            using MetricCollector<double> stateOfChargeCollector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));

            publisher.Publish(1);

            Assert.HasCount(0, activePowerCollector.GetMeasurementSnapshot());
            Assert.HasCount(1, stateOfChargeCollector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public void PublishNullableValueBecomingAvailableIsRecordedTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            batteryBank.ActivePower = null;
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower), MetricType.Gauge, x => x.ActivePower);
            using MetricCollector<double> collector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.ActivePower)));
            publisher.Publish(1);
            batteryBank.ActivePower = -12500.0;

            publisher.Publish(2);

            Assert.HasCount(1, collector.GetMeasurementSnapshot());
            Assert.AreEqual(-12500.0, collector.LastMeasurement!.Value);
        }


        [TestMethod]
        public void PublishReadsLatestInstanceValueTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);
            using MetricCollector<double> collector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));
            publisher.Publish(1);
            batteryBank.StateOfCharge = 12.25;

            publisher.Publish(2);

            IReadOnlyList<CollectedMeasurement<double>> measurements = collector.GetMeasurementSnapshot();
            Assert.HasCount(2, measurements);
            Assert.AreEqual(67.5, measurements[0].Value);
            Assert.AreEqual(12.25, measurements[1].Value);
        }


        [TestMethod]
        [DataRow(1, 12)]
        [DataRow(2, 6)]
        [DataRow(3, 4)]
        [DataRow(5, 2)]
        [DataRow(12, 1)]
        [DataRow(13, 0)]
        public void PublishOnlyOnMultiplesOfIntervalTest(int interval, int expectedCount)
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge, interval);
            using MetricCollector<double> collector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));

            for (int currentInterval = 1; currentInterval <= 12; currentInterval++)
            {
                publisher.Publish(currentInterval);
            }

            Assert.HasCount(expectedCount, collector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public void PublishMixedIntervalsOnlyUpdatesDueEntriesTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, long>(batteryBank, nameof(TestBatteryBank.ChargedEnergy), MetricType.Counter, x => x.ChargedEnergy, 1);
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge, 2);
            publisher.Register<TestBatteryBank, int>(batteryBank, nameof(TestBatteryBank.StoredEnergyDelta), MetricType.UpDownCounter, x => x.StoredEnergyDelta, 3);
            using MetricCollector<long> counterCollector = new MetricCollector<long>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.ChargedEnergy)));
            using MetricCollector<double> gaugeCollector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));
            using MetricCollector<int> upDownCollector = new MetricCollector<int>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StoredEnergyDelta)));

            publisher.Publish(2);

            Assert.HasCount(1, counterCollector.GetMeasurementSnapshot());
            Assert.HasCount(1, gaugeCollector.GetMeasurementSnapshot());
            Assert.HasCount(0, upDownCollector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public void PublishCounterAddsValueOnEveryPublishTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            batteryBank.ChargedEnergy = 500;
            publisher.Register<TestBatteryBank, long>(batteryBank, nameof(TestBatteryBank.ChargedEnergy), MetricType.Counter, x => x.ChargedEnergy);
            using MetricCollector<long> collector = new MetricCollector<long>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.ChargedEnergy)));

            publisher.Publish(1);
            publisher.Publish(2);
            publisher.Publish(3);

            IReadOnlyList<CollectedMeasurement<long>> measurements = collector.GetMeasurementSnapshot();
            Assert.HasCount(3, measurements);
            Assert.AreEqual(1500L, measurements.Sum(measurement => measurement.Value));
        }


        [TestMethod]
        public void PublishDisabledRecordsNothingTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, long>(batteryBank, nameof(TestBatteryBank.ChargedEnergy), MetricType.Counter, x => x.ChargedEnergy);
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);
            using MetricCollector<long> counterCollector = new MetricCollector<long>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.ChargedEnergy)));
            using MetricCollector<double> gaugeCollector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));
            publisher.IsEnabled = false;

            publisher.Publish(1);

            Assert.HasCount(0, counterCollector.GetMeasurementSnapshot());
            Assert.HasCount(0, gaugeCollector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public void PublishReEnabledRecordsAgainTest()
        {
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);
            using MetricCollector<double> collector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));
            publisher.IsEnabled = false;
            publisher.Publish(1);

            publisher.IsEnabled = true;
            publisher.Publish(2);

            Assert.HasCount(1, collector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public void PublishIsEnabledByDefaultTest()
        {
            using MetricsPublisher publisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);

            bool isEnabled = publisher.IsEnabled;

            Assert.IsTrue(isEnabled);
        }


        [TestMethod]
        public void PublishGetterExceptionLogsWarningAndContinuesTest()
        {
            FakeLogger<MetricsPublisher> logger = new FakeLogger<MetricsPublisher>();
            using MetricsPublisher publisher = CreateInitializedPublisher(logger);
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, double>(batteryBank, "ActivePowerFaulty", MetricType.Gauge, x => throw new InvalidOperationException("Modbus read timeout"));
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);
            using MetricCollector<double> faultyCollector = new MetricCollector<double>(publisher.Meter!, InstrumentName("ActivePowerFaulty"));
            using MetricCollector<double> stateOfChargeCollector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));

            publisher.Publish(1);

            Assert.AreEqual(1, logger.Collector.Count);
            Assert.AreEqual(LogLevel.Warning, logger.LatestRecord.Level);
            Assert.Contains("ActivePowerFaulty", logger.LatestRecord.Message);
            Assert.Contains("Modbus read timeout", logger.LatestRecord.Message);
            Assert.HasCount(0, faultyCollector.GetMeasurementSnapshot());
            Assert.HasCount(1, stateOfChargeCollector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public async Task PublishConcurrentCallsRecordEveryMeasurementTest()
        {
            const int publishCount = 64;
            using MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, long>(batteryBank, nameof(TestBatteryBank.ChargedEnergy), MetricType.Counter, x => x.ChargedEnergy);
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);
            using MetricCollector<long> counterCollector = new MetricCollector<long>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.ChargedEnergy)));
            using MetricCollector<double> gaugeCollector = new MetricCollector<double>(publisher.Meter!, InstrumentName(nameof(TestBatteryBank.StateOfCharge)));

            IEnumerable<Task> tasks = Enumerable.Range(1, publishCount).Select(currentInterval => Task.Run(() => publisher.Publish(currentInterval)));
            await Task.WhenAll(tasks);

            Assert.HasCount(publishCount, counterCollector.GetMeasurementSnapshot());
            Assert.AreEqual(125000L * publishCount, counterCollector.GetMeasurementSnapshot().Sum(measurement => measurement.Value));
            Assert.HasCount(publishCount, gaugeCollector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public void DisposeReleasesMeterTest()
        {
            MetricsPublisher publisher = CreateInitializedPublisher();

            publisher.Dispose();

            Assert.IsNull(publisher.Meter);
        }


        [TestMethod]
        public void DisposeTwiceDoesNotThrowTest()
        {
            MetricsPublisher publisher = CreateInitializedPublisher();
            publisher.Dispose();
            publisher.Dispose();

            Assert.IsNull(publisher.Meter);
        }


        [TestMethod]
        public void DisposeNotInitializedDoesNotThrowTest()
        {
            MetricsPublisher publisher = new MetricsPublisher(NullLogger<MetricsPublisher>.Instance);

            publisher.Dispose();

            Assert.IsNull(publisher.Meter);
        }


        [TestMethod]
        public void PublishAfterDisposeThrowsTest()
        {
            MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);
            publisher.Dispose();

            Action act = () => publisher.Publish(1);

            Assert.ThrowsExactly<ApplicationException>(act);
        }


        [TestMethod]
        public void RegisterAfterDisposeThrowsTest()
        {
            MetricsPublisher publisher = CreateInitializedPublisher();
            TestBatteryBank batteryBank = CreateBatteryBank();
            publisher.Dispose();

            Action act = () => publisher.Register<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.StateOfCharge), MetricType.Gauge, x => x.StateOfCharge);

            Assert.ThrowsExactly<ApplicationException>(act);
        }


        private readonly IEnumerable<KeyValuePair<string, object?>> _tags = new Dictionary<string, object?>
        {
            { "Name", "BatteryBank01" },
            { "DeviceId", 1 },
        };


        private TestBatteryBank CreateBatteryBank()
        {
            TestBatteryBank batteryBank = new TestBatteryBank()
            {
                ChargedEnergy = 125000,
                StoredEnergyDelta = -250,
                StateOfCharge = 67.5,
                ActivePower = 48000.0,
                CellTemperature = 31.2,
                IsOnline = true,
            };

            return batteryBank;
        }


        private string InstrumentName(string name)
        {
            return $"{Measurement.ToLower()}_{name.ToLower()}";
        }


        private MetricsPublisher CreateInitializedPublisher(ILogger<MetricsPublisher>? logger = null)
        {
            MetricsPublisher publisher = new MetricsPublisher(logger ?? NullLogger<MetricsPublisher>.Instance);
            publisher.Initialize(Measurement, _tags);

            return publisher;
        }
    }
}

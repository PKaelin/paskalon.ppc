// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using paskalON.Telemetry.Entries;
using paskalON.Telemetry.UnitTest.TestDoubles;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace paskalON.Telemetry.UnitTest.Entries
{
    [TestClass]
    public sealed class MetricEntryTest
    {
        private const string InstrumentName = "batterybank01_activepower";

        [TestMethod]
        public void MetricEntryConstructorNullInstanceThrowsTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);

            Action act = () => new MetricEntry<TestBatteryBank, double>(null!, nameof(TestBatteryBank.ActivePower), gauge, MetricType.Gauge, x => x.ActivePower, CreateTagList(), 1);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void MetricEntryConstructorNullNameThrowsTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => new MetricEntry<TestBatteryBank, double>(batteryBank, null!, gauge, MetricType.Gauge, x => x.ActivePower, CreateTagList(), 1);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void MetricEntryConstructorEmptyOrWhiteSpaceNameThrowsTest(string name)
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => new MetricEntry<TestBatteryBank, double>(batteryBank, name, gauge, MetricType.Gauge, x => x.ActivePower, CreateTagList(), 1);

            Assert.ThrowsExactly<ArgumentException>(act);
        }


        [TestMethod]
        public void MetricEntryConstructorNullInstrumentThrowsTest()
        {
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower),
                null!, MetricType.Gauge, x => x.ActivePower, CreateTagList(), 1);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void MetricEntryConstructorNullGetterThrowsTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower),
                gauge, MetricType.Gauge, null!, CreateTagList(), 1);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void MetricEntryConstructorSetsPropertiesTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);
            TestBatteryBank batteryBank = CreateBatteryBank();
            TagList tagList = CreateTagList();

            MetricEntry<TestBatteryBank, double> entry = new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(batteryBank.ActivePower),
                gauge, MetricType.Gauge, x => x.ActivePower, tagList, 5);

            Assert.AreSame(batteryBank, entry.Instance);
            Assert.AreEqual(nameof(batteryBank.ActivePower), entry.Name);
            Assert.AreSame(gauge, entry.Instrument);
            Assert.AreEqual(MetricType.Gauge, entry.MetricType);
            Assert.AreEqual(5, entry.Interval);
            Assert.AreEqual(2, entry.TagList.Count);
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void MetricEntryConstructorIntervalNotPositiveThrowsTest(int interval)
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower),
                gauge, MetricType.Gauge, x => x.ActivePower, CreateTagList(), interval);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }


        [TestMethod]
        [DataRow(1)]
        [DataRow(int.MaxValue)]
        public void MetricEntryConstructorIntervalBoundaryAcceptedTest(int interval)
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);
            TestBatteryBank batteryBank = CreateBatteryBank();

            MetricEntry<TestBatteryBank, double> entry = new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower),
                gauge, MetricType.Gauge, x => x.ActivePower, CreateTagList(), interval);

            Assert.AreEqual(interval, entry.Interval);
        }


        [TestMethod]
        public void MetricEntryConstructorInstrumentValueTypeMismatchThrowsTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<float> gauge = meter.CreateGauge<float>(InstrumentName);
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower),
                gauge, MetricType.Gauge, x => x.ActivePower, CreateTagList(), 1);

            Assert.ThrowsExactly<NotImplementedException>(act);
        }


        [TestMethod]
        public void MetricEntryConstructorObservableInstrumentThrowsTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            ObservableGauge<double> observableGauge = meter.CreateObservableGauge<double>(InstrumentName, () => 0.0);
            TestBatteryBank batteryBank = CreateBatteryBank();

            Action act = () => new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower),
                observableGauge, MetricType.Gauge, x => x.ActivePower, CreateTagList(), 1);

            Assert.ThrowsExactly<NotImplementedException>(act);
        }


        [TestMethod]
        [DataRow(MetricType.Counter)]
        [DataRow(MetricType.UpDownCounter)]
        [DataRow(MetricType.Gauge)]
        [DataRow(MetricType.Histogram)]
        public void UpdateRecordsValueWithTagsForEachInstrumentTypeTest(MetricType metricType)
        {
            using Meter meter = new Meter("BatteryBank01");
            Instrument<double> instrument = CreateInstrument(meter, metricType);
            using MetricCollector<double> collector = new MetricCollector<double>(instrument);
            TestBatteryBank batteryBank = CreateBatteryBank();
            batteryBank.ActivePower = 48_000.0;
            MetricEntry<TestBatteryBank, double> entry = new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower),
                instrument, metricType, x => x.ActivePower, CreateTagList(), 1);

            entry.Update();

            Assert.HasCount(1, collector.GetMeasurementSnapshot());
            Assert.AreEqual(48_000.0, collector.LastMeasurement!.Value);
            Assert.AreEqual("BatteryBank01", collector.LastMeasurement.Tags["Name"]);
            Assert.AreEqual(1, collector.LastMeasurement.Tags["DeviceId"]);
        }


        [TestMethod]
        public void UpdateUpDownCounterRecordsNegativeValueTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            UpDownCounter<int> upDownCounter = meter.CreateUpDownCounter<int>("batterybank01_storedenergydeltawh");
            using MetricCollector<int> collector = new MetricCollector<int>(upDownCounter);
            TestBatteryBank batteryBank = CreateBatteryBank();
            MetricEntry<TestBatteryBank, int> entry = new MetricEntry<TestBatteryBank, int>(batteryBank, nameof(TestBatteryBank.StoredEnergyDelta),
                upDownCounter, MetricType.UpDownCounter, x => x.StoredEnergyDelta, CreateTagList(), 1);

            entry.Update();

            Assert.AreEqual(-250, collector.LastMeasurement!.Value);
        }


        [TestMethod]
        public void UpdateNullValueRecordsNothingTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);
            using MetricCollector<double> collector = new MetricCollector<double>(gauge);
            TestBatteryBank batteryBank = CreateBatteryBank();
            batteryBank.ActivePower = null;
            MetricEntry<TestBatteryBank, double> entry = new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower),
                gauge, MetricType.Gauge, x => x.ActivePower, CreateTagList(), 1);

            entry.Update();

            Assert.HasCount(0, collector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public void UpdateInstanceTypeMismatchThrowsTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);
            using MetricCollector<double> collector = new MetricCollector<double>(gauge);
            MetricEntry<TestBatteryBank, double> entry = new MetricEntry<TestBatteryBank, double>("BatteryBank01", nameof(TestBatteryBank.ActivePower),
                gauge, MetricType.Gauge, x => x.ActivePower, CreateTagList(), 1);

            Action act = () => entry.Update();

            Assert.ThrowsExactly<ArgumentException>(act);
            Assert.HasCount(0, collector.GetMeasurementSnapshot());
        }


        [TestMethod]
        public void UpdateGetterExceptionPropagatesTest()
        {
            using Meter meter = new Meter("BatteryBank01");
            Gauge<double> gauge = meter.CreateGauge<double>(InstrumentName);
            TestBatteryBank batteryBank = CreateBatteryBank();
            MetricEntry<TestBatteryBank, double> entry = new MetricEntry<TestBatteryBank, double>(batteryBank, nameof(TestBatteryBank.ActivePower),
                gauge, MetricType.Gauge, x => throw new InvalidOperationException("Modbus read timeout"), CreateTagList(), 1);

            Action act = () => entry.Update();

            Assert.ThrowsExactly<InvalidOperationException>(act);
        }


        private TagList CreateTagList()
        {
            TagList tagList = new TagList(
            [
                new KeyValuePair<string, object?>("Name", "BatteryBank01"),
                new KeyValuePair<string, object?>("DeviceId", 1),
            ]);

            return tagList;
        }


        private TestBatteryBank CreateBatteryBank()
        {
            TestBatteryBank batteryBank = new TestBatteryBank()
            {
                ChargedEnergy = 125000,
                StoredEnergyDelta = -250,
                StateOfCharge = 67.5,
                ActivePower = -12500.0,
                CellTemperature = 31.2,
                IsOnline = true,
            };

            return batteryBank;
        }


        private Instrument<double> CreateInstrument(Meter meter, MetricType metricType)
        {
            Instrument<double> instrument = metricType switch
            {
                MetricType.Counter => meter.CreateCounter<double>(InstrumentName),
                MetricType.UpDownCounter => meter.CreateUpDownCounter<double>(InstrumentName),
                MetricType.Gauge => meter.CreateGauge<double>(InstrumentName),
                MetricType.Histogram => meter.CreateHistogram<double>(InstrumentName),
                _ => throw new ArgumentOutOfRangeException(nameof(metricType)),
            };

            return instrument;
        }
    }
}

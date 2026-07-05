// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Diagnostics.Metrics.Testing;

namespace paskalON.Telemetry.UnitTest
{
    [TestClass]
    public sealed class MetricsPublisherTest
    {
        private IEnumerable<KeyValuePair<string, object?>> _tags = new Dictionary<string, object?>
        {
            { "Name", "MyHelper1" },
            { "DeviceId", 1 },
        };


        /// <summary>
        /// Helper class for the metrics publisher unit tests.
        /// </summary>
        class MetricsPublisherHelper
        {
            public int CounterValue { get; set; }
            public double GaugeValue { get; set; }
            public int UpDownValue { get; set; }

            public MetricsPublisherHelper(IMetricsPublisher<MetricsPublisherHelper> publisher)
            {
                // Usually you would register here but do the publishing in the unit tests.
            }
        }

        /// <summary>
        /// Helper class for the metrics publisher unit tests.
        /// </summary>
        class MetricsPublisherHelperNullable
        {
            public int? CounterValue { get; set; }
            public double? GaugeValue { get; set; }
            public int? UpDownValue { get; set; }

            public MetricsPublisherHelperNullable(IMetricsPublisher<MetricsPublisherHelperNullable> publisher)
            {
                // Usually you would register here but do the publishing in the unit tests.
            }
        }


        [TestMethod]
        public void PublishNothingInitializedTest()
        {
            MetricsPublisher<MetricsPublisherHelper> publisher = new MetricsPublisher<MetricsPublisherHelper>();
            MetricsPublisherHelper helper = new MetricsPublisherHelper(publisher) { CounterValue = 1, GaugeValue = 2, UpDownValue = 3 };
            Assert.ThrowsExactly<ApplicationException>(() => publisher.Publish(helper, 1));
        }


        [TestMethod]
        public void RegisterNothingInitializedTest()
        {
            MetricsPublisher<MetricsPublisherHelper> publisher = new MetricsPublisher<MetricsPublisherHelper>();
            MetricsPublisherHelper helper = new MetricsPublisherHelper(publisher) { CounterValue = 1, GaugeValue = 2, UpDownValue = 3 };
            Assert.ThrowsExactly<ApplicationException>(() => publisher.Register<int>("CounterValue", MetricType.Counter, x => x.CounterValue, 1));
        }


        [TestMethod]
        public void RegisterTwiceWithInitializedTest()
        {
            MetricsPublisher<MetricsPublisherHelper> publisher = new MetricsPublisher<MetricsPublisherHelper>();
            MetricsPublisherHelper helper = new MetricsPublisherHelper(publisher) { CounterValue = 1, GaugeValue = 2, UpDownValue = 3 };
            publisher.Initialize(nameof(MetricsPublisherHelper), _tags);
            publisher.Register<int>("CounterValue", MetricType.Counter, x => x.CounterValue, 1);
            Assert.ThrowsExactly<ArgumentException>(() => publisher.Register<int>("CounterValue", MetricType.Counter, x => x.CounterValue, 1));
        }


        [TestMethod]
        public void PublishAllIntervalOfOneTest()
        {
            MetricsPublisher<MetricsPublisherHelper> publisher = new MetricsPublisher<MetricsPublisherHelper>();
            MetricsPublisherHelper helper = new MetricsPublisherHelper(publisher) { CounterValue = 1, GaugeValue = 2, UpDownValue = 3 };

            publisher.Initialize(nameof(MetricsPublisherHelper), _tags);
            publisher.Register<int>("CounterValue", MetricType.Counter, x => x.CounterValue, 1);
            publisher.Register<double>("GaugeValue", MetricType.Gauge, x => x.GaugeValue, 1);
            publisher.Register<int>("UpDownValue", MetricType.UpDownCounter, x => x.UpDownValue, 1);

            MetricCollector<int> colCounter = new MetricCollector<int>(publisher.Meter!, "CounterValue");
            MetricCollector<double> colGauge = new MetricCollector<double>(publisher.Meter!, "GaugeValue");
            MetricCollector<int> colUpDown = new MetricCollector<int>(publisher.Meter!, "UpDownValue");

            publisher.Publish(helper, 1);

            Assert.AreEqual(1, colCounter.GetMeasurementSnapshot().Last().Value);
            Assert.AreEqual(2, colGauge.GetMeasurementSnapshot().Last().Value);
            Assert.AreEqual(3, colUpDown.GetMeasurementSnapshot().Last().Value);
        }


        [TestMethod]
        public void PublishAllIntervalOfOneNulleableTest()
        {
            MetricsPublisher<MetricsPublisherHelperNullable> publisher = new MetricsPublisher<MetricsPublisherHelperNullable>();
            MetricsPublisherHelperNullable helper = new MetricsPublisherHelperNullable(publisher) { CounterValue = 1, GaugeValue = null, UpDownValue = null };

            publisher.Initialize(nameof(MetricsPublisherHelperNullable), _tags);
            publisher.Register<int>("CounterValue", MetricType.Counter, x => x.CounterValue, 1);
            publisher.Register<double>("GaugeValue", MetricType.Gauge, x => x.GaugeValue, 1);
            publisher.Register<int>("UpDownValue", MetricType.UpDownCounter, x => x.UpDownValue, 1);

            MetricCollector<int> colCounter = new MetricCollector<int>(publisher.Meter!, "CounterValue");
            MetricCollector<double> colGauge = new MetricCollector<double>(publisher.Meter!, "GaugeValue");
            MetricCollector<int> colUpDown = new MetricCollector<int>(publisher.Meter!, "UpDownValue");

            publisher.Publish(helper, 1);

            Assert.AreEqual(1, colCounter.GetMeasurementSnapshot().Last().Value);
            Assert.IsNull(colGauge.GetMeasurementSnapshot().LastOrDefault());
            Assert.IsNull(colUpDown.GetMeasurementSnapshot().LastOrDefault());
        }



        [TestMethod]
        public void PublishAllIntervalOneTwoThreeTest()
        {
            MetricsPublisher<MetricsPublisherHelper> publisher = new MetricsPublisher<MetricsPublisherHelper>();
            MetricsPublisherHelper helper = new MetricsPublisherHelper(publisher) { CounterValue = 1, GaugeValue = 2, UpDownValue = 3 };

            publisher.Initialize(nameof(MetricsPublisherHelper), _tags);
            publisher.Register<int>("CounterValue", MetricType.Counter, x => x.CounterValue, 1);
            publisher.Register<double>("GaugeValue", MetricType.Gauge, x => x.GaugeValue, 2);
            publisher.Register<int>("UpDownValue", MetricType.UpDownCounter, x => x.UpDownValue, 3);

            MetricCollector<int> colCounter = new MetricCollector<int>(publisher.Meter!, "CounterValue");
            MetricCollector<double> colGauge = new MetricCollector<double>(publisher.Meter!, "GaugeValue");
            MetricCollector<int> colUpDown = new MetricCollector<int>(publisher.Meter!, "UpDownValue");

            publisher.Publish(helper, 1);
            Assert.AreEqual(1, colCounter.GetMeasurementSnapshot().Last().Value);
            Assert.IsNull(colGauge.GetMeasurementSnapshot().LastOrDefault());
            Assert.IsNull(colUpDown.GetMeasurementSnapshot().LastOrDefault());

            helper.CounterValue = 2;
            helper.GaugeValue = 3;
            helper.UpDownValue = 4;

            publisher.Publish(helper, 2);
            Assert.AreEqual(2, colCounter.GetMeasurementSnapshot().Last().Value);
            Assert.AreEqual(3, colGauge.GetMeasurementSnapshot().Last().Value);
            Assert.IsNull(colUpDown.GetMeasurementSnapshot().LastOrDefault());

            helper.CounterValue = 3;
            helper.GaugeValue = 4;
            helper.UpDownValue = 5;

            publisher.Publish(helper, 3);
            Assert.AreEqual(3, colCounter.GetMeasurementSnapshot().Last().Value);
            Assert.AreEqual(3, colGauge.GetMeasurementSnapshot().Last().Value);
            Assert.AreEqual(5, colUpDown.GetMeasurementSnapshot().Last().Value);
        }

    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Domain.Strategies;
using paskalON.Telemetry;
using System.Text.RegularExpressions;

namespace paskalON.PowerControls.Domain.UnitTest.Strategies
{
    [TestClass]
    public class WeightDistributionStrategyTest
    {
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private FakeLogger _logger = new FakeLogger();
        private WeightDistributionStrategy? _distribution;


        [TestInitialize]
        public void Initialize()
        {
            _distribution = new WeightDistributionStrategy(_logger);
        }



        [TestMethod]
        public void WeightDistributionEmptyUnitsTest()
        {
            ActivePower active = ActivePower.FromKilo(20);
            ReactivePower reactive = ReactivePower.FromKilo(10);

            _distribution!.Distribute(active, reactive, new List<DerUnitPowerControl>());

            IReadOnlyList<FakeLogRecord> logs = _logger.Collector.GetSnapshot();
            Regex regexActive = new Regex(".*active.*requested: 20.*achieved: 0.*", RegexOptions.IgnoreCase);
            Regex regexReactive = new Regex(".*reactive.*requested: 10.*achieved: 0.*", RegexOptions.IgnoreCase);

            Assert.IsNotNull(logs.Where(m => regexActive.IsMatch(m.Message)).FirstOrDefault());
            Assert.IsNotNull(logs.Where(m => regexReactive.IsMatch(m.Message)).FirstOrDefault());
        }
    }
}

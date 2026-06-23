using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.UnitTest.Ramps
{
    [TestClass]
    public class RampControllerTest
    {
        [TestMethod]
        public void RampControllerConstructorTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                new RampController(NullLogger<RampController>.Instance, TimeProvider.System, null);
                new RampController(NullLogger<RampController>.Instance, null, new RampTimeConfig { ChangedBy = "Test" });
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }


        [TestMethod]
        public void RampControllerRampTimeStartStopTest()
        {
            RampTimeConfig config = new RampTimeConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpTimeSeconds = 60,
                RampDownTimeSeconds = 30,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);
            Assert.AreEqual(0, ramp.CurrentValue);
            Assert.AreEqual(0, ramp.StartValue);
            Assert.AreEqual(0, ramp.TargetValue);
            Assert.AreEqual(DateTimeOffset.MinValue, ramp.StartDate);

            DateTimeOffset now = timeProvider.GetUtcNow();
            ramp.Start(0, 60);
            Assert.AreEqual(0, ramp.CurrentValue);
            Assert.AreEqual(0, ramp.StartValue);
            Assert.AreEqual(60, ramp.TargetValue);
            Assert.AreEqual(now, ramp.StartDate);

            ramp.Stop();
            Assert.AreEqual(DateTimeOffset.MinValue, ramp.StartDate);
        }


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(2, 2)]
        [DataRow(10, 10)]
        [DataRow(21, 21)]
        [DataRow(50, 50)]
        [DataRow(60, 60)]
        [DataRow(100, 60)]
        [DataRow(int.MaxValue, 60)]
        public void RampControllerRampTimeCalculateRampUpTest(int timeSpan, double expectedValue)
        {
            RampTimeConfig config = new RampTimeConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpTimeSeconds = 60,
                RampDownTimeSeconds = 30,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(0, 60);
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(timeSpan));
            Assert.AreEqual(expectedValue, ramp.CalculatePrecision());
        }


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(3, 0)]
        [DataRow(8, 0)]
        [DataRow(10, 0)]
        [DataRow(12, 2)]
        [DataRow(20, 10)]
        [DataRow(31, 21)]
        [DataRow(60, 50)]
        [DataRow(70, 60)]
        [DataRow(110, 60)]
        [DataRow(int.MaxValue, 60)]
        public void RampControllerRampTimeCalculateRampUpRampDelayTest(int timeSpan, double expectedValue)
        {
            RampTimeConfig config = new RampTimeConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 10,
                RampUpTimeSeconds = 60,
                RampDownTimeSeconds = 30,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(0, 60);
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(timeSpan));
            Assert.AreEqual(expectedValue, ramp.CalculatePrecision());
        }



        [TestMethod]
        [DataRow(0, 60)]
        [DataRow(2, 56)]
        [DataRow(10, 40)]
        [DataRow(21, 18)]
        [DataRow(30, 0)]
        [DataRow(100, 0)]
        [DataRow(int.MaxValue, 0)]
        public void RampControllerRampTimeCalculateRampDownTest(int timeSpan, double expectedValue)
        {
            RampTimeConfig config = new RampTimeConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpTimeSeconds = 60,
                RampDownTimeSeconds = 30,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(60, 0);
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(timeSpan));
            Assert.AreEqual(expectedValue, ramp.CalculatePrecision());
        }


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(10, 79.588)]
        [DataRow(15, 102.377)]
        [DataRow(40, 169.02)]
        [DataRow(50, 185.884)]
        [DataRow(60, 200)]
        [DataRow(int.MaxValue, 200)]
        public void RampControllerRampTimeConstantCalculateRampUpTest(int timeSpan, double expectedValue)
        {
            RampTimeConstantConfig config = new RampTimeConstantConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpTimeConstantSeconds = 60,
                RampDownTimeConstantSeconds = 30,
                TuningValue = 9
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(0, 200);
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(timeSpan));
            Assert.AreEqual(expectedValue, ramp.CalculatePrecision());
        }


        [TestMethod]
        [DataRow(0, 200)]
        [DataRow(3, 144.249)]
        [DataRow(10, 79.588)]
        [DataRow(15, 51.927)]
        [DataRow(20, 30.98)]
        [DataRow(25, 14.116)]
        [DataRow(60, 0)]
        [DataRow(int.MaxValue, 0)]
        public void RampControllerRampTimeConstantCalculateRampDownTest(int timeSpan, double expectedValue)
        {
            RampTimeConstantConfig config = new RampTimeConstantConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpTimeConstantSeconds = 60,
                RampDownTimeConstantSeconds = 30,
                TuningValue = 9
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(200, 0);
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(timeSpan));
            Assert.AreEqual(expectedValue, ramp.CalculatePrecision());
        }


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 10)]
        [DataRow(5, 50)]
        [DataRow(8, 80)]
        [DataRow(10, 100)]
        [DataRow(11, 110)]
        [DataRow(int.MaxValue, 110)]
        public void RampControllerRampRateCalculateRampUpTest(int timeSpan, double expectedValue)
        {
            RampRateConfig config = new RampRateConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpRatePerSecond = 10,
                RampDownRatePerSecond = 20,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(0, 110);
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(timeSpan));
            Assert.AreEqual(expectedValue, ramp.CalculatePrecision());
        }


        [TestMethod]
        [DataRow(0, 110)]
        [DataRow(1, 90)]
        [DataRow(3, 50)]
        [DataRow(5, 10)]
        [DataRow(6, 0)]
        [DataRow(int.MaxValue, 0)]
        public void RampControllerRampRateCalculateRampDownTest(int timeSpan, double expectedValue)
        {
            RampRateConfig config = new RampRateConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpRatePerSecond = 10,
                RampDownRatePerSecond = 20,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(110, 0);
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(timeSpan));
            Assert.AreEqual(expectedValue, ramp.CalculatePrecision());
        }


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 30)]
        [DataRow(2, 45)]
        [DataRow(3, 60)]
        [DataRow(int.MaxValue, 60)]
        public void RampControllerRampRatePercentageCalculateRampUpTest(int timeSpan, double expectedValue)
        {
            RampRatePercentageConfig config = new RampRatePercentageConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpRatePercentPerSecond = 50,
                RampDownRatePercentPerSecond = 50,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(0, 60);
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(timeSpan));
            Assert.AreEqual(expectedValue, ramp.CalculatePrecision());
        }


        [TestMethod]
        [DataRow(0, 60)]
        [DataRow(1, 30)]
        [DataRow(2, 15)]
        [DataRow(3, 7.5)]
        [DataRow(4, 3.75)]
        [DataRow(5, 1.875)]
        [DataRow(6, 0.938)]
        [DataRow(7, 0)]
        [DataRow(int.MaxValue, 0)]
        public void RampControllerRampRatePercentageCalculateRampDownTest(int timeSpan, double expectedValue)
        {
            RampRatePercentageConfig config = new RampRatePercentageConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampUpRatePercentPerSecond = 50,
                RampDownRatePercentPerSecond = 50,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            IRampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(60, 0);
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(timeSpan));
            Assert.AreEqual(expectedValue, ramp.CalculatePrecision());
        }

    }
}

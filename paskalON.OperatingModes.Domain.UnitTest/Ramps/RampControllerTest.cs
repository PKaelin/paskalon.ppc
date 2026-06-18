using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
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
                RampTimeoutSeconds = 0,
                RampUpTimeSeconds = 60,
                RampDownTimeSeconds = 30,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            RampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
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
        public void RampControllerRampTimeCalculateTest()
        {
            RampTimeConfig config = new RampTimeConfig
            {
                Id = 1,
                ChangedBy = "Test",
                RampTimeSeconds = 0,
                RampTimeoutSeconds = 0,
                RampUpTimeSeconds = 60,
                RampDownTimeSeconds = 30,
            };

            FakeTimeProvider timeProvider = new FakeTimeProvider();
            RampController ramp = new RampController(NullLogger<RampController>.Instance, timeProvider, config);
            Assert.IsNotNull(ramp);

            ramp.Start(0, 60);
            Assert.AreEqual(0, ramp.CurrentValue);
            Assert.AreEqual(0, ramp.StartValue);
            Assert.AreEqual(60, ramp.TargetValue);
            // Still on now
            Assert.AreEqual(0, ramp.Calculate());
            // Move forward seconds
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            Assert.AreEqual(1, ramp.Calculate());
            timeProvider.Advance(TimeSpan.FromSeconds(1));
            Assert.AreEqual(2, ramp.Calculate());
            timeProvider.Advance(TimeSpan.FromSeconds(8));
            Assert.AreEqual(10, ramp.Calculate());
            timeProvider.Advance(TimeSpan.FromSeconds(10));
            Assert.AreEqual(20, ramp.Calculate());
            timeProvider.Advance(TimeSpan.FromSeconds(30));
            Assert.AreEqual(50, ramp.Calculate());
            timeProvider.Advance(TimeSpan.FromSeconds(10));
            Assert.AreEqual(60, ramp.Calculate());
            timeProvider.Advance(TimeSpan.FromSeconds(10));
            Assert.AreEqual(60, ramp.Calculate());
        }


    }
}

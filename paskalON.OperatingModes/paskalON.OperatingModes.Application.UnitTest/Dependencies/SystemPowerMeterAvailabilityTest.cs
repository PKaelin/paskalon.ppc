// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;
using paskalON.Messaging.Health;
using paskalON.OperatingModes.Application.Dependencies;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Application.UnitTest.Dependencies
{
    /// <summary>
    /// Tests the availability of the system power meter with <see cref="SystemPowerMeterAvailability"/>.
    /// </summary>
    [TestClass]
    public class SystemPowerMeterAvailabilityTest
    {
        /// <summary>
        /// The system power meter loss timeout of the tests.
        /// </summary>
        private static readonly TimeSpan LossTimeout = TimeSpan.FromMilliseconds(2000);


        /// <summary>
        /// The resume settle time of the tests.
        /// </summary>
        private static readonly TimeSpan ResumeSettleTime = TimeSpan.FromMilliseconds(2000);


        /// <summary>
        /// Confirms a null DER throws.
        /// </summary>
        [TestMethod]
        public void IsAvailableNullDerThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => SystemPowerMeterAvailability.IsAvailable(null!));
        }


        /// <summary>
        /// Confirms a DER without system power meter is available, there is nothing to lose.
        /// </summary>
        [TestMethod]
        public void IsAvailableWithoutSystemPowerMeterTest()
        {
            DerDto der = new DerDto { Name = "Plant" };

            bool isAvailable = SystemPowerMeterAvailability.IsAvailable(der);

            Assert.IsTrue(isAvailable);
        }


        /// <summary>
        /// Confirms a system power meter is only available with data, without communication error and with active and reactive power.
        /// </summary>
        [TestMethod]
        [DataRow(true, false, true, true, true, DisplayName = "Valid data")]
        [DataRow(false, false, true, true, false, DisplayName = "No data received yet")]
        [DataRow(true, true, true, true, false, DisplayName = "Communication error")]
        [DataRow(true, false, false, true, false, DisplayName = "No active power")]
        [DataRow(true, false, true, false, false, DisplayName = "No reactive power")]
        public void IsAvailableSingleSystemPowerMeterTest(bool hasCore, bool communicationError, bool hasActivePower, bool hasReactivePower, bool expected)
        {
            PmSystemDto meter = CreateSystemPowerMeter(10, communicationError, hasActivePower, hasReactivePower, hasCore);
            DerDto der = new DerDto { Name = "Plant", SystemPowerMeters = new List<PmSystemDto> { meter } };

            bool isAvailable = SystemPowerMeterAvailability.IsAvailable(der);

            Assert.AreEqual(expected, isAvailable);
        }


        /// <summary>
        /// Confirms one valid system power meter is enough, e.g. a redundant meter takes over.
        /// </summary>
        [TestMethod]
        [DataRow(false, false, true)]
        [DataRow(true, false, true)]
        [DataRow(false, true, true)]
        [DataRow(true, true, false)]
        public void IsAvailableRedundantSystemPowerMetersTest(bool firstCommunicationError, bool secondCommunicationError, bool expected)
        {
            DerDto der = new DerDto
            {
                Name = "Plant",
                SystemPowerMeters = new List<PmSystemDto>
                {
                    CreateSystemPowerMeter(10, firstCommunicationError, true, true, true),
                    CreateSystemPowerMeter(11, secondCommunicationError, true, true, true)
                }
            };

            bool isAvailable = SystemPowerMeterAvailability.IsAvailable(der);

            Assert.AreEqual(expected, isAvailable);
        }


        /// <summary>
        /// Confirms the dependency gate holds after the loss timeout of the system power meter and resumes after the settle time.
        /// </summary>
        [TestMethod]
        public void IsAvailableConditionHoldsAndResumesDependencyGateTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            PmSystemDto meter = CreateSystemPowerMeter(10, false, true, true, true);
            DerDto der = new DerDto { Name = "Plant", SystemPowerMeters = new List<PmSystemDto> { meter } };
            DependencyHealthMonitor monitor = new DependencyHealthMonitor(NullLogger<DependencyHealthMonitor>.Instance, timeProvider);
            monitor.RegisterCondition(DependencyNames.SystemPowerMeter, () => SystemPowerMeterAvailability.IsAvailable(der), LossTimeout);
            DependencyGate gate = new DependencyGate(monitor, ResumeSettleTime, timeProvider);
            gate.GetHoldReason();
            timeProvider.Advance(ResumeSettleTime);

            string? runningReason = gate.GetHoldReason();
            meter.UpdateCore(new PmSystemCoreDto { DeviceId = 10, CommunicationError = true });
            timeProvider.Advance(LossTimeout - TimeSpan.FromMilliseconds(1));
            string? withinTimeoutReason = gate.GetHoldReason();
            timeProvider.Advance(TimeSpan.FromMilliseconds(1));
            string? lostReason = gate.GetHoldReason();
            meter.UpdateCore(CreateCore(10, false, true, true));
            string? settlingReason = gate.GetHoldReason();
            timeProvider.Advance(ResumeSettleTime);
            string? resumedReason = gate.GetHoldReason();

            Assert.IsNull(runningReason);
            Assert.IsNull(withinTimeoutReason);
            Assert.AreEqual($"Lost dependencies: {DependencyNames.SystemPowerMeter}.", lostReason);
            Assert.IsNotNull(settlingReason);
            Assert.IsNull(resumedReason);
        }


        /// <summary>
        /// Creates a system power meter.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <param name="communicationError">Whether the meter has a communication error.</param>
        /// <param name="hasActivePower">Whether the meter reports the active power.</param>
        /// <param name="hasReactivePower">Whether the meter reports the reactive power.</param>
        /// <param name="hasCore">Whether the meter has received core data.</param>
        /// <returns>The system power meter.</returns>
        private static PmSystemDto CreateSystemPowerMeter(int deviceId, bool communicationError, bool hasActivePower, bool hasReactivePower, bool hasCore)
        {
            PmSystemDto meter = new PmSystemDto(new PmSystemDefinitionDto { DeviceId = deviceId, Name = $"SystemPowerMeter{deviceId}" });

            if (hasCore)
            {
                meter.UpdateCore(CreateCore(deviceId, communicationError, hasActivePower, hasReactivePower));
            }

            return meter;
        }


        /// <summary>
        /// Creates the core data of a system power meter.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <param name="communicationError">Whether the meter has a communication error.</param>
        /// <param name="hasActivePower">Whether the meter reports the active power.</param>
        /// <param name="hasReactivePower">Whether the meter reports the reactive power.</param>
        /// <returns>The core data.</returns>
        private static PmSystemCoreDto CreateCore(int deviceId, bool communicationError, bool hasActivePower, bool hasReactivePower)
        {
            return new PmSystemCoreDto
            {
                DeviceId = deviceId,
                CommunicationError = communicationError,
                ActivePower = hasActivePower ? new ActivePower(6000000) : null,
                ReactivePower = hasReactivePower ? new ReactivePower(500000) : null
            };
        }
    }
}

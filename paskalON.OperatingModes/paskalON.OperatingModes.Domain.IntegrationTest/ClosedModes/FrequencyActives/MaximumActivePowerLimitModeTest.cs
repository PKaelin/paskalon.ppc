// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Integration tests of <see cref="MaximumActivePowerLimitMode"/> with a base mode, real ramp controllers and the system operating mode control.
    /// </summary>
    [TestClass]
    public class MaximumActivePowerLimitModeTest
    {
        /// <summary>
        /// Tolerance for power values in Watts.
        /// </summary>
        private const double Tolerance = 1;


        /// <summary>
        /// Fake time provider to advance the ramps.
        /// </summary>
        private FakeTimeProvider? _timeProvider;


        /// <summary>
        /// Measured active power at the POI.
        /// </summary>
        private ActivePower? _activePowerAtPoi;


        /// <summary>
        /// Available active power of the plant.
        /// </summary>
        private ActivePower? _availableActivePower;


        /// <summary>
        /// The limiting mode under test with a 6 MW cap.
        /// </summary>
        private MaximumActivePowerLimitMode? _limitMode;


        /// <summary>
        /// The operating mode stack.
        /// </summary>
        private OperatingModeStack? _stack;


        /// <summary>
        /// The system operating mode control.
        /// </summary>
        private SystemOperatingModeControl? _control;


        /// <summary>
        /// Creates a 10 MW plant with 8 MW available power, a maximum power point tracking mode and a 6 MW maximum active power limit mode,
        /// both ramping with 1 MW/s.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 15, 8, 0, 0, TimeSpan.Zero));
            _activePowerAtPoi = null;
            _availableActivePower = new ActivePower(8000000);
            SystemConfig systemConfig = new SystemConfig
            {
                ChangedBy = "Test",
                Type = PowerControlType.Bess,
                ReferenceFrequency = 50,
                NameplateMinimumActivePowerWatt = -10000000,
                NameplateMaximumActivePowerWatt = 10000000,
                NameplateMinimumReactivePowerVars = -5000000,
                NameplateMaximumReactivePowerVars = 5000000,
            };

            MaximumPowerPointTrackingModeConfig trackingConfig = new MaximumPowerPointTrackingModeConfig
            {
                ChangedBy = "Test",
                Name = "MaximumPowerPointTrackingMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 1000,
                DeadbandAvailable = 1000,
                RampConfig = new RampRateConfig { ChangedBy = "Test", RampUpRatePerSecond = 1000000, RampDownRatePerSecond = 1000000 },
            };

            MaximumActivePowerLimitModeConfig limitConfig = new MaximumActivePowerLimitModeConfig
            {
                ChangedBy = "Test",
                Name = "MaximumActivePowerLimitMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 1000,
                DeadbandAvailable = 1000,
                DeadbandError = 10000,
                MaximumActivePowerLimitWatts = 6000000,
                RampConfig = new RampRateConfig { ChangedBy = "Test", RampUpRatePerSecond = 1000000, RampDownRatePerSecond = 1000000 },
            };

            MaximumPowerPointTrackingModeMap trackingMap = new MaximumPowerPointTrackingModeMap { AvailableActivePower = () => _availableActivePower, AvailableReactivePower = () => null };
            MaximumActivePowerLimitModeMap limitMap = new MaximumActivePowerLimitModeMap
            {
                AvailableActivePower = () => _availableActivePower,
                AvailableReactivePower = () => null,
                ActivePowerAtPoi = () => _activePowerAtPoi
            };

            MaximumPowerPointTrackingMode trackingMode = new MaximumPowerPointTrackingMode(NullLogger.Instance, _timeProvider, new Mock<IMetricsPublisher>().Object, systemConfig,
                trackingConfig, trackingMap, new RampController(NullLogger.Instance, _timeProvider, trackingConfig.RampConfig));
            _limitMode = new MaximumActivePowerLimitMode(NullLogger.Instance, _timeProvider, new Mock<IMetricsPublisher>().Object, systemConfig, limitConfig, limitMap,
                new RampController(NullLogger.Instance, _timeProvider, limitConfig.RampConfig));
            _stack = new OperatingModeStack();
            _stack.Add(trackingMode, 1);
            _stack.Add(_limitMode, 2);

            SystemOperatingModeControlMap systemMap = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            _control = new SystemOperatingModeControl(NullLogger.Instance, systemConfig, _stack, new OperatingModeOverrides(), systemMap, new Mock<IMetricsPublisher>().Object);
        }


        /// <summary>
        /// Confirms the available power is limited to the cap and the binding limit is corrected with the measured power at the POI.
        /// </summary>
        [TestMethod]
        public void MaximumActivePowerLimitModeCorrectsLossesAtPoiTest()
        {
            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(10));
            _control!.Calculate(CancellationToken.None);
            double appliedAtCap = _control!.SetpointActivePowerActual.Watts;
            _activePowerAtPoi = new ActivePower(5800000);

            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(1));
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(6000000, appliedAtCap, Tolerance);
            Assert.IsTrue(_limitMode!.IsLimiting);
            Assert.AreEqual(200000, _limitMode!.ActiveFeedback!.ErrorAdjustment.Watts, Tolerance);
            Assert.AreEqual(6200000, _control!.SetpointActivePowerActual.Watts, Tolerance);
        }


        /// <summary>
        /// Confirms the correction is held once the POI reached the cap and reset when the limit is no longer binding.
        /// </summary>
        [TestMethod]
        public void MaximumActivePowerLimitModeHoldsAndResetsCorrectionTest()
        {
            _activePowerAtPoi = new ActivePower(5800000);
            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(10));
            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(1));
            _control!.Calculate(CancellationToken.None);
            _activePowerAtPoi = new ActivePower(6000000);
            _timeProvider!.Advance(TimeSpan.FromSeconds(1));
            _control!.Calculate(CancellationToken.None);
            double heldApplied = _control!.SetpointActivePowerActual.Watts;
            _availableActivePower = new ActivePower(5000000);

            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(5));
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(6200000, heldApplied, Tolerance);
            Assert.IsFalse(_limitMode!.IsLimiting);
            Assert.AreEqual(0, _limitMode!.ActiveFeedback!.ErrorAdjustment.Watts);
            Assert.AreEqual(5000000, _control!.SetpointActivePowerActual.Watts, Tolerance);
        }
    }
}

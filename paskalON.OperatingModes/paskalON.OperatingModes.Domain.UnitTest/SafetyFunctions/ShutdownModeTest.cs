// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.OperatingModes.Domain.SafetyFunctions;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.SafetyFunctions
{
    /// <summary>
    /// Tests <see cref="ShutdownMode"/>. Seeding with the applied targets is tested in the system operating mode control tests.
    /// </summary>
    [TestClass]
    public class ShutdownModeTest
    {
        /// <summary>
        /// The mocked active power ramp controller.
        /// </summary>
        private Mock<IRampController>? _rampActive;


        /// <summary>
        /// The mocked reactive power ramp controller (shallow copy of the active power ramp controller).
        /// </summary>
        private Mock<IRampController>? _rampReactive;


        /// <summary>
        /// The operating mode under test.
        /// </summary>
        private ShutdownMode? _mode;


        /// <summary>
        /// Creates the shutdown mode with mocked ramp controllers.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _rampActive = new Mock<IRampController>();
            _rampReactive = new Mock<IRampController>();
            _rampActive.Setup(controller => controller.ShallowCopy()).Returns(_rampReactive.Object);
            OperatingModeBaseMap map = new OperatingModeBaseMap { AvailableActivePower = () => null, AvailableReactivePower = () => null };
            _mode = new ShutdownMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(), CreateConfig(), map,
                _rampActive.Object);
        }


        /// <summary>
        /// Confirms the shutdown targets are zero by default.
        /// </summary>
        [TestMethod]
        public void ShutdownModeConfigDefaultTargetsZeroTest()
        {
            ShutdownModeConfig config = CreateConfig();

            Assert.AreEqual(0, config.ShutdownTargetActivePowerWatts);
            Assert.AreEqual(0, config.ShutdownTargetReactivePowerVars);
        }


        /// <summary>
        /// Confirms the mode is a plant-wide override mode with both power axes.
        /// </summary>
        [TestMethod]
        public void ShutdownModeConstructorTest()
        {
            IOverrideMode overrideMode = _mode!;

            Assert.AreEqual(OperatingModeClassification.Override, _mode!.Classification);
            Assert.IsNull(overrideMode.TargetDerUnit);
            Assert.IsNotNull(_mode!.ActiveControl);
            Assert.IsNotNull(_mode!.ReactiveControl);
            Assert.IsFalse(_mode!.IsShutdownCompleted);
        }


        /// <summary>
        /// Confirms the targets ramp from the start targets to zero without changing the signals of the stack.
        /// </summary>
        [TestMethod]
        public void ShutdownModeCalculateRampsTargetsToZeroTest()
        {
            _rampActive!.Setup(controller => controller.Calculate()).Returns(2500000);
            _rampReactive!.Setup(controller => controller.Calculate()).Returns(500000);
            ActivateWithStartTargets(5000000, 1000000);
            OperatingModeSignals signals = CreateSignals();

            _mode!.Calculate(signals);

            Assert.AreEqual(2500000, _mode!.ActiveControl!.Target.Watts);
            Assert.AreEqual(500000, _mode!.ReactiveControl!.Target.VoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            Assert.IsFalse(_mode!.IsShutdownCompleted);
            Assert.AreEqual(5000000, signals.FinalActivePower.Watts);
            _rampActive!.Verify(controller => controller.Start(5000000, 0), Times.Once);
            _rampReactive!.Verify(controller => controller.Start(1000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms the shutdown is completed once both power axes reached zero.
        /// </summary>
        [TestMethod]
        public void ShutdownModeCalculateCompletedAtZeroTest()
        {
            ActivateWithStartTargets(5000000, 1000000);

            _mode!.Calculate(CreateSignals());

            Assert.AreEqual(0, _mode!.ActiveControl!.Target.Watts);
            Assert.AreEqual(0, _mode!.ReactiveControl!.Target.VoltAmperesReactive);
            Assert.IsTrue(_mode!.IsShutdownCompleted);
        }


        /// <summary>
        /// Confirms the targets ramp to the configured shutdown targets and the shutdown is completed when they are reached.
        /// </summary>
        [TestMethod]
        public void ShutdownModeCalculateRampsToConfiguredTargetsTest()
        {
            ShutdownModeConfig config = CreateConfig();
            config.ShutdownTargetActivePowerWatts = 1000000;
            config.ShutdownTargetReactivePowerVars = -500000;
            _mode = new ShutdownMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(), config,
                OperatingModeTestData.CreateMap(), _rampActive!.Object);
            _rampActive!.Setup(controller => controller.Calculate()).Returns(1000000);
            _rampReactive!.Setup(controller => controller.Calculate()).Returns(-500000);
            ActivateWithStartTargets(5000000, 1000000);

            _mode!.Calculate(CreateSignals());

            Assert.AreEqual(1000000, _mode!.ActiveControl!.Target.Watts);
            Assert.AreEqual(-500000, _mode!.ReactiveControl!.Target.VoltAmperesReactive);
            Assert.IsTrue(_mode!.IsShutdownCompleted);
            _rampActive!.Verify(controller => controller.Start(5000000, 1000000), Times.Once);
            _rampReactive!.Verify(controller => controller.Start(1000000, -500000), Times.Once);
        }


        /// <summary>
        /// Confirms null signals are rejected.
        /// </summary>
        [TestMethod]
        public void ShutdownModeCalculateNullSignalsThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mode!.Calculate(null!));
        }


        /// <summary>
        /// Activates the mode and sets the start targets as the system operating mode control does for a plant-wide override.
        /// </summary>
        /// <param name="activePower">The start active power target in Watts.</param>
        /// <param name="reactivePower">The start reactive power target in Vars.</param>
        private void ActivateWithStartTargets(double activePower, double reactivePower)
        {
            _mode!.Activate();
            _mode!.ActiveControl!.SetStartTarget(new ActivePower(activePower));
            _mode!.ReactiveControl!.SetStartTarget(new ReactivePower(reactivePower));
        }


        /// <summary>
        /// Creates the shutdown mode configuration with the default shutdown targets.
        /// </summary>
        /// <returns>The shutdown mode configuration.</returns>
        private ShutdownModeConfig CreateConfig()
        {
            return new ShutdownModeConfig
            {
                ChangedBy = "Test",
                Name = "ShutdownMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };
        }


        /// <summary>
        /// Creates the signals of a stack calculating 5 MW and 1 MVar.
        /// </summary>
        /// <returns>The operating mode signals.</returns>
        private OperatingModeSignals CreateSignals()
        {
            return new OperatingModeSignals
            {
                SetpointActivePower = new ActivePower(5000000),
                SetpointReactivePower = new ReactivePower(1000000),
                BaseActivePower = new ActivePower(5000000),
                BaseReactivePower = new ReactivePower(1000000)
            };
        }
    }
}

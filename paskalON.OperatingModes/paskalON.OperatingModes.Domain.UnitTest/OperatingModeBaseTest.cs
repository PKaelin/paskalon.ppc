// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest
{
    /// <summary>
    /// Tests the axis composition, state and metrics of <see cref="OperatingModeBase"/>.
    /// </summary>
    [TestClass]
    public class OperatingModeBaseTest
    {
        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void OperatingModeBaseConstructorNullArgumentsThrowTest()
        {
            ILogger logger = NullLogger.Instance;
            TimeProvider timeProvider = TimeProvider.System;
            IMetricsPublisher publisher = new Mock<IMetricsPublisher>().Object;
            SystemConfig systemConfig = OperatingModeTestData.CreateSystemConfig();
            OperatingModeBaseConfig config = OperatingModeTestData.CreateConfig("Mode");
            OperatingModeBaseMap map = OperatingModeTestData.CreateMap();
            IRampController rampController = OperatingModeTestData.CreateRampController().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => new NullCheckTestMode(null!, timeProvider, publisher, systemConfig, config, map, rampController));
            Assert.ThrowsExactly<ArgumentNullException>(() => new NullCheckTestMode(logger, null!, publisher, systemConfig, config, map, rampController));
            Assert.ThrowsExactly<ArgumentNullException>(() => new NullCheckTestMode(logger, timeProvider, null!, systemConfig, config, map, rampController));
            Assert.ThrowsExactly<ArgumentNullException>(() => new NullCheckTestMode(logger, timeProvider, publisher, null!, config, map, rampController));
            Assert.ThrowsExactly<ArgumentNullException>(() => new NullCheckTestMode(logger, timeProvider, publisher, systemConfig, null!, map, rampController));
            Assert.ThrowsExactly<ArgumentNullException>(() => new NullCheckTestMode(logger, timeProvider, publisher, systemConfig, config, null!, rampController));
            Assert.ThrowsExactly<ArgumentNullException>(() => new NullCheckTestMode(logger, timeProvider, publisher, systemConfig, config, map, null!));
        }


        /// <summary>
        /// Confirms the power controls are created from the classification.
        /// </summary>
        /// <param name="modeKind">The kind of test operating mode.</param>
        /// <param name="expectedClassification">The expected classification.</param>
        /// <param name="expectedActive">Whether an active power control is expected.</param>
        /// <param name="expectedReactive">Whether a reactive power control is expected.</param>
        [TestMethod]
        [DataRow("Plain", OperatingModeClassification.None, false, false)]
        [DataRow("BaseActive", OperatingModeClassification.BaseActive, true, false)]
        [DataRow("BaseReactive", OperatingModeClassification.BaseReactive, false, true)]
        [DataRow("Override", OperatingModeClassification.Override, true, true)]
        public void OperatingModeBaseConstructorCreatesPowerControlsTest(string modeKind, OperatingModeClassification expectedClassification, bool expectedActive, bool expectedReactive)
        {
            TestOperatingModeBase operatingMode = CreateMode(modeKind, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateRampController().Object);

            Assert.AreEqual("Mode", operatingMode.Name);
            Assert.AreEqual(expectedClassification, operatingMode.Classification);
            Assert.AreEqual(expectedActive, operatingMode.ActiveControl is not null);
            Assert.AreEqual(expectedReactive, operatingMode.ReactiveControl is not null);
            Assert.AreEqual(OperatingModeState.Disabled, operatingMode.State);
            Assert.IsFalse(operatingMode.IsActivated);
            Assert.AreEqual(DateTimeOffset.MinValue, operatingMode.LastActivated);
        }


        /// <summary>
        /// Confirms the ramp controller is only copied when both power axes exist.
        /// </summary>
        /// <param name="modeKind">The kind of test operating mode.</param>
        /// <param name="expectedShallowCopies">The expected number of ramp controller copies.</param>
        [TestMethod]
        [DataRow("BaseActive", 0)]
        [DataRow("BaseReactive", 0)]
        [DataRow("Override", 1)]
        public void OperatingModeBaseConstructorRampControllerPerAxisTest(string modeKind, int expectedShallowCopies)
        {
            Mock<IRampController> rampController = OperatingModeTestData.CreateRampController();

            TestOperatingModeBase operatingMode = CreateMode(modeKind, new Mock<IMetricsPublisher>().Object, rampController.Object);

            IRampController? firstRampController = operatingMode.ActiveControl?.RampController ?? operatingMode.ReactiveControl?.RampController;
            Assert.AreSame(rampController.Object, firstRampController);
            rampController.Verify(controller => controller.ShallowCopy(), Times.Exactly(expectedShallowCopies));

            if (operatingMode.ActiveControl is not null && operatingMode.ReactiveControl is not null)
            {
                Assert.AreNotSame(operatingMode.ActiveControl.RampController, operatingMode.ReactiveControl.RampController);
            }
        }


        /// <summary>
        /// Confirms activating sets the enabling state and the time stamp.
        /// </summary>
        [TestMethod]
        public void OperatingModeBaseActivateSetsEnablingAndLastActivatedTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 12, 8, 0, 0, TimeSpan.Zero));
            TestBaseActiveMode operatingMode = new TestBaseActiveMode("Mode", timeProvider: timeProvider);

            operatingMode.Activate();

            Assert.IsTrue(operatingMode.IsActivated);
            Assert.AreEqual(timeProvider.GetUtcNow(), operatingMode.LastActivated);
            Assert.AreEqual(OperatingModeState.Enabling, operatingMode.State);
            Assert.AreEqual(OperatingModeState.Enabling, operatingMode.ActiveControl!.State);
        }


        /// <summary>
        /// Confirms a stack mode establishes its target immediately and an override mode ramps from its current target.
        /// </summary>
        /// <param name="modeKind">The kind of test operating mode.</param>
        /// <param name="expectedRampStarts">The expected number of ramp starts.</param>
        [TestMethod]
        [DataRow("BaseActive", 0)]
        [DataRow("Override", 1)]
        public void OperatingModeBaseActivateStartPolicyTest(string modeKind, int expectedRampStarts)
        {
            Mock<IRampController> rampController = OperatingModeTestData.CreateRampController();
            TestOperatingModeBase operatingMode = CreateMode(modeKind, new Mock<IMetricsPublisher>().Object, rampController.Object);
            // TestBaseActiveMode -> IBaseActiveMode -> IStackableMode = PowerControlStart.AtTargetSetpoint
            operatingMode.Activate();
            operatingMode.ActiveControl!.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Exactly(expectedRampStarts));
        }


        /// <summary>
        /// Confirms deactivating disables all power axes immediately.
        /// </summary>
        [TestMethod]
        public void OperatingModeBaseDeactivateDisablesImmediatelyTest()
        {
            TestOverrideMode operatingMode = new TestOverrideMode("Mode", "Unit1");
            operatingMode.Activate();

            // System operating mode control transitions the applied target
            operatingMode.Deactivate();

            Assert.IsFalse(operatingMode.IsActivated);
            Assert.AreEqual(OperatingModeState.Disabled, operatingMode.State);
            Assert.AreEqual(OperatingModeState.Disabled, operatingMode.ActiveControl!.State);
            Assert.AreEqual(OperatingModeState.Disabled, operatingMode.ReactiveControl!.State);
        }


        /// <summary>
        /// Confirms the combination of the power axis states to the state of the operating mode.
        /// </summary>
        /// <param name="active">The active power axis state or null if the axis does not exist.</param>
        /// <param name="reactive">The reactive power axis state or null if the axis does not exist.</param>
        /// <param name="expected">The expected state of the operating mode.</param>
        [TestMethod]
        [DataRow(null, null, OperatingModeState.Disabled)]
        [DataRow(OperatingModeState.Enabled, null, OperatingModeState.Enabled)]
        [DataRow(null, OperatingModeState.RampingToEnabled, OperatingModeState.RampingToEnabled)]
        [DataRow(OperatingModeState.Enabled, OperatingModeState.Enabled, OperatingModeState.Enabled)]
        [DataRow(OperatingModeState.Enabled, OperatingModeState.Disabled, OperatingModeState.Enabled)]
        [DataRow(OperatingModeState.RampingToEnabled, OperatingModeState.Enabled, OperatingModeState.RampingToEnabled)]
        [DataRow(OperatingModeState.Enabled, OperatingModeState.RampingToEnabled, OperatingModeState.RampingToEnabled)]
        [DataRow(OperatingModeState.Enabling, OperatingModeState.Enabled, OperatingModeState.Enabling)]
        [DataRow(OperatingModeState.RampingToEnabled, OperatingModeState.Enabling, OperatingModeState.Enabling)]
        [DataRow(OperatingModeState.RampingToDisabled, OperatingModeState.Disabled, OperatingModeState.RampingToDisabled)]
        [DataRow(OperatingModeState.RampingToDisabled, OperatingModeState.RampingToDisabled, OperatingModeState.RampingToDisabled)]
        [DataRow(OperatingModeState.Disabled, OperatingModeState.Disabled, OperatingModeState.Disabled)]
        public void OperatingModeBaseCombineStatesTest(OperatingModeState? active, OperatingModeState? reactive, OperatingModeState expected)
        {
            OperatingModeState state = OperatingModeBase.CombineStates(active, reactive);

            Assert.AreEqual(expected, state);
        }



        /// <summary>
        /// Confirms only the metrics of the existing power axis and the contribution are registered.
        /// </summary>
        [TestMethod]
        public void OperatingModeBaseRegisterMetricsOnlyExistingAxisTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            TestBaseActiveMode operatingMode = new TestBaseActiveMode("Mode", publisher.Object);

            operatingMode.RegisterMetricsForTest();

            publisher.Verify(p => p.Initialize("OperatingMode", It.Is<IEnumerable<KeyValuePair<string, object?>>>(tags =>
                tags.Any(tag => tag.Key == "Name" && (string?)tag.Value == "Mode") &&
                tags.Any(tag => tag.Key == "Classification" && (string?)tag.Value == "BaseActive"))), Times.Once);
            VerifyAxisMetric(publisher, "ActivePowerTarget", Times.Once());
            VerifyAxisMetric(publisher, "ActivePowerSetpoint", Times.Once());
            VerifyAxisMetric(publisher, "ActivePowerTargetSetpoint", Times.Once());
            VerifyAxisMetric(publisher, "ActivePowerAvailable", Times.Once());
            VerifyModeMetric(publisher, "ActivePowerContribution", Times.Once());
            VerifyModeMetric(publisher, "ReactivePowerContribution", Times.Never());
            publisher.Verify(p => p.Register(It.IsAny<PowerControl<ReactivePower>>(), It.IsAny<string>(), It.IsAny<MetricType>(),
                It.IsAny<Func<PowerControl<ReactivePower>, double?>>(), It.IsAny<int>()), Times.Never);
        }


        /// <summary>
        /// Confirms override modes are tagged with their DER unit and have no contribution metrics.
        /// </summary>
        [TestMethod]
        public void OperatingModeBaseRegisterMetricsOverrideTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            TestOverrideMode operatingMode = new TestOverrideMode("Mode", "Unit1", publisher.Object);

            operatingMode.RegisterMetricsForTest();

            publisher.Verify(p => p.Initialize("OperatingMode", It.Is<IEnumerable<KeyValuePair<string, object?>>>(tags =>
                tags.Any(tag => tag.Key == "DerUnit" && (string?)tag.Value == "Unit1"))), Times.Once);
            VerifyAxisMetric(publisher, "ActivePowerTarget", Times.Once());
            VerifyModeMetric(publisher, "ActivePowerContribution", Times.Never());
            VerifyModeMetric(publisher, "ReactivePowerContribution", Times.Never());
        }


        /// <summary>
        /// Confirms limiting modes register the is limiting metric.
        /// </summary>
        [TestMethod]
        public void OperatingModeBaseRegisterMetricsLimitingTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            TestLimitingActiveMode operatingMode = new TestLimitingActiveMode("Mode", publisher.Object);

            operatingMode.RegisterMetricsForTest();

            publisher.Verify(p => p.Register(It.IsAny<OperatingModeBase>(), "ActivePowerIsLimiting", MetricType.Gauge,
                It.IsAny<Func<OperatingModeBase, int?>>(), It.IsAny<int>()), Times.Once);
        }


        /// <summary>
        /// Creates a test operating mode of the specified kind.
        /// </summary>
        /// <param name="modeKind">The kind of test operating mode.</param>
        /// <param name="publisher">The metrics publisher.</param>
        /// <param name="rampController">The ramp controller.</param>
        /// <returns>The test operating mode.</returns>
        private TestOperatingModeBase CreateMode(string modeKind, IMetricsPublisher publisher, IRampController rampController)
        {
            return modeKind switch
            {
                "BaseActive" => new TestBaseActiveMode("Mode", publisher, rampController),
                "BaseReactive" => new TestBaseReactiveMode("Mode", publisher, rampController),
                "Override" => new TestOverrideMode("Mode", "Unit1", publisher, rampController),
                _ => new TestPlainMode("Mode", publisher, rampController),
            };
        }


        /// <summary>
        /// Verifies the registration of an active power axis metric.
        /// </summary>
        /// <param name="publisher">The mocked publisher.</param>
        /// <param name="name">The metric name.</param>
        /// <param name="times">The expected number of registrations.</param>
        private void VerifyAxisMetric(Mock<IMetricsPublisher> publisher, string name, Times times)
        {
            publisher.Verify(p => p.Register(It.IsAny<PowerControl<ActivePower>>(), name, MetricType.Gauge,
                It.IsAny<Func<PowerControl<ActivePower>, double?>>(), It.IsAny<int>()), times);
        }


        /// <summary>
        /// Verifies the registration of an operating mode metric.
        /// </summary>
        /// <param name="publisher">The mocked publisher.</param>
        /// <param name="name">The metric name.</param>
        /// <param name="times">The expected number of registrations.</param>
        private void VerifyModeMetric(Mock<IMetricsPublisher> publisher, string name, Times times)
        {
            publisher.Verify(p => p.Register(It.IsAny<OperatingModeBase>(), name, MetricType.Gauge,
                It.IsAny<Func<OperatingModeBase, double?>>(), It.IsAny<int>()), times);
        }


        /// <summary>
        /// Test operating mode exposing the full base constructor.
        /// </summary>
        private sealed class NullCheckTestMode : OperatingModeBase
        {
            /// <summary>
            /// Constructor of <see cref="NullCheckTestMode"/>.
            /// </summary>
            /// <param name="logger">The logger.</param>
            /// <param name="timeProvider">The time provider.</param>
            /// <param name="publisher">The metrics publisher.</param>
            /// <param name="systemConfig">The system configuration.</param>
            /// <param name="config">The operating mode configuration.</param>
            /// <param name="map">The operating mode map.</param>
            /// <param name="rampController">The ramp controller.</param>
            public NullCheckTestMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, OperatingModeBaseConfig config,
                OperatingModeBaseMap map, IRampController rampController)
                : base(logger, timeProvider, publisher, systemConfig, config, map, rampController)
            {
            }


            /// <inheritdoc/>
            public override OperatingModeSignals Calculate(OperatingModeSignals signals)
            {
                return signals;
            }
        }
    }
}

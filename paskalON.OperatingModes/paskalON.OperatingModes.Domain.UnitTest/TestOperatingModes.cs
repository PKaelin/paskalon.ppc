// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest
{
    /// <summary>
    /// Configurable operating mode used to test the base class, the stack and the system operating mode control.
    /// </summary>
    internal abstract class TestOperatingModeBase : OperatingModeBase
    {
        /// <summary>
        /// Gets or sets the calculation that is executed by <see cref="Calculate"/>.
        /// </summary>
        public Func<OperatingModeSignals, OperatingModeSignals> CalculateAction { get; set; } = signals => signals;


        /// <summary>
        /// Constructor of <see cref="TestOperatingModeBase"/>.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="publisher">The metrics publisher or null for a mock.</param>
        /// <param name="rampController">The ramp controller or null for a mock.</param>
        /// <param name="timeProvider">The time provider or null for the system time provider.</param>
        protected TestOperatingModeBase(string name, IMetricsPublisher? publisher, IRampController? rampController, TimeProvider? timeProvider)
            : base(NullLogger.Instance, timeProvider ?? TimeProvider.System, publisher ?? new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                  OperatingModeTestData.CreateConfig(name), OperatingModeTestData.CreateMap(), rampController ?? OperatingModeTestData.CreateRampController().Object)
        {
        }


        /// <inheritdoc/>
        public override OperatingModeSignals Calculate(OperatingModeSignals signals)
        {
            return CalculateAction.Invoke(signals);
        }


        /// <summary>
        /// Registers the metrics of the operating mode.
        /// </summary>
        public void RegisterMetricsForTest()
        {
            RegisterMetrics();
        }
    }


    /// <summary>
    /// Test operating mode without classification that is not stackable.
    /// </summary>
    internal sealed class TestPlainMode : TestOperatingModeBase
    {
        /// <summary>
        /// Constructor of <see cref="TestPlainMode"/>.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="publisher">The metrics publisher or null for a mock.</param>
        /// <param name="rampController">The ramp controller or null for a mock.</param>
        public TestPlainMode(string name, IMetricsPublisher? publisher = null, IRampController? rampController = null)
            : base(name, publisher, rampController, null)
        {
        }
    }


    /// <summary>
    /// Test operating mode that is stackable without classification.
    /// </summary>
    internal sealed class TestStackableMode : TestOperatingModeBase, IStackableMode
    {
        /// <summary>
        /// Constructor of <see cref="TestStackableMode"/>.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        public TestStackableMode(string name)
            : base(name, null, null, null)
        {
        }
    }


    /// <summary>
    /// Test base active operating mode.
    /// </summary>
    internal sealed class TestBaseActiveMode : TestOperatingModeBase, IBaseActiveMode
    {
        /// <summary>
        /// Constructor of <see cref="TestBaseActiveMode"/>.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="publisher">The metrics publisher or null for a mock.</param>
        /// <param name="rampController">The ramp controller or null for a mock.</param>
        /// <param name="timeProvider">The time provider or null for the system time provider.</param>
        public TestBaseActiveMode(string name, IMetricsPublisher? publisher = null, IRampController? rampController = null, TimeProvider? timeProvider = null)
            : base(name, publisher, rampController, timeProvider)
        {
        }
    }


    /// <summary>
    /// Test base reactive operating mode.
    /// </summary>
    internal sealed class TestBaseReactiveMode : TestOperatingModeBase, IBaseReactiveMode
    {
        /// <summary>
        /// Constructor of <see cref="TestBaseReactiveMode"/>.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="publisher">The metrics publisher or null for a mock.</param>
        /// <param name="rampController">The ramp controller or null for a mock.</param>
        public TestBaseReactiveMode(string name, IMetricsPublisher? publisher = null, IRampController? rampController = null)
            : base(name, publisher, rampController, null)
        {
        }
    }


    /// <summary>
    /// Test additive active operating mode.
    /// </summary>
    internal sealed class TestAdditiveActiveMode : TestOperatingModeBase, IAdditiveActiveMode
    {
        /// <summary>
        /// Constructor of <see cref="TestAdditiveActiveMode"/>.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="rampController">The ramp controller or null for a mock.</param>
        public TestAdditiveActiveMode(string name, IRampController? rampController = null)
            : base(name, null, rampController, null)
        {
        }
    }


    /// <summary>
    /// Test limiting active operating mode.
    /// </summary>
    internal sealed class TestLimitingActiveMode : TestOperatingModeBase, ILimitingActiveMode
    {
        /// <summary>
        /// Constructor of <see cref="TestLimitingActiveMode"/>.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="publisher">The metrics publisher or null for a mock.</param>
        /// <param name="rampController">The ramp controller or null for a mock.</param>
        public TestLimitingActiveMode(string name, IMetricsPublisher? publisher = null, IRampController? rampController = null)
            : base(name, publisher, rampController, null)
        {
        }
    }


    /// <summary>
    /// Test override operating mode for a DER unit.
    /// </summary>
    internal sealed class TestOverrideMode : TestOperatingModeBase, IOverrideMode
    {
        /// <summary>
        /// Name of the overridden DER unit.
        /// </summary>
        private readonly string? _targetDerUnit;


        /// <inheritdoc/>
        public string? TargetDerUnit { get => _targetDerUnit; }


        /// <summary>
        /// Constructor of <see cref="TestOverrideMode"/>.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="targetDerUnit">The name of the overridden DER unit.</param>
        /// <param name="publisher">The metrics publisher or null for a mock.</param>
        /// <param name="rampController">The ramp controller or null for a mock.</param>
        public TestOverrideMode(string name, string? targetDerUnit, IMetricsPublisher? publisher = null, IRampController? rampController = null)
            : base(name, publisher, rampController, null)
        {
            _targetDerUnit = targetDerUnit;
        }
    }


    /// <summary>
    /// Test base active/reactive operating mode.
    /// </summary>
    internal sealed class TestBaseActiveReactiveMode : TestOperatingModeBase, IBaseActiveMode, IBaseReactiveMode
    {
        /// <summary>
        /// Constructor of <see cref="TestBaseActiveReactiveMode "/>.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="publisher">The metrics publisher or null for a mock.</param>
        /// <param name="rampController">The ramp controller or null for a mock.</param>
        public TestBaseActiveReactiveMode(string name, IMetricsPublisher? publisher = null, IRampController? rampController = null)
            : base(name, publisher, rampController, null)
        {
        }
    }

}

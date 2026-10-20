// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Domain.Transitions;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.Systems
{
    /// <summary>
    /// Applies the override modes and the operating mode stack to the setpoints received from the supervisory control.
    /// </summary>
    /// <remarks>
    /// Calculation cycle:
    /// 1. Take a consistent snapshot of the committed stack, the override slot and their changes since the last cycle.
    /// 2. Apply the lifecycle of the changes: activate added and deactivate removed or replaced operating modes.
    /// 3. Calculate the override modes, their targets are published as <see cref="OverrideSetpoints"/>.
    /// 4. Recalculate the entire stack in priority order. The final signals are the objectives of the power axes.
    /// 5. Start a bumpless transition per affected power axis with one segment per changed operating mode. A changed operating
    ///    mode is an added, removed or replaced operating mode or a stack operating mode whose target setpoint changed. Each segment ramps the
    ///    step of its operating mode with its own ramp. The applied target stays continuous: the origin is the current applied
    ///    target, without a previous cycle the measured power at the POI (or zero). Without a base or exclusive mode the
    ///    objective of the axis is zero.
    /// 6. Enforce the mandatory constraints immediately (plant-wide overrides and nameplate) and publish the applied targets.
    /// </remarks>
    public class SystemOperatingModeControl : ISystemOperatingModeControl
    {
        /// <summary>
        /// Lock object for thread-safe access to the setpoints.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// System configuration.
        /// </summary>
        private readonly SystemConfig _systemConfig;


        /// <summary>
        /// The operating mode stack with the selected operating modes.
        /// </summary>
        private readonly OperatingModeStack _stack;


        /// <summary>
        /// The override slot with the active override modes.
        /// </summary>
        private readonly OperatingModeOverrides _overrides;


        /// <summary>
        /// Input mapping class for the measured power at the POI.
        /// </summary>
        private readonly SystemOperatingModeControlMap _map;


        /// <summary>
        /// Metrics publisher for publishing telemetry data.
        /// </summary>
        private readonly IMetricsPublisher _publisher;


        /// <summary>
        /// Publisher of the applied setpoints that the power control distributes over the system.
        /// </summary>
        private readonly ISetpointsPublisher _setpointsPublisher;


        /// <summary>
        /// Transition kind of operating modes that are activated but have not established their target yet.
        /// </summary>
        private readonly Dictionary<IOperatingMode, TransitionKind> _pendingActivations = new Dictionary<IOperatingMode, TransitionKind>();


        /// <summary>
        /// Operating modes whose transition state is tracked, keyed by name. Removed operating modes stay until their segments finished.
        /// </summary>
        private readonly Dictionary<string, IOperatingMode> _transitionModes = new Dictionary<string, IOperatingMode>(StringComparer.Ordinal);


        /// <summary>
        /// Whether a calculation cycle has completed and the applied targets are valid transition origins.
        /// </summary>
        private bool _hasAppliedTargets;


        /// <inheritdoc/>
        public SystemState State
        {
            get { lock (_dataLock) { return field; } }
            set
            {
                lock (_dataLock)
                {
                    if (value != field)
                    {
                        _logger.LogInformation("System operating mode control state changed to: {State}", value);
                        field = value;
                    }
                }
            }
        }


        /// <inheritdoc/>
        public ActivePower SetpointActivePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ActivePower(0);


        /// <inheritdoc/>
        public ReactivePower SetpointReactivePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ReactivePower(0);


        /// <inheritdoc/>
        public ActivePower SetpointActivePowerActual
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ActivePower(0);


        /// <inheritdoc/>
        public ReactivePower SetpointReactivePowerActual
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ReactivePower(0);


        /// <inheritdoc/>
        public ActivePower ActivePowerObjective
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ActivePower(0);


        /// <inheritdoc/>
        public ReactivePower ReactivePowerObjective
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ReactivePower(0);


        /// <inheritdoc/>
        public AxisTransition ActivePowerTransition { get; } = new AxisTransition();


        /// <inheritdoc/>
        public AxisTransition ReactivePowerTransition { get; } = new AxisTransition();


        /// <summary>
        /// Gets the base active power of the last calculation cycle.
        /// </summary>
        public ActivePower BaseActivePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ActivePower(0);


        /// <summary>
        /// Gets the base reactive power of the last calculation cycle.
        /// </summary>
        public ReactivePower BaseReactivePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ReactivePower(0);


        /// <summary>
        /// Gets the total additive active power delta of the last calculation cycle.
        /// </summary>
        public ActivePower AdditiveActivePowerDelta
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ActivePower(0);


        /// <summary>
        /// Gets the total additive reactive power delta of the last calculation cycle.
        /// </summary>
        public ReactivePower AdditiveReactivePowerDelta
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ReactivePower(0);


        /// <summary>
        /// Gets whether the active power was limited in the last calculation cycle.
        /// </summary>
        public bool IsActivePowerLimited
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Gets whether the reactive power was limited in the last calculation cycle.
        /// </summary>
        public bool IsReactivePowerLimited
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <inheritdoc/>
        public IReadOnlyList<UnitOverrideSetpoint> OverrideSetpoints
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = Array.Empty<UnitOverrideSetpoint>();



        /// <summary>
        /// Constructor of <see cref="SystemOperatingModeControl"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="stack">The operating mode stack with the selected operating modes.</param>
        /// <param name="overrides">The override slot with the active override modes.</param>
        /// <param name="map">Input mapping class for the measured power at the POI.</param>
        /// <param name="publisher">The metrics publisher.</param>
        /// <param name="setpointsPublisher">Publisher of the applied setpoints that the power control distributes over the system.</param>
        public SystemOperatingModeControl(ILogger logger, SystemConfig systemConfig, OperatingModeStack stack, OperatingModeOverrides overrides,
            SystemOperatingModeControlMap map, IMetricsPublisher publisher, ISetpointsPublisher setpointsPublisher)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(systemConfig);
            ArgumentNullException.ThrowIfNull(stack);
            ArgumentNullException.ThrowIfNull(overrides);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(setpointsPublisher);

            _logger = logger;
            _systemConfig = systemConfig;
            _stack = stack;
            _overrides = overrides;
            _map = map;
            _publisher = publisher;
            _setpointsPublisher = setpointsPublisher;
            RegisterMetrics();
        }


        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException">A setpoint is outside the nameplate.</exception>
        public void UpdateSetpoints(ActivePower activePower, ReactivePower reactivePower)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(activePower.Watts, _systemConfig.NameplateMinimumActivePowerWatt);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(activePower.Watts, _systemConfig.NameplateMaximumActivePowerWatt);
            ArgumentOutOfRangeException.ThrowIfLessThan(reactivePower.VoltAmperesReactive, _systemConfig.NameplateMinimumReactivePowerVars);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(reactivePower.VoltAmperesReactive, _systemConfig.NameplateMaximumReactivePowerVars);

            lock (_dataLock)
            {
                SetpointActivePower = activePower;
                SetpointReactivePower = reactivePower;
            }

            _logger.LogInformation("Supervisory setpoints updated to {SetpointActivePower} and {SetpointReactivePower}.", activePower, reactivePower);
        }


        /// <inheritdoc/>
        /// <remarks>
        /// If an operating mode throws an exception the actual setpoints of the last cycle are held and the state is set to faulted.
        /// </remarks>
        public void Calculate(CancellationToken cancellationToken)
        {
            OperatingModeSignals signals;

            lock (_dataLock)
            {
                signals = new OperatingModeSignals
                {
                    SetpointActivePower = SetpointActivePower,
                    SetpointReactivePower = SetpointReactivePower
                };
            }

            OperatingModeCycle<IStackableMode> stack = _stack.TakeCycle();
            OperatingModeCycle<IOverrideMode> overrides = _overrides.TakeCycle();

            try
            {
                List<AxisChange> changes = new List<AxisChange>();
                ApplyOverrideChanges(overrides.Changes, changes);
                ApplyStackChanges(stack.Changes, changes);
                List<UnitOverrideSetpoint> overrideSetpoints = CalculateOverrides(overrides.OperatingModes, signals, cancellationToken);

                foreach (IStackableMode operatingMode in stack.OperatingModes)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    CalculateOperatingMode(operatingMode, signals, changes);
                }

                IOverrideMode? plantOverride = overrides.OperatingModes.FirstOrDefault(overrideMode => overrideMode.TargetDerUnit is null);
                double activeApplied = ResolveAxis(PowerAxis.Active, signals.FinalActivePower.Watts, changes, plantOverride?.ActiveControl?.Target.Watts);
                double reactiveApplied = ResolveAxis(PowerAxis.Reactive, signals.FinalReactivePower.VoltAmperesReactive, changes, plantOverride?.ReactiveControl?.Target.VoltAmperesReactive);
                UpdateTransitionStates(stack.OperatingModes, changes);
                ActivePower setpointActivePowerActual = new ActivePower(activeApplied);
                ReactivePower setpointReactivePowerActual = new ReactivePower(reactiveApplied);
                bool isSetpointChanged;

                lock (_dataLock)
                {
                    // The first calculated setpoints are always published
                    isSetpointChanged = SetpointActivePowerActual != setpointActivePowerActual || SetpointReactivePowerActual != setpointReactivePowerActual;
                    ActivePowerObjective = signals.FinalActivePower;
                    ReactivePowerObjective = signals.FinalReactivePower;
                    SetpointActivePowerActual = setpointActivePowerActual;
                    SetpointReactivePowerActual = setpointReactivePowerActual;
                    BaseActivePower = signals.BaseActivePower;
                    BaseReactivePower = signals.BaseReactivePower;
                    AdditiveActivePowerDelta = signals.AdditiveActivePowerDelta;
                    AdditiveReactivePowerDelta = signals.AdditiveReactivePowerDelta;
                    IsActivePowerLimited = signals.LimitedActivePower.HasValue && (signals.LimitedActivePower.Value != signals.BaseActivePower + signals.AdditiveActivePowerDelta);
                    IsReactivePowerLimited = signals.LimitedReactivePower.HasValue && (signals.LimitedReactivePower.Value != signals.BaseReactivePower + signals.AdditiveReactivePowerDelta);
                    OverrideSetpoints = overrideSetpoints;
                    _hasAppliedTargets = true;
                    State = SystemState.Running;
                }

                if (isSetpointChanged == true)
                {
                    PublishSetpoints(setpointActivePowerActual, setpointReactivePowerActual);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Hold the actual setpoints of the last cycle
                _logger.LogError(ex, "System operating mode control calculation failed. Actual setpoints are held.");
                State = SystemState.Faulted;
            }
        }


        /// <summary>
        /// Publishes the applied setpoints that the power control distributes over the system.
        /// </summary>
        /// <remarks>
        /// A failing publisher does not fault the calculation, the applied setpoints are already committed.
        /// </remarks>
        /// <param name="setpointActivePower">The applied active power setpoint.</param>
        /// <param name="setpointReactivePower">The applied reactive power setpoint.</param>
        private void PublishSetpoints(ActivePower setpointActivePower, ReactivePower setpointReactivePower)
        {
            try
            {
                _setpointsPublisher.Publish(setpointActivePower, setpointReactivePower);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Publishing the setpoints {SetpointActivePower} and {SetpointReactivePower} failed.", setpointActivePower, setpointReactivePower);
            }
        }


        /// <summary>
        /// Applies the lifecycle of the committed override slot changes.
        /// </summary>
        /// <remarks>
        /// A plant-wide override forces the applied targets immediately, its removal transitions back to the objectives.
        /// </remarks>
        /// <param name="overrideChanges">The committed override slot changes.</param>
        /// <param name="changes">The power axis changes of the current cycle.</param>
        private void ApplyOverrideChanges(IReadOnlyList<OperatingModeChange<IOverrideMode>> overrideChanges, List<AxisChange> changes)
        {
            foreach (OperatingModeChange<IOverrideMode> change in overrideChanges)
            {
                if (change.Kind == OperatingModeChangeKind.Added)
                {
                    Activate(change.OperatingMode);

                    // A plant-wide override takes over from the current applied targets, e.g. a shutdown ramps them to zero
                    if (change.OperatingMode.TargetDerUnit is null)
                    {
                        change.OperatingMode.ActiveControl?.SetStartTarget(new ActivePower(GetTransitionOrigin(PowerAxis.Active)));
                        change.OperatingMode.ReactiveControl?.SetStartTarget(new ReactivePower(GetTransitionOrigin(PowerAxis.Reactive)));
                    }

                    continue;
                }

                if (change.OperatingMode.TargetDerUnit is null)
                {
                    AddAxisChanges(change.OperatingMode, TransitionKind.ModeRemoved, changes);
                }

                Deactivate(change.OperatingMode);
            }
        }


        /// <summary>
        /// Applies the lifecycle of the committed stack changes and records the affected power axes with the previous contributions.
        /// </summary>
        /// <param name="stackChanges">The committed stack changes.</param>
        /// <param name="changes">The power axis changes of the current cycle.</param>
        private void ApplyStackChanges(IReadOnlyList<OperatingModeChange<IStackableMode>> stackChanges, List<AxisChange> changes)
        {
            foreach (OperatingModeChange<IStackableMode> change in stackChanges)
            {
                switch (change.Kind)
                {
                    case OperatingModeChangeKind.Added:
                        ActivateStackMode(change.OperatingMode, _hasAppliedTargets ? TransitionKind.ModeAdded : TransitionKind.InitialActivation);
                        AddAxisChanges(change.OperatingMode, _hasAppliedTargets ? TransitionKind.ModeAdded : TransitionKind.InitialActivation, changes);
                        break;

                    case OperatingModeChangeKind.Removed:
                        AddAxisChanges(change.OperatingMode, TransitionKind.ModeRemoved, changes);
                        Deactivate(change.OperatingMode);
                        break;

                    case OperatingModeChangeKind.Replaced:
                        AddReplacementChanges(change.Predecessor!, change.OperatingMode, changes);
                        Deactivate(change.Predecessor!);
                        ActivateStackMode(change.OperatingMode, _hasAppliedTargets ? TransitionKind.ModeReplacement : TransitionKind.InitialActivation);
                        break;
                }
            }
        }


        /// <summary>
        /// Records the changes of a replacement.
        /// </summary>
        /// <remarks>
        /// On a power axis of both operating modes the replacement is one change of the replacing operating mode starting from the
        /// contribution of the predecessor, so the handover uses the ramp of the replacing operating mode only.
        /// </remarks>
        /// <param name="predecessor">The replaced operating mode.</param>
        /// <param name="operatingMode">The replacing operating mode.</param>
        /// <param name="changes">The power axis changes of the current cycle.</param>
        private void AddReplacementChanges(IStackableMode predecessor, IStackableMode operatingMode, List<AxisChange> changes)
        {
            TransitionKind kind = _hasAppliedTargets ? TransitionKind.ModeReplacement : TransitionKind.InitialActivation;

            foreach (PowerAxis axis in new PowerAxis[] { PowerAxis.Active, PowerAxis.Reactive })
            {
                bool hasPredecessorAxis = GetRampController(predecessor, axis) is not null;
                bool hasOperatingModeAxis = GetRampController(operatingMode, axis) is not null;

                if (hasOperatingModeAxis)
                {
                    double previousContribution = hasPredecessorAxis ? GetContribution(predecessor, axis) : 0;
                    changes.Add(new AxisChange(axis, operatingMode, kind, previousContribution));
                }
                else if (hasPredecessorAxis)
                {
                    changes.Add(new AxisChange(axis, predecessor, TransitionKind.ModeRemoved, GetContribution(predecessor, axis)));
                }
            }
        }


        /// <summary>
        /// Calculates the override modes.
        /// </summary>
        /// <param name="overrideModes">The active override modes.</param>
        /// <param name="signals">The operating mode signals of the current cycle.</param>
        /// <param name="cancellationToken">Token to cancel the calculation.</param>
        /// <returns>The forced setpoints of the override modes.</returns>
        private static List<UnitOverrideSetpoint> CalculateOverrides(IReadOnlyList<IOverrideMode> overrideModes, OperatingModeSignals signals, CancellationToken cancellationToken)
        {
            List<UnitOverrideSetpoint> overrideSetpoints = new List<UnitOverrideSetpoint>();

            foreach (IOverrideMode overrideMode in overrideModes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                overrideMode.Calculate(signals);
                overrideSetpoints.Add(new UnitOverrideSetpoint(overrideMode.Name, overrideMode.TargetDerUnit, overrideMode.ActiveControl?.Target, overrideMode.ReactiveControl?.Target));
            }

            return overrideSetpoints;
        }


        /// <summary>
        /// Calculates an operating mode, records its contribution to the final signals and records the power axes on which it established its target.
        /// </summary>
        /// <param name="operatingMode">The operating mode to calculate.</param>
        /// <param name="signals">The operating mode signals of the current cycle.</param>
        /// <param name="changes">The power axis changes of the current cycle.</param>
        private void CalculateOperatingMode(IStackableMode operatingMode, OperatingModeSignals signals, List<AxisChange> changes)
        {
            ActivePower activeBefore = signals.FinalActivePower;
            ReactivePower reactiveBefore = signals.FinalReactivePower;
            double activeContributionBefore = operatingMode.ActivePowerContribution.Watts;
            double reactiveContributionBefore = operatingMode.ReactivePowerContribution.VoltAmperesReactive;
            OperatingModeState? activeStateBefore = operatingMode.ActiveControl?.State;
            OperatingModeState? reactiveStateBefore = operatingMode.ReactiveControl?.State;
            operatingMode.Calculate(signals);

            if (operatingMode is OperatingModeBase operatingModeBase)
            {
                operatingModeBase.SetContribution(signals.FinalActivePower - activeBefore, signals.FinalReactivePower - reactiveBefore);
            }

            // A stack operating mode follows its target setpoint immediately, the step is transitioned with its ramp (one ramp layer)
            if (operatingMode.ActiveControl?.HasTargetSetpointChanged == true)
            {
                changes.Add(new AxisChange(PowerAxis.Active, operatingMode, TransitionKind.TargetChanged, activeContributionBefore));
            }

            if (operatingMode.ReactiveControl?.HasTargetSetpointChanged == true)
            {
                changes.Add(new AxisChange(PowerAxis.Reactive, operatingMode, TransitionKind.TargetChanged, reactiveContributionBefore));
            }

            // An operating mode waiting for available power establishes its target later, that is a change of the axis as well
            if (_pendingActivations.TryGetValue(operatingMode, out TransitionKind kind))
            {
                if (activeStateBefore == OperatingModeState.Enabling && operatingMode.ActiveControl?.State == OperatingModeState.Enabled)
                {
                    changes.Add(new AxisChange(PowerAxis.Active, operatingMode, kind, activeContributionBefore));
                }

                if (reactiveStateBefore == OperatingModeState.Enabling && operatingMode.ReactiveControl?.State == OperatingModeState.Enabled)
                {
                    changes.Add(new AxisChange(PowerAxis.Reactive, operatingMode, kind, reactiveContributionBefore));
                }

                if (operatingMode.ActiveControl?.State != OperatingModeState.Enabling && operatingMode.ReactiveControl?.State != OperatingModeState.Enabling)
                {
                    _pendingActivations.Remove(operatingMode);
                }
            }
        }


        /// <summary>
        /// Resolves the applied target of a power axis from its objective, the transition and the mandatory constraints.
        /// </summary>
        /// <param name="axis">The power axis.</param>
        /// <param name="objective">The objective of the power axis in base unit.</param>
        /// <param name="changes">The power axis changes of the current cycle.</param>
        /// <param name="forcedTarget">The target forced by a plant-wide override or null.</param>
        /// <returns>The applied target in base unit.</returns>
        private double ResolveAxis(PowerAxis axis, double objective, List<AxisChange> changes, double? forcedTarget)
        {
            AxisTransition transition = axis == PowerAxis.Active ? ActivePowerTransition : ReactivePowerTransition;

            // The new segments cover the step against the offsets of the previous cycle, so running segments keep their own progress
            double previousOffset = transition.Offset;
            // Call all segments of transition (ramp controller calculate)
            transition.Advance();
            List<TransitionRequest> requests = BuildTransitionRequests(axis, changes);

            if (requests.Count > 0)
            {
                double origin = GetTransitionOrigin(axis);
                transition.Start(origin - objective - previousOffset, requests);
                _logger.LogInformation("{Axis} power transition caused by {Names} from {Origin} to {Objective}.", axis,
                    string.Join(", ", requests.Select(request => $"{request.OperatingModeName} ({request.Kind})")), origin, objective);
            }

            double applied = objective + transition.Offset;

            // Mandatory constraints are enforced immediately and never delayed by a transition
            if (forcedTarget.HasValue)
            {
                transition.Cancel();
                applied = forcedTarget.Value;
            }

            double constrained = ClampToNameplate(axis, applied);

            if (constrained != applied)
            {
                transition.Constrain(constrained - objective);
            }

            return constrained;
        }


        /// <summary>
        /// Tells the operating modes whether a transition segment caused by them is running, so that an enabled operating mode
        /// shows <see cref="OperatingModeState.RampingToEnabled"/> until the applied target reached its target and a removed
        /// operating mode shows <see cref="OperatingModeState.RampingToDisabled"/> until its contribution is ramped to zero.
        /// </summary>
        /// <param name="stackModes">The selected operating modes of the current cycle.</param>
        /// <param name="changes">The power axis changes of the current cycle.</param>
        private void UpdateTransitionStates(IReadOnlyList<IStackableMode> stackModes, List<AxisChange> changes)
        {
            foreach (IOperatingMode operatingMode in stackModes.Concat(changes.Select(change => change.OperatingMode)))
            {
                _transitionModes[operatingMode.Name] = operatingMode;
            }

            HashSet<string> activeSegments = ActivePowerTransition.Segments.Select(segment => segment.OperatingModeName).ToHashSet(StringComparer.Ordinal);
            HashSet<string> reactiveSegments = ReactivePowerTransition.Segments.Select(segment => segment.OperatingModeName).ToHashSet(StringComparer.Ordinal);

            foreach (IOperatingMode operatingMode in _transitionModes.Values.ToArray())
            {
                bool isActiveTransitioning = activeSegments.Contains(operatingMode.Name);
                bool isReactiveTransitioning = reactiveSegments.Contains(operatingMode.Name);
                operatingMode.ActiveControl?.SetTransitioning(isActiveTransitioning);
                operatingMode.ReactiveControl?.SetTransitioning(isReactiveTransitioning);

                // Removed operating modes are no longer tracked once their segments finished
                if (isActiveTransitioning is false && isReactiveTransitioning is false && operatingMode.IsActivated is false)
                {
                    _transitionModes.Remove(operatingMode.Name);
                }
            }
        }


        /// <summary>
        /// Builds one transition request per changed operating mode of a power axis.
        /// </summary>
        /// <remarks>
        /// The step of an operating mode is its contribution before the changes minus its current contribution. Several changes of
        /// the same operating mode in one cycle are combined starting from the contribution before the first change.
        /// </remarks>
        /// <param name="axis">The power axis.</param>
        /// <param name="changes">The power axis changes of the current cycle.</param>
        /// <returns>The transition requests in change order.</returns>
        private List<TransitionRequest> BuildTransitionRequests(PowerAxis axis, List<AxisChange> changes)
        {
            List<TransitionRequest> requests = new List<TransitionRequest>();

            foreach (IGrouping<IOperatingMode, AxisChange> modeChanges in changes.Where(change => change.Axis == axis).GroupBy(change => change.OperatingMode))
            {
                IOperatingMode operatingMode = modeChanges.Key;
                IRampController? rampController = GetRampController(operatingMode, axis);

                if (rampController is null)
                {
                    continue;
                }

                double currentContribution = operatingMode.IsActivated ? GetContribution(operatingMode, axis) : 0;
                double offset = modeChanges.First().PreviousContribution - currentContribution;
                requests.Add(new TransitionRequest(operatingMode.Name, modeChanges.Last().Kind, offset, rampController.ShallowCopy(), OwnsAxis(operatingMode, axis)));
            }

            return requests;
        }


        /// <summary>
        /// Gets the origin of a transition, the current applied target or the measured power at the POI if there is no previous cycle.
        /// </summary>
        /// <param name="axis">The power axis.</param>
        /// <returns>The transition origin in base unit.</returns>
        private double GetTransitionOrigin(PowerAxis axis)
        {
            if (_hasAppliedTargets)
            {
                return axis == PowerAxis.Active ? SetpointActivePowerActual.Watts : SetpointReactivePowerActual.VoltAmperesReactive;
            }

            double? measured = axis == PowerAxis.Active ? _map.ActivePowerAtPoi.Invoke()?.Watts : _map.ReactivePowerAtPoi.Invoke()?.VoltAmperesReactive;

            if (measured.HasValue is false || ClampToNameplate(axis, measured.Value) != measured.Value)
            {
                return 0;
            }

            return measured.Value;
        }


        /// <summary>
        /// Clamps a value of a power axis to the nameplate.
        /// </summary>
        /// <param name="axis">The power axis.</param>
        /// <param name="value">The value in base unit.</param>
        /// <returns>The clamped value.</returns>
        private double ClampToNameplate(PowerAxis axis, double value)
        {
            double minimum = axis == PowerAxis.Active ? _systemConfig.NameplateMinimumActivePowerWatt : _systemConfig.NameplateMinimumReactivePowerVars;
            double maximum = axis == PowerAxis.Active ? _systemConfig.NameplateMaximumActivePowerWatt : _systemConfig.NameplateMaximumReactivePowerVars;

            return Math.Min(Math.Max(value, minimum), maximum);
        }


        /// <summary>
        /// Activates a stack operating mode and remembers the kind of transition until it established its target.
        /// </summary>
        /// <param name="operatingMode">The operating mode to activate.</param>
        /// <param name="kind">The kind of transition.</param>
        private void ActivateStackMode(IStackableMode operatingMode, TransitionKind kind)
        {
            Activate(operatingMode);
            _pendingActivations[operatingMode] = kind;
        }


        /// <summary>
        /// Activates an operating mode.
        /// </summary>
        /// <param name="operatingMode">The operating mode to activate.</param>
        private void Activate(IOperatingMode operatingMode)
        {
            if (operatingMode is OperatingModeBase operatingModeBase)
            {
                operatingModeBase.Activate();
            }
        }


        /// <summary>
        /// Deactivates an operating mode.
        /// </summary>
        /// <param name="operatingMode">The operating mode to deactivate.</param>
        private void Deactivate(IOperatingMode operatingMode)
        {
            _pendingActivations.Remove(operatingMode);

            if (operatingMode is OperatingModeBase operatingModeBase)
            {
                operatingModeBase.Deactivate();
            }
        }


        /// <summary>
        /// Records a change with the current contribution for every power axis of the operating mode.
        /// </summary>
        /// <param name="operatingMode">The changed operating mode.</param>
        /// <param name="kind">The kind of transition.</param>
        /// <param name="changes">The power axis changes of the current cycle.</param>
        private void AddAxisChanges(IOperatingMode operatingMode, TransitionKind kind, List<AxisChange> changes)
        {
            if (operatingMode.ActiveControl is not null)
            {
                changes.Add(new AxisChange(PowerAxis.Active, operatingMode, kind, GetContribution(operatingMode, PowerAxis.Active)));
            }

            if (operatingMode.ReactiveControl is not null)
            {
                changes.Add(new AxisChange(PowerAxis.Reactive, operatingMode, kind, GetContribution(operatingMode, PowerAxis.Reactive)));
            }
        }


        /// <summary>
        /// Gets the contribution of an operating mode to a power axis.
        /// </summary>
        /// <param name="operatingMode">The operating mode.</param>
        /// <param name="axis">The power axis.</param>
        /// <returns>The contribution in base unit.</returns>
        private double GetContribution(IOperatingMode operatingMode, PowerAxis axis)
        {
            return axis == PowerAxis.Active ? operatingMode.ActivePowerContribution.Watts : operatingMode.ReactivePowerContribution.VoltAmperesReactive;
        }


        /// <summary>
        /// Gets whether the operating mode owns the power axis (base, exclusive or override).
        /// </summary>
        /// <param name="operatingMode">The operating mode.</param>
        /// <param name="axis">The power axis.</param>
        /// <returns>True if the operating mode owns the power axis.</returns>
        private bool OwnsAxis(IOperatingMode operatingMode, PowerAxis axis)
        {
            OperatingModeClassification owners = axis == PowerAxis.Active
                ? OperatingModeClassification.Override | OperatingModeClassification.BaseActive | OperatingModeClassification.ExclusiveActive
                : OperatingModeClassification.Override | OperatingModeClassification.BaseReactive | OperatingModeClassification.ExclusiveReactive;

            return (operatingMode.Classification & owners) != OperatingModeClassification.None;
        }


        /// <summary>
        /// Gets the ramp controller of the power axis of an operating mode.
        /// </summary>
        /// <param name="operatingMode">The operating mode.</param>
        /// <param name="axis">The power axis.</param>
        /// <returns>The ramp controller or null if the operating mode has no such power axis.</returns>
        private IRampController? GetRampController(IOperatingMode operatingMode, PowerAxis axis)
        {
            return axis == PowerAxis.Active ? operatingMode.ActiveControl?.RampController : operatingMode.ReactiveControl?.RampController;
        }


        /// <summary>
        /// Register metrics at the publisher.
        /// </summary>
        private void RegisterMetrics()
        {
            Dictionary<string, object?> tags = new Dictionary<string, object?>();

            // Initialize metrics
            _publisher.Initialize(nameof(SystemOperatingModeControl), tags);
            // MetricsFactorClass1
            _publisher.Register<SystemOperatingModeControl, int>(this, nameof(State), MetricType.Gauge, x => (int)x.State, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(SetpointActivePower), MetricType.Gauge, x => x.SetpointActivePower.Watts, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(SetpointReactivePower), MetricType.Gauge, x => x.SetpointReactivePower.VoltAmperesReactive, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(SetpointActivePowerActual), MetricType.Gauge, x => x.SetpointActivePowerActual.Watts, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(SetpointReactivePowerActual), MetricType.Gauge, x => x.SetpointReactivePowerActual.VoltAmperesReactive, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(ActivePowerObjective), MetricType.Gauge, x => x.ActivePowerObjective.Watts, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(ReactivePowerObjective), MetricType.Gauge, x => x.ReactivePowerObjective.VoltAmperesReactive, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, "ActivePowerTransitionOffset", MetricType.Gauge, x => x.ActivePowerTransition.Offset, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, "ReactivePowerTransitionOffset", MetricType.Gauge, x => x.ReactivePowerTransition.Offset, 1);
            _publisher.Register<SystemOperatingModeControl, int>(this, nameof(ActivePowerTransition), MetricType.Gauge, x => (int)x.ActivePowerTransition.Kind, 1);
            _publisher.Register<SystemOperatingModeControl, int>(this, nameof(ReactivePowerTransition), MetricType.Gauge, x => (int)x.ReactivePowerTransition.Kind, 1);
            _publisher.Register<SystemOperatingModeControl, int>(this, "ActivePowerTransitionCount", MetricType.Gauge, x => x.ActivePowerTransition.Segments.Count, 1);
            _publisher.Register<SystemOperatingModeControl, int>(this, "ReactivePowerTransitionCount", MetricType.Gauge, x => x.ReactivePowerTransition.Segments.Count, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(BaseActivePower), MetricType.Gauge, x => x.BaseActivePower.Watts, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(BaseReactivePower), MetricType.Gauge, x => x.BaseReactivePower.VoltAmperesReactive, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(AdditiveActivePowerDelta), MetricType.Gauge, x => x.AdditiveActivePowerDelta.Watts, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(AdditiveReactivePowerDelta), MetricType.Gauge, x => x.AdditiveReactivePowerDelta.VoltAmperesReactive, 1);
            _publisher.Register<SystemOperatingModeControl, int>(this, nameof(IsActivePowerLimited), MetricType.Gauge, x => x.IsActivePowerLimited ? 1 : 0, 1);
            _publisher.Register<SystemOperatingModeControl, int>(this, nameof(IsReactivePowerLimited), MetricType.Gauge, x => x.IsReactivePowerLimited ? 1 : 0, 1);
            _publisher.Register<SystemOperatingModeControl, int>(this, "OverrideCount", MetricType.Gauge, x => x.OverrideSetpoints.Count, 1);
        }


        /// <summary>
        /// Change of a power axis caused by an operating mode in the current cycle.
        /// </summary>
        /// <param name="Axis">The affected power axis.</param>
        /// <param name="OperatingMode">The operating mode causing the change.</param>
        /// <param name="Kind">The kind of transition.</param>
        /// <param name="PreviousContribution">The contribution of the operating mode to the power axis before the change.</param>
        private sealed record AxisChange(PowerAxis Axis, IOperatingMode OperatingMode, TransitionKind Kind, double PreviousContribution);
    }
}

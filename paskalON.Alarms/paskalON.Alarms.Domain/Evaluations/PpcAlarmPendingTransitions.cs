// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Evaluations
{
    /// <summary>
    /// In memory list of the alarm raises and normalizations that wait for the delay of their configuration to elapse.
    /// </summary>
    /// <remarks>
    /// - At most one pending transition per publisher, configuration and property.
    /// - The pending transitions are not persisted, after a restart the delay starts again with the first received value.
    /// - Thread safe, the delay check reads whether transitions are due while the alarm handler changes them.
    /// </remarks>
    public class PpcAlarmPendingTransitions
    {
        /// <summary>
        /// Lock of the pending transitions.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Pending transitions per publisher, configuration and property.
        /// </summary>
        private readonly Dictionary<(PpcAlarmKey Key, int PpcAlarmConfigId, string PropertyName), PpcAlarmPendingTransition> _transitions =
            new Dictionary<(PpcAlarmKey Key, int PpcAlarmConfigId, string PropertyName), PpcAlarmPendingTransition>();


        /// <summary>
        /// Gets the number of pending transitions.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_dataLock)
                {
                    return _transitions.Count;
                }
            }
        }


        /// <summary>
        /// Starts the delay of a transition. An already pending transition of the same kind keeps its delay,
        /// so that further changes of an abnormal (or normal) value do not restart it.
        /// A pending transition of the other kind is replaced.
        /// </summary>
        /// <param name="transition">The transition to start.</param>
        public void Start(PpcAlarmPendingTransition transition)
        {
            ArgumentNullException.ThrowIfNull(transition);

            lock (_dataLock)
            {
                (PpcAlarmKey, int, string) id = (transition.Key, transition.PpcAlarmConfigId, transition.PropertyName);

                if (_transitions.TryGetValue(id, out PpcAlarmPendingTransition? pending) && pending.Transition == transition.Transition)
                {
                    return;
                }

                _transitions[id] = transition;
            }
        }


        /// <summary>
        /// Cancels the pending transition of a publisher, configuration and property, e.g. the value is normal again before the alarm was raised.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="ppcAlarmConfigId">Id of the alarm configuration.</param>
        /// <param name="propertyName">Property or dictionary entry of the alarm.</param>
        /// <returns>True if a pending transition was cancelled.</returns>
        public bool Cancel(PpcAlarmKey key, int ppcAlarmConfigId, string propertyName)
        {
            lock (_dataLock)
            {
                return _transitions.Remove((key, ppcAlarmConfigId, propertyName));
            }
        }


        /// <summary>
        /// Cancels the pending transitions of a property of a publisher whose configuration is not one of the given configurations
        /// (e.g. overridden by a more specific configuration).
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="propertyName">Property or dictionary entry.</param>
        /// <param name="ppcAlarmConfigIds">Ids of the configurations whose transitions are kept.</param>
        public void CancelOthers(PpcAlarmKey key, string propertyName, IReadOnlyCollection<int> ppcAlarmConfigIds)
        {
            ArgumentNullException.ThrowIfNull(ppcAlarmConfigIds);

            lock (_dataLock)
            {
                List<(PpcAlarmKey, int, string)> ids = _transitions.Keys
                    .Where(id => id.Key == key && id.PropertyName == propertyName && ppcAlarmConfigIds.Contains(id.PpcAlarmConfigId) is false)
                    .ToList();

                foreach ((PpcAlarmKey, int, string) id in ids)
                {
                    _transitions.Remove(id);
                }
            }
        }


        /// <summary>
        /// Cancels all pending transitions of a publisher, e.g. after its evaluation failed and its values are evaluated again.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        public void CancelAll(PpcAlarmKey key)
        {
            lock (_dataLock)
            {
                List<(PpcAlarmKey, int, string)> ids = _transitions.Keys.Where(id => id.Key == key).ToList();

                foreach ((PpcAlarmKey, int, string) id in ids)
                {
                    _transitions.Remove(id);
                }
            }
        }


        /// <summary>
        /// Gets the pending transition of a publisher, configuration and property.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="ppcAlarmConfigId">Id of the alarm configuration.</param>
        /// <param name="propertyName">Property or dictionary entry of the alarm.</param>
        /// <returns>The pending transition, null if there is none.</returns>
        public PpcAlarmPendingTransition? Get(PpcAlarmKey key, int ppcAlarmConfigId, string propertyName)
        {
            lock (_dataLock)
            {
                _transitions.TryGetValue((key, ppcAlarmConfigId, propertyName), out PpcAlarmPendingTransition? transition);

                return transition;
            }
        }


        /// <summary>
        /// Gets the publishers with transitions whose delay elapsed.
        /// </summary>
        /// <param name="now">Current time.</param>
        /// <returns>Keys of the publishers.</returns>
        public IReadOnlyList<PpcAlarmKey> GetDueKeys(DateTimeOffset now)
        {
            lock (_dataLock)
            {
                return _transitions.Values
                    .Where(transition => transition.DueOn <= now)
                    .Select(transition => transition.Key)
                    .Distinct()
                    .ToList();
            }
        }


        /// <summary>
        /// Removes and returns the transitions of a publisher whose delay elapsed, in the order of their value changes.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="now">Current time.</param>
        /// <returns>The due transitions.</returns>
        public IReadOnlyList<PpcAlarmPendingTransition> TakeDue(PpcAlarmKey key, DateTimeOffset now)
        {
            lock (_dataLock)
            {
                List<PpcAlarmPendingTransition> due = _transitions.Values
                    .Where(transition => transition.Key == key && transition.DueOn <= now)
                    .OrderBy(transition => transition.Value.ValueChangedOn)
                    .ToList();

                foreach (PpcAlarmPendingTransition transition in due)
                {
                    _transitions.Remove((transition.Key, transition.PpcAlarmConfigId, transition.PropertyName));
                }

                return due;
            }
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Evaluations;

namespace paskalON.Alarms.Application.Stores
{
    /// <summary>
    /// In memory stateless store of the latest published alarm relevant values per publisher.
    /// </summary>
    /// <remarks>
    /// The store is not shared with failover instances (not implemented yet).
    /// Values that are missing in an update keep their latest value.
    /// </remarks>
    public class PpcAlarmValueStore : IPpcAlarmValueStore
    {
        /// <summary>
        /// Lock of the stored values.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Latest values by name per publisher.
        /// </summary>
        private readonly Dictionary<PpcAlarmKey, Dictionary<string, PpcAlarmKeyValue>> _values = new Dictionary<PpcAlarmKey, Dictionary<string, PpcAlarmKeyValue>>();


        /// <inheritdoc/>
        public List<PpcAlarmKeyValue> Update(PpcAlarmKey key, IReadOnlyDictionary<string, double> values, DateTimeOffset receivedOn)
        {
            ArgumentNullException.ThrowIfNull(values);

            List<PpcAlarmKeyValue> changedValues = new List<PpcAlarmKeyValue>();

            lock (_dataLock)
            {
                if (_values.TryGetValue(key, out Dictionary<string, PpcAlarmKeyValue>? storedValues) is false)
                {
                    storedValues = new Dictionary<string, PpcAlarmKeyValue>(StringComparer.Ordinal);
                    _values[key] = storedValues;
                }

                foreach (KeyValuePair<string, double> value in values)
                {
                    bool isChanged = storedValues.TryGetValue(value.Key, out PpcAlarmKeyValue? storedValue) is false
                        || storedValue.Value.Equals(value.Value) is false;

                    if (isChanged)
                    {
                        PpcAlarmKeyValue changedValue = new PpcAlarmKeyValue
                        {
                            Name = value.Key,
                            Value = value.Value,
                            ValueChanged = true,
                            ValueChangedOn = receivedOn
                        };
                        storedValues[value.Key] = changedValue;
                        changedValues.Add(changedValue);
                    }
                    else if (storedValue != null && storedValue.ValueChanged)
                    {
                        storedValues[value.Key] = storedValue with { ValueChanged = false };
                    }
                }
            }

            return changedValues;
        }


        /// <inheritdoc/>
        public List<PpcAlarmKeyValue> GetValues(PpcAlarmKey key)
        {
            lock (_dataLock)
            {
                if (_values.TryGetValue(key, out Dictionary<string, PpcAlarmKeyValue>? storedValues))
                {
                    return storedValues.Values.ToList();
                }

                return new List<PpcAlarmKeyValue>();
            }
        }


        /// <inheritdoc/>
        public void Remove(PpcAlarmKey key)
        {
            lock (_dataLock)
            {
                _values.Remove(key);
            }
        }
    }
}

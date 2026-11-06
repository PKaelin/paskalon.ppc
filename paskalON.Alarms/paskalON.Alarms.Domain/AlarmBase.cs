// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs;
using paskalON.Domains;

namespace paskalON.Alarms.Domain
{
    /// <summary>
    /// Base of all alarm domains, handles the alarm sequence.
    /// </summary>
    /// <remarks>
    /// A new alarm is raised in the sequence state <see cref="AlarmSequenceState.Alarm"/> with an abnormal process condition.
    /// Alarm -> Acknowledged: The operator acknowledges the alarm (not possible for critical alarms).
    /// Alarm/Acknowledged -> Normal: The process condition reverts to normal.
    /// A normalized alarm is final, a new occurrence raises a new alarm.
    /// </remarks>
    public abstract class AlarmBase : DomainBase
    {
        /// <summary>
        /// Timestamp when the alarm was created.
        /// </summary>
        public required DateTimeOffset CreatedOn
        {
            get => field;
            set => field = value.ToUniversalTime();
        }


        /// <summary>
        /// State of the alarm in the alarm sequence.
        /// </summary>
        public AlarmSequenceState AlarmSequenceState { get; private set; } = AlarmSequenceState.Alarm;


        /// <summary>
        /// Condition of the process that caused the alarm event.
        /// </summary>
        public AlarmProcessCondition AlarmProcessCondition { get; private set; } = AlarmProcessCondition.Abnormal;


        /// <summary>
        /// Severity level of the alarm.
        /// </summary>
        public abstract AlarmSeverity SeverityLevel { get; }


        /// <summary>
        /// Indicates whether the alarm is active and not yet acknowledged and its severity allows an acknowledgement.
        /// </summary>
        /// <remarks>
        /// Critical alarms cannot be acknowledged.
        /// </remarks>
        public bool CanBeAcknowledged
        {
            get => AlarmSequenceState == AlarmSequenceState.Alarm && SeverityLevel != AlarmSeverity.Critical;
        }


        /// <summary>
        /// Indicates whether the alarm is active (not normalized).
        /// </summary>
        public bool IsActive
        {
            get => AlarmSequenceState != AlarmSequenceState.Normal;
        }


        /// <summary>
        /// Acknowledges the alarm.
        /// </summary>
        /// <remarks>
        /// An alarm that is already acknowledged or normalized is left unchanged.
        /// </remarks>
        /// <param name="changedBy">User or system that acknowledged the alarm.</param>
        /// <param name="changedDate">Timestamp of the acknowledgement.</param>
        /// <exception cref="InvalidOperationException">Thrown when the alarm is critical, critical alarms cannot be acknowledged.</exception>
        public void Acknowledge(string changedBy, DateTimeOffset changedDate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(changedBy);

            if (AlarmSequenceState != AlarmSequenceState.Alarm)
            {
                return;
            }

            if (SeverityLevel == AlarmSeverity.Critical)
            {
                throw new InvalidOperationException($"Alarm {Id} is critical and cannot be acknowledged.");
            }

            AlarmSequenceState = AlarmSequenceState.Acknowledged;
            ChangedBy = changedBy;
            ChangedDate = changedDate;
        }


        /// <summary>
        /// Normalizes the alarm because the process condition reverted to normal.
        /// </summary>
        /// <param name="changedBy">User or system that normalized the alarm.</param>
        /// <param name="changedDate">Timestamp of the normalization.</param>
        /// <exception cref="InvalidOperationException">Thrown when the alarm is already normalized.</exception>
        public void Normalize(string changedBy, DateTimeOffset changedDate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(changedBy);

            if (IsActive is false)
            {
                throw new InvalidOperationException($"Alarm {Id} is already normalized.");
            }

            AlarmSequenceState = AlarmSequenceState.Normal;
            AlarmProcessCondition = AlarmProcessCondition.Normal;
            ChangedBy = changedBy;
            ChangedDate = changedDate;
        }
    }
}

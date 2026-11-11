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
    /// A new alarm is raised in the sequence state <see cref="AlarmSequenceState.Alarm"/>.
    /// Alarm -> Acknowledged: The operator acknowledges the active alarm.
    /// Alarm/Acknowledged -> Normal: The value of the alarm is back to normal.
    /// A normalized alarm stays normal, a new occurrence raises a new alarm.
    /// Only an alarm whose configuration is acknowledgeable can be acknowledged, once, active or already normal:
    /// an active alarm moves to Acknowledged, a normal alarm stays Normal, both record who acknowledged it and when.
    /// Alarms of configurations that are not acknowledgeable cannot be acknowledged, the issue that caused them must be resolved:
    /// they go Alarm -> Normal.
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
        /// User who acknowledged the alarm, null while it is not acknowledged.
        /// </summary>
        public string? AcknowledgedBy { get; private set; }


        /// <summary>
        /// Timestamp of the acknowledgement, null while the alarm is not acknowledged.
        /// </summary>
        public DateTimeOffset? AcknowledgedOn
        {
            get => field;
            private set => field = value?.ToUniversalTime();
        }


        /// <summary>
        /// Severity level of the alarm.
        /// </summary>
        public abstract AlarmSeverity SeverityLevel { get; }


        /// <summary>
        /// Indicates whether the configuration of the alarm allows an acknowledgement.
        /// </summary>
        public abstract bool IsAcknowledgeable { get; }


        /// <summary>
        /// Indicates whether the alarm can be acknowledged: its configuration is acknowledgeable and it is not acknowledged yet,
        /// regardless of whether it is active or already normal.
        /// </summary>
        public bool CanBeAcknowledged
        {
            get => IsAcknowledgeable && AcknowledgedOn == null;
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
        /// - An active alarm moves to the sequence state Acknowledged, a normal alarm stays Normal.
        /// - Both record who acknowledged the alarm and when.
        /// - An alarm that is already acknowledged is left unchanged, the first acknowledgement is kept.
        /// </remarks>
        /// <param name="changedBy">User who acknowledged the alarm.</param>
        /// <param name="changedDate">Timestamp of the acknowledgement.</param>
        /// <exception cref="InvalidOperationException">Thrown when the configuration of the alarm is not acknowledgeable.</exception>
        public void Acknowledge(string changedBy, DateTimeOffset changedDate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(changedBy);

            if (IsAcknowledgeable is false)
            {
                throw new InvalidOperationException($"Alarm {Id} is not acknowledgeable.");
            }

            if (AcknowledgedOn != null)
            {
                return;
            }

            if (AlarmSequenceState == AlarmSequenceState.Alarm)
            {
                AlarmSequenceState = AlarmSequenceState.Acknowledged;
            }

            AcknowledgedBy = changedBy;
            AcknowledgedOn = changedDate;
            ChangedBy = changedBy;
            ChangedDate = changedDate;
        }


        /// <summary>
        /// Normalizes the alarm because its value is back to normal, an acknowledgement is kept.
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
            ChangedBy = changedBy;
            ChangedDate = changedDate;
        }
    }
}

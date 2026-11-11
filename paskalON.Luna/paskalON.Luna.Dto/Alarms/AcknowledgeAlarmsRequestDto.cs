// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.ComponentModel.DataAnnotations;

namespace paskalON.Luna.Dto.Alarms
{
    /// <summary>
    /// Request of the UI to acknowledge alarms, e.g. the selected alarm or the alarms shown in the alarm list.
    /// </summary>
    public record AcknowledgeAlarmsRequestDto
    {
        /// <summary>
        /// Maximum number of alarms of one request.
        /// </summary>
        public const int MaximumAlarmCount = 10000;


        /// <summary>
        /// Ids of the alarms to acknowledge. Alarms that cannot be acknowledged (not acknowledgeable or already acknowledged)
        /// are skipped by the alarm service.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(MaximumAlarmCount)]
        public required List<int> AlarmIds { get; init; }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.ComponentModel.DataAnnotations;

namespace paskalON.Alarms.Service.Dto.V1.Requests
{
    /// <summary>
    /// Request class to acknowledge alarms.
    /// </summary>
    public class AcknowledgeAlarmsRequest
    {
        /// <summary>
        /// Maximum number of alarms of one request.
        /// </summary>
        public const int MaximumAlarmCount = 10000;


        /// <summary>
        /// Gets or sets the ids of the alarms to acknowledge. Alarms that cannot be acknowledged
        /// (not acknowledgeable or already acknowledged) and unknown ids are skipped.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(MaximumAlarmCount)]
        public required List<int> AlarmIds { get; set; }


        /// <summary>
        /// Gets or sets the user who acknowledges the alarms.
        /// </summary>
        [Required]
        [StringLength(250, MinimumLength = 1)]
        public required string AcknowledgedBy { get; set; }
    }
}

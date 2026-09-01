// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// C37 signal types.
    /// </summary>
    public enum C37SignalType
    {
        /// <summary>
        /// Contains bitmasks indicating data validity, synchronization status, trigger detections, and overall PMU time quality.
        /// </summary>
        StatusFlag,
        /// <summary>
        /// Grid metrics such as voltage, current, waveforms, etc.
        /// This can include individual three-phase values, positive sequence, or negative sequence components.
        /// </summary>
        Phasor,
        /// <summary>
        /// Transmission frequency of the grid or its deviation from the nominal base frequency (50 Hz or 60 Hz).
        /// </summary>
        Frequency,
        /// <summary>
        /// Derivative of frequency over time (DFREQ) or ROCOF (Rate of Change of Frequency) defining how fast the system frequency is shifting.
        /// </summary>
        RateOfChangeOfFrequency,
        /// <summary>
        /// Grid metrics such as active power, reactive power, apparent power, etc.
        /// </summary>
        Analog,
        /// <summary>
        /// Binary status words reflecting the on/off states of substation equipment, such as circuit breaker relays or switch contacts.
        /// </summary>
        Digital
    }
}

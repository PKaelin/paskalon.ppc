// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Luna.Dto.Devices
{
    /// <summary>
    /// System meter summary data transfer object (DTO).
    /// </summary>
    public record SystemMeterSummaryDto
    {
        /// <summary>
        /// Current active power output of the system.
        /// </summary>
        /// <remarks>
        /// Positive (+MW) Producing active power (exporting).
        /// Negative (-MW) Consuming active power (importing).
        /// </remarks>
        public ActivePower ActivePower { get; set; }


        /// <summary>
        /// Current reactive power output of the system.
        /// </summary>
        /// <remarks>
        /// Positive(+VAR) Injecting reactive power (capacitive).
        /// Negative(-VAR) Absorbing reactive power (inductive).
        /// </remarks>
        public ReactivePower ReactivePower { get; set; }


        /// <summary>
        /// Current state of charge of the system if it has energy storages.
        /// </summary>
        /// <remarks>
        /// | SoC Range | Status  | Hex color |
        /// | High      | 81–100% | #3B82F6   | 
        /// | Normal    | 20–80%  | #22C55E   | 
        /// | Moderate  | 10–19%  |	#EAB308   | 
        /// | Low       | 5–9%    | #F97316   | 
        /// | Critical  | 0–4%    | #EF4444   | 
        /// </remarks>
        public double? StateOfCharge { get; set; }


        /// <summary>
        /// Current power factor of the system.
        /// </summary>
        /// <remarks>
        /// The value is signed according to the power factor standard (IEEE or IEC) of the system power meter.
        /// </remarks>
        public double PowerFactor { get; set; }


        /// <summary>
        /// Current frequency of the system.
        /// </summary>
        public double Frequency { get; set; }
    }
}

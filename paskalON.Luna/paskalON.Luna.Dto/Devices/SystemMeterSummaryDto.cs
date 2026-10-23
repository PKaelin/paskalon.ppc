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
        public ActivePower ActivePower { get; set; }


        /// <summary>
        /// Current reactive power output of the system.
        /// </summary>
        public ReactivePower ReactivePower { get; set; }


        /// <summary>
        /// Current state of charge of the system if it has energy storages.
        /// </summary>
        public double? StateOfCharge { get; set; }


        /// <summary>
        /// Current power factor of the system.
        /// </summary>
        public double PowerFactor { get; set; }


        /// <summary>
        /// Current frequency of the system.
        /// </summary>
        public double Frequency { get; set; }
    }
}

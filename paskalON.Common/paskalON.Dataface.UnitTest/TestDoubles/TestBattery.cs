// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;

namespace paskalON.Dataface.UnitTest.TestDoubles
{
    /// <summary>
    /// Battery device test double whose properties are updated through registered dataface setters.
    /// </summary>
    internal sealed class TestBattery
    {
        /// <summary>
        /// State of charge in percent.
        /// </summary>
        public double StateOfCharge { get; set; }


        /// <summary>
        /// Active power in watts, null when the device did not report a value.
        /// </summary>
        public float? ActivePower { get; set; }


        /// <summary>
        /// Cell temperature in tenths of a degree Celsius.
        /// </summary>
        public short Temperature { get; set; }


        /// <summary>
        /// Raw device status word.
        /// </summary>
        public ushort StatusWord { get; set; }


        /// <summary>
        /// Number of full charge/discharge cycles.
        /// </summary>
        public int CycleCount { get; set; }


        /// <summary>
        /// True when the DC contactor is closed.
        /// </summary>
        public bool IsContactorClosed { get; set; }


        /// <summary>
        /// Grid frequency in hertz.
        /// </summary>
        public float Frequency { get; set; }


        /// <summary>
        /// Voltage phasor.
        /// </summary>
        public C37PhasorValue VoltagePhasor { get; set; }


        /// <summary>
        /// Current phasor, null when the device did not report a value.
        /// </summary>
        public C37PhasorValue? CurrentPhasor { get; set; }
    }
}

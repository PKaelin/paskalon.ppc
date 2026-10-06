// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Telemetry.UnitTest.TestDoubles
{
    /// <summary>
    /// Battery bank test double that exposes typical device values for metrics registration.
    /// </summary>
    public sealed class TestBatteryBank
    {
        /// <summary>
        /// Total charged energy in watt hours (monotonically increasing).
        /// </summary>
        public long ChargedEnergy { get; set; }


        /// <summary>
        /// Net change of the stored energy in watt hours since the last cycle (may be negative).
        /// </summary>
        public int StoredEnergyDelta { get; set; }


        /// <summary>
        /// State of charge in percent.
        /// </summary>
        public double StateOfCharge { get; set; }


        /// <summary>
        /// Active power in watts, null when the device did not deliver a value.
        /// </summary>
        public double? ActivePower { get; set; }


        /// <summary>
        /// Cell temperature in degrees celsius used for distribution measurements.
        /// </summary>
        public double CellTemperature { get; set; }


        /// <summary>
        /// Whether the battery bank is online (a type not supported by metric instruments).
        /// </summary>
        public bool IsOnline { get; set; }
    }
}

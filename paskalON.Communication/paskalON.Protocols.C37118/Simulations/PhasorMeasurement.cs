// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Simulations
{
    /// <summary>
    /// Represents a phasor PMU measurement.
    /// </summary>
    public sealed class PhasorMeasurement
    {
        /// <summary>
        /// Constructor of <see cref="PhasorMeasurement"/>.
        /// </summary>
        /// <param name="name">Channel name.</param>
        /// <param name="angle">Angle of phasor.</param>
        /// <param name="magnitude">Magnitude of phasor.</param>
        /// <param name="phasorType">Phasor unit type.</param>
        public PhasorMeasurement(string name, float angle, float magnitude, PhasorUnitTypes phasorType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            Name = name;
            Angle = angle;
            Magnitude = magnitude;
            PhasorType = phasorType;
        }


        /// <summary>
        /// Channel name.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Angle of phasor.
        /// </summary>
        public float Angle { get; set; }


        /// <summary>
        /// Magnitude of phasor.
        /// </summary>
        public float Magnitude { get; set; }


        /// <summary>
        /// Phasor unit type.
        /// </summary>
        public PhasorUnitTypes PhasorType { get; set; }
    }
}

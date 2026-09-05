// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Simulations
{
    /// <summary>
    /// Represents an analog PMU measurement.
    /// </summary>
    public sealed class AnalogMeasurement
    {
        /// <summary>
        /// Constructor of <see cref="AnalogMeasurement"/>.
        /// </summary>
        /// <param name="name">Channel name.</param>
        /// <param name="measurement">Measurement value.</param>
        public AnalogMeasurement(string name, float measurement)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            Name = name;
            Measurement = measurement;
        }


        /// <summary>
        /// Channel name.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Measurement value.
        /// </summary>
        public float Measurement { get; set; }
    }
}

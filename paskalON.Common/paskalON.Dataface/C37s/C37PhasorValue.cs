// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// C37 Phasor value.
    /// </summary>
    /// <remarks>
    /// A phasor C37.118 is a complex number representing the magnitude and phase angle of a sinusoidal voltage or current.
    /// </remarks>
    public readonly struct C37PhasorValue
    {
        /// <summary>
        /// Magnitude of the phasor.
        /// </summary>
        public float Magnitude { get; }

        /// <summary>
        /// Angle of the phasor.
        /// </summary>
        /// <remarks>
        /// Angle can be in radian or degrees.
        /// </remarks>
        public float Angle { get; }


        /// <summary>
        /// Constructor of <see cref="C37PhasorValue"/>.
        /// </summary>
        /// <param name="magnitude">Magnitude of the phasor.</param>
        /// <param name="angle"Angle of the phasor.></param>
        public C37PhasorValue(float magnitude, float angle)
        {
            Magnitude = magnitude;
            Angle = angle;
        }
    }
}

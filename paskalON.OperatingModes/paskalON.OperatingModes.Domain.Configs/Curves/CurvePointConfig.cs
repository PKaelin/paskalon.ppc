// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.OperatingModes.Domain.Configs.Curves
{
    public class CurvePointConfig : DomainBase
    {
        /// <summary>
        /// Parent relationship to CurveBaseConfigId Id.
        /// </summary>
        public int CurveBaseConfigId { get; set; }


        /// <summary>
        /// Parent relationship to CurveBaseConfig.
        /// </summary>
        public required CurveBaseConfig CurveBaseConfig { get; set; }


        /// <summary>
        /// The X value of the point configuration.
        /// </summary>
        public double X { get; protected set; }


        /// <summary>
        /// The Y value of the point configuration.
        /// </summary>
        public double Y { get; protected set; }


        /// <summary>
        /// Initialize new instance of <see cref="CurvePointConfig"/>
        /// </summary>
        public CurvePointConfig()
        {
        }


        /// <summary>
        /// Initialize new instance of <see cref="CurvePointConfig"/> with the specific x and y values.
        /// </summary>
        /// <param name="x">The X value of the point configuration.</param>
        /// <param name="y">The Y value of the point configuration.</param>
        public CurvePointConfig(double x, double y)
        {

        }


        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}

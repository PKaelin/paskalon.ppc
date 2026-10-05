// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Maths.Randoms;

namespace paskalON.Maths.Calculuses.Exponents
{
    /// <summary>
    /// An exponential function represents the relationship between an input and output, where we use repeated multiplication 
    /// on an initial value to get the output for any given input. 
    /// Exponential functions can grow or decay very quickly.
    /// </summary>
    public class Exponential2PointFunction : ICalculateOutputFunction
    {
        /// <summary>
        /// Initial value / vertical scaling.
        /// </summary>
        private readonly double _a;


        /// <summary>
        /// Base / growth factor
        /// </summary>
        /// <remarks>
        /// Determines how quickly the function grows or decays.
        /// </remarks>
        private readonly double _b;


        /// <summary>
        /// Definition of exponential point1 X and Y.
        /// </summary>
        public (double X, double Y) Point1 { get; }


        /// <summary>
        /// Definition of exponential point2 X and Y.
        /// </summary>
        public (double X, double Y) Point2 { get; }


        /// <summary>
        /// Adds an offset to f(x).
        /// </summary>
        public double Offset { get; } = 0;


        /// <summary>
        /// Minimum of noise range that gets applied.
        /// </summary>
        public double NoiseMin { get; } = 0;


        /// <summary>
        /// Maximum of noise range that gets applied.
        /// </summary>
        public double NoiseMax { get; } = 0;


        /// <summary>
        /// Constructor of <see cref="Exponential2PointFunction"/>.
        /// </summary>
        /// <param name="point1">Point 1 of the exponential function calculation.</param>
        /// <param name="point2">Point 2 of the exponential function calculation.</param>
        /// <param name="offset">Offset to f(x).</param>
        public Exponential2PointFunction((double X, double Y) point1, (double X, double Y) point2, double offset = 0)
            : this(point1, point2, offset, 0, 0)
        {
        }


        /// <summary>
        /// Constructor of <see cref="Exponential2PointFunction"/>.
        /// </summary>
        /// <param name="point1">Point 1 of the exponential function calculation.</param>
        /// <param name="point2">Point 2 of the exponential function calculation.</param>
        /// <param name="offset">Offset to f(x).</param>
        /// <param name="noiseMin">Minimum of noise range that gets applied.</param>
        /// <param name="noiseMax">Maximum of noise range that gets applied.</param>
        public Exponential2PointFunction((double X, double Y) point1, (double X, double Y) point2, double offset, double noiseMin, double noiseMax)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(noiseMin, noiseMax);

            if (point1.X == point2.X)
            {
                throw new ArgumentException("First and second point cannot have the same x-value");
            }

            if ((point1.Y == 0) || (point2.Y == 0))
            {
                throw new ArgumentException("Point1 Y or Point 2 Y cannot be 0 for an exponential function");
            }

            if (Math.Sign(point1.Y) != Math.Sign(point2.Y))
            {
                throw new ArgumentException("Point1 Y or Point 2 Y must have the same sign");
            }

            Point1 = point1;
            Point2 = point2;
            Offset = offset;
            NoiseMin = noiseMin;
            NoiseMax = noiseMax;

            _b = Math.Pow(Point1.Y / Point2.Y, Math.Pow(Point1.X - Point2.X, -1));
            _a = Point1.Y / Math.Pow(_b, Point1.X);
        }


        /// <summary>
        /// Calculates the output from the piecewise exponential curve from two points.
        /// </summary>
        /// <param name="x">X input.</param>
        /// <returns>"Y Output from the calculated curve.</returns>
        /// <remarks>
        /// Exponential equation from two points (x1, y1) and (x2, y2) 
        /// Formula: f(x) = ab^x
        /// Compute b: y1=ab^x1 and y2=ab^x2 algebra to y1/y2=(ab^x1/ab^x2) to y1/y2=b^(x1-x2) to b=(y1/y2)^((x1-x2)^-1)
        /// Compute a: a=y1/b^x1 = y2/b^x2
        /// x: x-axis, time, is a variable
        /// Limitations: x1 and x2 cannot be equal, y1 and 2 can be the same, y2 cannot be 0, y1 and y2 must have the same sign.
        /// </remarks>
        public double CalculateOutput(double x)
        {
            if (NoiseMin == 0 && NoiseMax == 0)
            {
                return _a * Math.Pow(_b, x) + Offset;
            }

            return _a * Math.Pow(_b, x) + Offset + Random.Shared.NextDoubleInRange(NoiseMin, NoiseMax);
        }


        /// <summary>
        /// Calculates the output from the piecewise exponential curve from two points.
        /// </summary>
        /// <param name="x">X input.</param>
        /// <param name="precision">Precision of the returned value.</param>
        /// <returns>"Y Output from the calculated curve.</returns>
        public double CalculateOutputPrecision(double x, int precision = 3)
        {
            return Math.Round(CalculateOutput(x), precision);
        }
    }
}

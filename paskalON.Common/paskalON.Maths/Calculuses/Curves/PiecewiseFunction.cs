// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Maths.Calculuses.Coordinates;
using paskalON.Maths.Calculuses.Exponents;

namespace paskalON.Maths.Calculuses.Curves
{
    /// <summary>
    /// Class representing a piecewise function, which is defined by a list of piecewise points. 
    /// Each piecewise point specifies the type of function to use for interpolation between points, as well as the X and Y coordinates. 
    /// The class calculates the output for a given input X by determining which piecewise function to use based on the defined points and applying the appropriate interpolation method.
    /// </summary>
    public class PiecewiseFunction : ICalculateOutputFunction
    {
        /// <summary>
        /// Dictionary to store the piecewise functions.
        /// </summary>
        private Dictionary<double, ICalculateOutputFunction> _functions = new Dictionary<double, ICalculateOutputFunction>();


        /// <summary>
        /// Definition of linear points X and Y.
        /// </summary>
        public IReadOnlyList<PiecewisePoint> PiecewisePoints { get; } = new List<PiecewisePoint>();


        /// <summary>
        /// Adds an offset to f(x).
        /// </summary>
        public double Offset { get; } = 0;


        /// <summary>
        /// Constructor of <see cref="PiecewiseFunction"/>.
        /// </summary>
        /// <param name="piecewisePoints">List of piecewise points.</param>
        /// <param name="offset">Offset to apply.</param>
        public PiecewiseFunction(List<PiecewisePoint> piecewisePoints, double offset = 0)
        {
            if ((piecewisePoints == null) || (piecewisePoints.Count == 0))
            {
                throw new ArgumentException("PiecewisePoints cannot be null or empty");
            }

            if (piecewisePoints.Any(pf => pf.Type != PiecewiseFunctionType.LinearPointFunction && pf.Type != PiecewiseFunctionType.Exponential2PointFunction))
            {
                throw new ArgumentException("Piecewise points must be of type LinearPointFunction or Exponential2PointFunction.", nameof(piecewisePoints));
            }

            List<PiecewisePoint> piecewisePointsList = new List<PiecewisePoint>(piecewisePoints);
            piecewisePointsList.Sort((l1, l2) => l1.X.CompareTo(l2.X));
            PiecewisePoints = piecewisePointsList.AsReadOnly();

            for (int i = 1; i < PiecewisePoints.Count; i++)
            {
                if (PiecewisePoints[i - 1].X == PiecewisePoints[i].X)
                {
                    throw new ArgumentException("Piecewise points cannot have duplicate x-values.", nameof(piecewisePoints));
                }
            }

            Offset = offset;
            InitializeFunctions();
        }


        /// <summary>
        /// Calculates the output for a given input X by determining which piecewise function to use based on the defined points and applying the appropriate interpolation method.
        /// </summary>
        /// <param name="x">Input value for which to calculate the output.</param>
        /// <returns>Output value corresponding to the input X.</returns>
        public double CalculateOutput(double x)
        {
            if (PiecewisePoints.Count == 1)
            {
                return PiecewisePoints[0].Y + Offset;
            }

            // First point is the minimum, last point is the maximum. If x is outside of the range, return the corresponding y value.
            if (x < PiecewisePoints[0].X)
            {
                return PiecewisePoints[0].Y + Offset;
            }

            // Last point is the maximum. If x is outside of the range, return the corresponding y value.
            if (x >= PiecewisePoints[^1].X)
            {
                return PiecewisePoints[^1].Y + Offset;
            }

            double key = PiecewisePoints[0].X;

            for (int i = 1; i < PiecewisePoints.Count; i++)
            {
                if (x < PiecewisePoints[i].X)
                {
                    break;
                }

                key = PiecewisePoints[i].X;
            }

            return _functions[key].CalculateOutput(x);
        }


        /// <summary>
        /// Calculates the output for a given input X by determining which piecewise function to use based on the defined points and applying the appropriate interpolation method.
        /// </summary>
        /// <param name="x">Input value for which to calculate the output.</param>
        /// <param name="precision">Precision of the returned value.</param>
        /// <returns>Output value corresponding to the input X.</returns>
        public double CalculateOutputPrecision(double x, int precision = 3)
        {
            return Math.Round(CalculateOutput(x), precision);
        }


        /// <summary>
        /// Initializes the piecewise functions based on the defined piecewise points.
        /// </summary>
        private void InitializeFunctions()
        {
            _functions.Clear();

            for (int i = 0; i < PiecewisePoints.Count; i++)
            {

                if (_functions.ContainsKey(PiecewisePoints[i].X) == false)
                {
                    (double x, double y) endpoint;

                    if (PiecewisePoints.Count > i + 1)
                    {
                        endpoint = (PiecewisePoints[i + 1].X, PiecewisePoints[i + 1].Y);
                    }
                    else
                    {
                        endpoint = (PiecewisePoints[i].X, PiecewisePoints[i].Y);
                    }

                    if (i == PiecewisePoints.Count - 1)
                    {
                        continue;
                    }

                    if (PiecewisePoints[i].Type == PiecewiseFunctionType.LinearPointFunction)
                    {
                        List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(PiecewisePoints[i].X, PiecewisePoints[i].Y), new LinearPoint(endpoint.x, endpoint.y) };
                        _functions.Add(PiecewisePoints[i].X, new LinearPointFunction(points, Offset, PiecewisePoints[i].NoiseMin, PiecewisePoints[i].NoiseMax));
                    }
                    else if (PiecewisePoints[i].Type == PiecewiseFunctionType.Exponential2PointFunction)
                    {
                        _functions.Add(PiecewisePoints[i].X, new Exponential2PointFunction((PiecewisePoints[i].X, PiecewisePoints[i].Y), (endpoint.x, endpoint.y),
                            Offset, PiecewisePoints[i].NoiseMin, PiecewisePoints[i].NoiseMax));
                    }
                }
            }
        }


        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"{nameof(PiecewiseFunction)} Offset: {Offset} Points: {string.Join(',', PiecewisePoints.Select(p => p.ToString()))}";
        }

    }
}

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
        public List<PiecewisePoint> PiecewisePoints { get; private set; } = new List<PiecewisePoint>();


        /// <summary>
        /// Adds an offset to f(x).
        /// </summary>
        public double Offset { get; set; } = 0;


        /// <summary>
        /// Precision to round the output to.
        /// </summary>
        public int Precision { get; set; }


        /// <summary>
        /// Constructor of <see cref="PiecewiseFunction"/>.
        /// </summary>
        /// <param name="piecewisePoints">List of piecewise points.</param>
        public PiecewiseFunction(List<PiecewisePoint> piecewisePoints, double offset = 0, int precision = 3)
        {
            if ((piecewisePoints == null) || (piecewisePoints.Count == 0))
            {
                throw new ArgumentException("PiecewisePoints cannot be null or empty");
            }

            PiecewisePoints = piecewisePoints;
            PiecewisePoints.Sort((l1, l2) => l1.X.CompareTo(l2.X));
            Offset = offset;
            InitializeFunctions();
            Precision = precision;
        }


        /// <summary>
        /// Calculates the output for a given input X by determining which piecewise function to use based on the defined points and applying the appropriate interpolation method.
        /// </summary>
        /// <param name="x">Input value for which to calculate the output.</param>
        /// <returns>Output value corresponding to the input X.</returns>
        public double CalculateOutput(double x)
        {
            double key = _functions.Keys.Aggregate((k1, k2) => Math.Abs(k1 - x) < Math.Abs(k2 - x) ? (k1 > x && x > 0 ? k2 : k1) : (k2 > x ? k1 : k2));

            if (key == 0 && x <= _functions.Keys.FirstOrDefault())
            {
                key = _functions.Keys.FirstOrDefault();
            }
            else if (key == 0 && x >= _functions.Keys.LastOrDefault())
            {
                key = _functions.Keys.LastOrDefault();
            }

            if ((key == 0) && (key == 0 && _functions.Keys.FirstOrDefault() != 0))
            {
                return 0;
            }

            return _functions[(double)key].CalculateOutput(x);
        }


        /// <summary>
        /// Calculates the output for a given input X by determining which piecewise function to use based on the defined points and applying the appropriate interpolation method.
        /// </summary>
        /// <param name="x">Input value for which to calculate the output.</param>
        /// <returns>Output value corresponding to the input X.</returns>
        public double CalculateOutputPrecision(double x)
        {
            return Math.Round(CalculateOutput(x), Precision);
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

                    if (PiecewisePoints[i].Type == PiecewiseFunctionType.LinearPointFunction)
                    {
                        List<LinearPoint> points = new List<LinearPoint> { new(PiecewisePoints[i].X, PiecewisePoints[i].Y), new(endpoint.x, endpoint.y) };
                        _functions.Add(PiecewisePoints[i].X, new LinearPointFunction(points, Offset, PiecewisePoints[i].NoiseMin, PiecewisePoints[i].NoiseMax, PiecewisePoints[i].Precision));
                    }
                    else if ((PiecewisePoints[i].Type == PiecewiseFunctionType.Exponential2PointFunction) && (i < PiecewisePoints.Count - 1))
                    {
                        _functions.Add(PiecewisePoints[i].X, new Exponential2PointFunction((PiecewisePoints[i].X, PiecewisePoints[i].Y), (endpoint.x, endpoint.y),
                            Offset, PiecewisePoints[i].NoiseMin, PiecewisePoints[i].NoiseMax, PiecewisePoints[i].Precision));
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

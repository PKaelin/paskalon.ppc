using Microsoft.Extensions.Logging;

namespace paskalON.Maths.Calculuses.Coordinates
{
    /// <summary>
    /// Class for calculating the Euclidean distance between two points in one or more dimensions.
    /// </summary>
    public class EuclideanDistance
    {
        /// <summary>
        /// ILogger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<EuclideanDistance> _logger;


        /// <summary>
        /// Constructor that initializes the logger for the EuclideanDistance class.
        /// </summary>
        /// <param name="logger">Logger instance for the EuclideanDistance class.</param>
        public EuclideanDistance(ILogger<EuclideanDistance> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Calculates one dimension two point Euclidean distance.
        /// Derived from the Pythagorean theorem, it measures the shortest path connecting two coordinates.
        /// </summary>
        /// <param name="x1">Point x1</param>
        /// <param name="x2">Point x2</param>
        /// <returns>Euclidean Distance</returns>
        public double GetEuclideanDistance(double x1, double x2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2));
        }


        /// <summary>
        /// Calculates two dimension two point Euclidean distance
        /// </summary>
        /// <param name="x1">Point x1</param>
        /// <param name="y1">Point y1</param>
        /// <param name="x2">Point x2</param>
        /// <param name="y2">Point y2</param>
        /// <returns>Euclidean Distance</returns>
        public double GetEuclideanDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }


        /// <summary>
        /// Calculates the Euclidean distance between to RGB (red, green blue) values 
        /// </summary>
        /// <param name="r1">Red 1</param>
        /// <param name="g1">Green 1</param>
        /// <param name="b1">Blue 1</param>
        /// <param name="r2">Red 2</param>
        /// <param name="g2">Green 2</param>
        /// <param name="b2">Blue 2</param>
        /// <returns>Euclidean Distance</returns>
        public double GetRgbEuclideanDistance(double r1, double g1, double b1, double r2, double g2, double b2)
        {
            return Math.Sqrt(Math.Pow(r2 - r1, 2) + Math.Pow(g2 - g1, 2) + Math.Pow(b2 - b1, 2));
        }


        /// <summary>
        /// Multidimensional Euclidian Distance with integer parameters. 
        /// </summary>
        /// <remarks>
        /// Non-Generic method for performance reasons.
        /// </remarks>
        /// <param name="p1">Multidimensional point one.</param>
        /// <param name="p2">Multidimensional point two.</param>
        /// <returns>Euclidean Distance</returns>
        public double GetEuclideanDistance(int[] p1, int[] p2)
        {
            double sum = 0;

            if (p1 == null || p2 == null)
            {
                return double.MaxValue;
            }

            if (p1.Length != p2.Length)
            {
                _logger.LogError("{MethodName} length of parameters are not equal. P1: {P1s} - P2: {P2s}", nameof(GetEuclideanDistance), string.Join(',', p1.Select(op1 => op1)), string.Join(',', p2.Select(op2 => op2)));

                return double.MaxValue;
            }

            // Simple approach
            for (int i = 0; i < p1.Length; i++)
            {
                sum += Math.Pow(p1[i] - p2[i], 2);
            }

            return Math.Sqrt(sum);
        }


        /// <summary>
        /// Multidimensional Euclidian Distance with integer parameters. 
        /// </summary>
        /// <remarks>
        /// Non-Generic method for performance reasons.
        /// </remarks>
        /// <param name="p1">Multidimensional point.</param>
        /// <param name="pl">List of multidimensional points.</param>
        /// <returns>Euclidean Distance</returns>
        public (int[]?, double) GetEuclideanDistance(int[] p1, IEnumerable<int[]> pl)
        {
            double distance;
            double closestDistance = double.MaxValue;
            int[]? closestVector = null;

            foreach (int[] vector in pl)
            {
                distance = GetEuclideanDistance(p1, vector);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestVector = vector;
                }
            }

            return (closestVector, closestDistance);
        }



        /// <summary>
        /// Multidimensional Squared Euclidian Distance with integer parameters. 
        /// </summary>
        /// <remarks>
        /// Non-Generic method for performance reasons.
        /// More performant version to calculated the shortest distance but leave out the square root operation for performance.
        /// </remarks>
        /// <param name="p1">Multidimensional point one.</param>
        /// <param name="p2">Multidimensional point two.</param>
        /// <returns>Squared Euclidean Distance</returns>
        public double GetSquaredEuclideanDistance(int[] p1, int[] p2)
        {
            double sum = 0;

            if (p1 == null || p2 == null)
            {
                return double.MinValue;
            }

            if (p1.Length != p2.Length)
            {
                _logger.LogError("{MethodName} length of parameters are not equal. P1: {P1s} - P2: {P2s}", nameof(GetEuclideanDistance), string.Join(',', p1.Select(op1 => op1)), string.Join(',', p2.Select(op2 => op2)));

                return double.MinValue;
            }

            // Simple approach
            for (int i = 0; i < p1.Length; i++)
            {
                sum += Math.Pow(p1[i] - p2[i], 2);
            }

            return sum;
        }


        /// <summary>
        /// Get closest vector from file using Euclidean distance. The file should contain one vector per line with comma separated values.
        /// </summary>
        /// <param name="input">Input vector</param>
        /// <param name="filename">File name containing a list of vectors</param>
        /// <param name="paging">With very big files paging is recommended instead of loading it all into memory</param>
        /// <returns>Closest vector from file in relation to the input vector</returns>
        /// <exception cref="ApplicationException"></exception>
        public (int[]?, double) GetClosestVectorFromFile(int[] input, string filename, int paging = 10000)
        {
            if (input == null)
            {
                return (null, double.MaxValue);
            }

            if (File.Exists(filename) == false)
            {
                throw new ApplicationException($"File {filename} does not exist");
            }

            using StreamReader reader = new StreamReader(filename);
            (int[]? vector, double distance) temp;
            (int[]? vector, double distance) closest = (null, double.MaxValue);
            List<int[]> vectors = new List<int[]>();

            while (reader.EndOfStream == false)
            {
                string? vectorString = reader.ReadLine();

                if (vectorString != null)
                {
                    vectors.Add(Array.ConvertAll<string, int>(vectorString.Split(',', StringSplitOptions.TrimEntries), int.Parse));
                }

                if (vectors.Count == paging || reader.Peek() < 0)
                {
                    temp = GetEuclideanDistance(input, vectors);

                    if (temp.distance < closest.distance)
                    {
                        closest = temp;
                    }

                    vectors.Clear();
                }
            }

            return closest;
        }
    }
}

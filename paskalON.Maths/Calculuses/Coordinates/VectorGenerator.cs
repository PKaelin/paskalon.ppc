using Microsoft.Extensions.Logging;

namespace paskalON.Maths.Calculuses.Coordinates
{
    /// <summary>
    /// Class responsible to generate random vectors with specified dimensions and value ranges, and to create files containing these vectors.
    /// </summary>
    public class VectorGenerator
    {
        /// <summary>
        /// Logger for logging the parameters used in vector generation and file creation.
        /// </summary>
        private readonly ILogger<VectorGenerator> _logger;


        /// <summary>
        /// Constructor that initializes the logger for the VectorGenerator class.
        /// </summary>
        /// <param name="logger">Logger instance for the VectorGenerator class</param>
        public VectorGenerator(ILogger<VectorGenerator> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Generates a list of random vectors with specified number of rows, dimensions, and value ranges.
        /// </summary>
        /// <param name="rows">Number of vectors to generate</param>
        /// <param name="dimension">Number of dimensions for each vector</param>
        /// <param name="min">Minimum value for each dimension</param>
        /// <param name="max">Maximum value for each dimension</param>
        /// <returns>List of generated vectors</returns>
        public List<int[]> CreateMultidimensionalVector(int rows, int dimension, int min, int max)
        {
            Random ran = new Random();
            List<int[]> vectors = new List<int[]>();

            for (int r = 0; r < rows; r++)
            {
                List<int> vector = new List<int>();

                for (int d = 0; d < dimension; d++)
                {
                    vector.Add(ran.Next(min, max));
                }

                vectors.Add(vector.ToArray());
            }

            _logger.LogError("{name} rows: {rows} dimensions: {dimension} min:{min} max:{max}", nameof(CreateMultidimensionalVector), rows, dimension, min, max);

            return vectors;
        }


        /// <summary>
        /// Creates a file containing random vectors with specified number of rows, dimensions, and value ranges. Each vector is written as a comma-separated line in the file.
        /// </summary>
        /// <param name="filename">Name of the file to create</param>
        /// <param name="rows">Number of vectors to generate</param>
        /// <param name="dimension">Number of dimensions for each vector</param>
        /// <param name="min">Minimum value for each dimension</param>
        /// <param name="max">Maximum value for each dimension</param>
        public void CreateMultidimensionalVectorFile(string filename, int rows, int dimension, int min, int max)
        {
            Random ran = new Random();

            using (StreamWriter sw = new StreamWriter(filename))
            {
                for (int r = 0; r < rows; r++)
                {
                    for (int d = 0; d < dimension; d++)
                    {
                        if (d == dimension - 1)
                        {
                            sw.WriteLine(ran.Next(min, max));
                        }
                        else
                        {
                            sw.Write($"{ran.Next(min, max)},");
                        }
                    }
                }
            }

            _logger.LogError("{name} file: {filename} rows: {rows} dimensions: {dimension} min:{min} max:{max}", nameof(CreateMultidimensionalVectorFile), filename, rows, dimension, min, max);
        }
    }
}

using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Maths.Calculuses.Coordinates;

namespace paskalON.Maths.UnitTest.Calculuses.Coordinates
{
    [TestClass]
    public class VectorGeneratorTest
    {
        [TestMethod]
        [DataRow(3, 3, 0, 10)]
        [DataRow(3, 3, 10, 1000)]
        [DataRow(5, 5, 10, 1000)]
        [DataRow(10, 10, 100, 10000)]
        public void CreateMultidimensionalVector(int rows, int dimension, int min, int max)
        {
            VectorGenerator generator = new VectorGenerator(NullLogger<VectorGenerator>.Instance);
            List<int[]> vectors = generator.CreateMultidimensionalVector(rows, dimension, min, max);

            Assert.IsNotNull(vectors);
            Assert.HasCount(rows, vectors);

            for (int row = 0; row < rows; row++)
            {
                int dimmAssert = 0;

                for (int dim = 0; dim < dimension; dim++)
                {
                    Assert.IsTrue(vectors[row][dim] >= min && vectors[row][dim] <= max);
                    dimmAssert++;
                }

                Assert.AreEqual(dimension, dimmAssert);
                dimmAssert = 0;
            }
        }
    }
}

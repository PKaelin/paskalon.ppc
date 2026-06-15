using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Maths.Calculuses.Coordinates;

namespace paskalON.Maths.UnitTest.Calculuses.Coordinates
{
    [TestClass]
    public class EuclideanDistanceTest
    {
        [TestMethod]
        [DataRow(1, 2, 1)]
        [DataRow(1, 5, 4)]
        [DataRow(5, 5, 0)]
        [DataRow(1, 22, 21)]
        [DataRow(5, 22, 17)]
        [DataRow(5, 1, 4)]
        [DataRow(10, 1, 9)]
        public void EuclideanDistanceOneDimensionTwoPointsTest(double x1, double x2, double expected)
        {
            EuclideanDistance euclidean = new EuclideanDistance(NullLogger<EuclideanDistance>.Instance);

            double distance = euclidean.GetEuclideanDistance(x1, x2);
            Assert.AreEqual(expected, Math.Round(distance, 5));
        }



        [TestMethod]
        [DataRow(1, 1, 2, 2, 1.41421)]
        [DataRow(1, 1, 5, 5, 5.65685)]
        [DataRow(5, 5, 5, 5, 0)]
        [DataRow(5, 5, 22, 22, 24.04163)]
        [DataRow(5, 5, 1, 1, 5.65685)]
        public void EuclideanDistanceTwoDimensionTwoPointsTest(double x1, double y1, double x2, double y2, double expected)
        {
            EuclideanDistance euclidean = new EuclideanDistance(NullLogger<EuclideanDistance>.Instance);

            double distance = euclidean.GetEuclideanDistance(x1, y1, x2, y2);
            Assert.AreEqual(expected, Math.Round(distance, 5));
        }



        [TestMethod]
        [DataRow(255, 0, 0, 255, 26, 26, 36.76955)]     // ff0000: 255,0,0 / ff1a1a: 255,26,26
        [DataRow(255, 0, 0, 102, 51, 0, 161.27616)]     // ff0000: 255,0,0 / 663300: 102,51,0 (brown)
        [DataRow(255, 0, 0, 255, 153, 51, 161.27616)]   // ff0000: 255,0,0 / ff9933: 255,153,51 (orange)
        [DataRow(255, 0, 0, 128, 0, 128, 180.31362)]    // ff0000: 255,0,0 / 800080: 128,0,128 (purple)
        [DataRow(0, 0, 255, 0, 0, 230, 25)]             // ff0000: 0,0,255 / 0000e6: 0,0,230 (blueish)
        [DataRow(0, 0, 255, 77, 77, 255, 108.89444)]    // ff0000: 0,0,255 / 4d4dff: 77,77,255 (blueish)
        public void EuclideanDistanceRgbTest(double r1, double g1, double b1, double r2, double g2, double b2, double expected)
        {
            EuclideanDistance euclidean = new EuclideanDistance(NullLogger<EuclideanDistance>.Instance);

            double distance = euclidean.GetRgbEuclideanDistance(r1, g1, b1, r2, g2, b2);
            Assert.AreEqual(expected, Math.Round(distance, 5));
        }



        [TestMethod]
        [DataRow(new int[] { 1, 1, 1 }, new int[] { 5, 5, 5 }, 6.92820)]
        [DataRow(new int[] { 1, 1, 1 }, new int[] { 123, 155, 83 }, 212.89434)]
        [DataRow(new int[] { 15, 24, 36 }, new int[] { 123, 155, 83 }, 176.16470)]
        [DataRow(new int[] { 100, 200, 300 }, new int[] { 300, 200, 100 }, 282.84271)]
        [DataRow(new int[] { 100, 200, 300 }, new int[] { 52, 21, 96 }, 275.61023)]
        [DataRow(new int[] { 15, 24, 36, 12 }, new int[] { 123, 155, 83, 55 }, 181.33670)]
        [DataRow(new int[] { 15, 24, 36, 12, 2, 0 }, new int[] { 123, 155, 83, 55, 5, 12 }, 181.75808)]
        [DataRow(new int[] { 15, 24, 36, 12, 2, 0, 1234 }, new int[] { 123, 155, 83, 55, 5, 12, 5678 }, 4447.71537)]
        [DataRow(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }, new int[] { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 318.32373)]
        [DataRow(new int[] { 1, 1, 1 }, new int[] { 5 }, double.MaxValue)]
        [DataRow(new int[] { 1, }, new int[] { 5, 5, 5 }, double.MaxValue)]
        [DataRow(new int[] { 1, 1, 1 }, null, double.MaxValue)]
        [DataRow(null, new int[] { 1, 1, 1 }, double.MaxValue)]
        public void EuclideanDistanceMultiDimensionsIntTest(int[] p1, int[] p2, double expected)
        {
            EuclideanDistance euclidean = new EuclideanDistance(NullLogger<EuclideanDistance>.Instance);

            double distance = euclidean.GetEuclideanDistance(p1, p2);
            Assert.AreEqual(expected, Math.Round(distance, 5));
        }


        [TestMethod]
        [DataRow(new int[] { 1 }, new int[] { 5 }, 16)]
        [DataRow(new int[] { 1, 1 }, new int[] { 5, 5 }, 32)]
        [DataRow(new int[] { 1, 1, 1 }, new int[] { 5, 5, 5 }, 48)]
        [DataRow(new int[] { 10, 20, 30 }, new int[] { 5, 5, 5 }, 875)]
        public void SquaredEuclideanDistanceMultiDimensionsIntTest(int[] p1, int[] p2, double expected)
        {
            EuclideanDistance euclidean = new EuclideanDistance(NullLogger<EuclideanDistance>.Instance);

            double distance = euclidean.GetSquaredEuclideanDistance(p1, p2);
            Assert.AreEqual(expected, Math.Round(distance, 5));
        }


        [TestMethod]
        public void EuclideanDistanceMultiDimensionsListTest()
        {
            EuclideanDistance euclidean = new EuclideanDistance(NullLogger<EuclideanDistance>.Instance);
            List<int[]> list = new List<int[]>
            {
                new[] { 190, 145, 882 },
                new[] { 487, 882, 168 },
                new[] { 840, 329, 369 },
                new[] { 243, 434, 218 },
                new[] { 366, 593, 383 }
            };

            (int[]? vector, double distance) closest = euclidean.GetEuclideanDistance(new int[] { 200, 400, 190 }, list);

            Assert.IsNotNull(closest.vector);
            Assert.AreEqual(61.55485, Math.Round(closest.distance, 5));
            Assert.AreEqual(list[3], closest.vector);
        }
    }
}
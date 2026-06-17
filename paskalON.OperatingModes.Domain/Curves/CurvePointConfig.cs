using paskalON.Domains;

namespace paskalON.OperatingModes.Domain.Curves
{
    public class CurvePointConfig : DomainBase
    {
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

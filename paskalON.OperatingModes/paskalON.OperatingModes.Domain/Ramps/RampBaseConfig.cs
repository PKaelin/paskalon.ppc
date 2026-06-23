using paskalON.Domains;

namespace paskalON.OperatingModes.Domain.Ramps
{
    /// <summary>
    /// Configuration for base for ramp ramp models.
    /// </summary>
    public class RampBaseConfig : DomainBase
    {
        /// <summary>
        /// Ramp time in seconds between enabling the operating mode and start of ramp according to model.
        /// </summary>
        public int RampTimeSeconds
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }
    }
}

using paskalON.Domains;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Configuration class for the system.
    /// </summary>
    public class SystemConfig : DomainBase
    {
        /// <summary>
        /// Gets or sets the system reference frequency in Hertz.
        /// </summary>
        public double ReferenceFrequency
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Gets or sets the system reference voltage in Volts.
        /// </summary>
        public double ReferenceVoltage
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Gets or sets the systems maximum voltage nameplate.
        /// </summary>
        /// <remarks>
        /// Maximum voltage nameplate refers to the highest operating voltage a system can continuously and safely operate.
        /// Most equipment are designed to operate safely with a voltage variation of +/- 10% from the rated nameplate.
        /// </remarks>
        public double NameplateMaximumVoltage
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Gets or sets the systems minimum voltage nameplate.
        /// </summary>
        public double NameplateMinimumVoltage
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Gets or sets the systems maximum current nameplate.
        /// </summary>
        /// <remarks>
        public double NameplateMaximumCurrent
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Gets or sets the systems minimum current nameplate.
        /// </summary>
        public double NameplateMinimumCurrent
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


    }
}

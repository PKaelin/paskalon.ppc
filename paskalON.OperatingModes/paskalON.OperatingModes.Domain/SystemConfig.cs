using paskalON.Domains;
using System.ComponentModel.DataAnnotations;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Configuration class for the system.
    /// </summary>
    public class SystemConfig : DomainBase
    {
        /// <summary>
        /// Operating mode type.
        /// </summary>
        /// <remarks>
        /// Though this is a flag this operating mode system should be configured to only serve one type.
        /// </remarks>
        [Required]
        public OperatingModeType Type
        {
            get;
            set
            {
                int v = (int)value;
                if (Enum.IsDefined(typeof(OperatingModeType), value) == false) throw new ArgumentException("Only one type per operating mode system is allowed.");
                field = value;
            }
        }


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

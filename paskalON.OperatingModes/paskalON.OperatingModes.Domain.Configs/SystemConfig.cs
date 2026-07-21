// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.OperatingModes.Domain.Configs
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
        public required OperatingModeType Type
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
        /// System reference frequency in Hertz.
        /// </summary>
        public required double ReferenceFrequency
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// System reference voltage in Volts.
        /// </summary>
        public double ReferenceVoltage
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems maximum voltage nameplate.
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
        /// Systems minimum voltage nameplate.
        /// </summary>
        public double NameplateMinimumVoltage
        {
            get { return field; }
            set { field = value; }
        }


        /// <summary>
        /// Systems maximum current nameplate.
        /// </summary>
        /// <remarks>
        public double NameplateMaximumCurrent
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems minimum current nameplate.
        /// </summary>
        public double NameplateMinimumCurrent
        {
            get { return field; }
            set { field = value; }
        }


        /// <summary>
        /// Systems maximum active power nameplate in kilo watt.
        /// </summary>
        /// <remarks>
        public double NameplateMaximumActivePowerKiloWatt
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems minimum active power nameplate in kilo wat.
        /// </summary>
        /// <remarks>
        public double NameplateMinimumActivePowerKiloWatt
        {
            get { return field; }
            set { field = value; }
        }


        /// <summary>
        /// Systems maximum reactive power nameplate in kilo vars.
        /// </summary>
        /// <remarks>
        public double NameplateMaximumReactivePowerKiloVars
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems minimum reactive power nameplate in kilo vars.
        /// </summary>
        /// <remarks>
        public double NameplateMinimumReactivePowerKiloVars
        {
            get { return field; }
            set { field = value; }
        }
    }
}

using paskalON.Communication.Protocols.Modbus.Enums;
using paskalON.PhysicalUnits;

namespace paskalON.Communication.Protocols.Modbus.Configurations
{
    /// <summary>
    /// Stores a single entry in the ModbusRegisterMap. Contains the Modbus port index,
    /// scaling factor and the register format. This is a nullable struct and so is not
    /// a child configuration.
    /// </summary>
    public class ModbusRegisterMapEntryConfig
    {
        /// <summary>
        /// Primary Id
        /// </summary>
        public int ModbusRegisterMapEntryConfigId { get; set; }



        /// <summary>
        /// Index property.
        /// </summary>
        public int Index { get; set; }


        /// <summary>
        /// Modbus register format. String is parable to ModbusCommon.ModbusRegisterFormat.
        /// </summary>
        public ModbusRegisterFormat? ModbusRegisterFormat { get; set; }


        /// <summary>
        /// Scale property.
        /// </summary>
        public double Scale { get; set; }


        /// <summary>
        /// Individual Offset property.
        /// </summary>
        public int IndividualOffset { get; set; }


        /// <summary>
        /// Metric unit prefix.
        /// </summary>
        public MetricPrefix? UnitPrefix { get; set; }


        /// <summary>
        /// Calls Equals.
        /// </summary>
        /// <param name="obj1">First object to compare.</param>
        /// <param name="obj2">Second object to compare.</param>
        /// <returns>True if the first object equals the second object.</returns>
        /// <remarks>Must be overridden for code analysis because we override Equals on a value type.</remarks>
        public static bool operator ==(ModbusRegisterMapEntryConfig obj1, object obj2) => obj1.Equals(obj2);


        /// <summary>
        /// Calls Equals. Returns the conjugate.
        /// </summary>
        /// <param name="obj1">First object to compare.</param>
        /// <param name="obj2">Second object to compare.</param>
        /// <returns>True if the first object does not equal the second object.</returns>
        /// <remarks>Must be overridden for code analysis because we override Equals on a value type.</remarks>
        public static bool operator !=(ModbusRegisterMapEntryConfig obj1, object obj2) => !obj1.Equals(obj2);


        /// <summary>
        /// Overriding base Equals call.
        /// </summary>
        /// <param name="obj">Object to compare to, should be a ModbusRegisterMap. </param>
        /// <returns>True if obj is equal to this. False otherwise.</returns>
        public override bool Equals(object? obj)
        {
            return obj is ModbusRegisterMapEntryConfig && Equals((ModbusRegisterMapEntryConfig)obj);
        }


        /// <summary>
        /// Equals function for two explicit ModbusRegisterMapEntry objects.
        /// </summary>
        /// <param name="other">ModbusRegisterMap being compared to.</param>
        /// <returns>True if other is equal to this. False otherwise.</returns>
        public bool Equals(ModbusRegisterMapEntryConfig other)
        {
            return (Index == other.Index) && (Math.Abs(Scale - other.Scale) < double.Epsilon) &&
                (ModbusRegisterFormat == null ? false : ModbusRegisterFormat.Equals(other.ModbusRegisterFormat));
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return new { Index, ModbusRegisterFormat, Scale }.GetHashCode();
        }

    }
}

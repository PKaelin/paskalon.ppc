// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.PhysicalUnits;

namespace paskalON.Communication.Protocols.Modbus.Configurations
{
    /// <summary>
    /// Stores a single entry in the ModbusRegisterMap. Contains the Modbus port index,
    /// scaling factor and the register format. Uses value equality over its mapping values.
    /// </summary>
    public class ModbusRegisterMapEntryConfig : IEquatable<ModbusRegisterMapEntryConfig>
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
        public ModbusDataType? ModbusRegisterFormat { get; set; }


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
        /// Determines whether two entries are equal. Null-safe on both sides.
        /// </summary>
        /// <param name="left">
        /// First entry to compare.
        /// </param>
        /// <param name="right">
        /// Second entry to compare.
        /// </param>
        /// <returns>
        /// True if both entries are null or have equal values; otherwise false.
        /// </returns>
        public static bool operator ==(ModbusRegisterMapEntryConfig? left, ModbusRegisterMapEntryConfig? right)
        {
            return object.Equals(left, right);
        }


        /// <summary>
        /// Determines whether two entries are not equal. Null-safe on both sides.
        /// </summary>
        /// <param name="left">
        /// First entry to compare.
        /// </param>
        /// <param name="right">
        /// Second entry to compare.
        /// </param>
        /// <returns>
        /// True if the entries are not equal; otherwise false.
        /// </returns>
        public static bool operator !=(ModbusRegisterMapEntryConfig? left, ModbusRegisterMapEntryConfig? right)
        {
            return (left == right) is false;
        }


        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as ModbusRegisterMapEntryConfig);
        }


        /// <summary>
        /// Determines whether the specified entry has the same mapping values as this instance.
        /// </summary>
        /// <param name="other">
        /// Entry to compare with this instance.
        /// </param>
        /// <returns>
        /// True if <paramref name="other"/> has equal <see cref="Index"/>, <see cref="ModbusRegisterFormat"/>,
        /// <see cref="Scale"/>, <see cref="IndividualOffset"/> and <see cref="UnitPrefix"/>; otherwise false.
        /// </returns>
        /// <remarks>
        /// The database Id is intentionally not compared.
        /// </remarks>
        public bool Equals(ModbusRegisterMapEntryConfig? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (Index == other.Index)
                && (ModbusRegisterFormat == other.ModbusRegisterFormat)
                && Scale.Equals(other.Scale)
                && (IndividualOffset == other.IndividualOffset)
                && (UnitPrefix == other.UnitPrefix);
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The hash code is derived from mutable properties. Do not modify an instance while it is stored in a hash-based collection.
        /// </remarks>
        public override int GetHashCode()
        {
            return HashCode.Combine(Index, ModbusRegisterFormat, Scale, IndividualOffset, UnitPrefix);
        }

    }
}

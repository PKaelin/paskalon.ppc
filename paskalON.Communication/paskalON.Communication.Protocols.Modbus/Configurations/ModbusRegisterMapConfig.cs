
namespace paskalON.Communication.Protocols.Modbus.Configurations
{
    public abstract class ModbusRegisterMapConfig
    {
        /// <summary>
        /// Name of the configuration.
        /// </summary>
        public string? Name { get; set; }


        /// <summary>
        /// Index of the starting holding register of this register map.
        /// </summary>        
        public ushort StartingHoldingRegister { get; set; } = 40001;


        /// <summary>
        /// Index of the starting input register of this register map.
        /// </summary>
        public ushort StartingInputRegister { get; set; } = 30001;


        /// <summary>
        /// Ihe index of the starting discrete input of this register map.
        /// </summary>
        public ushort StartingDiscreteInput { get; set; } = 10001;


        /// <summary>
        /// Index of the starting coil of this register map.
        /// </summary>
        public ushort StartingCoil { get; set; } = 1;


        private List<ModbusRegisterMapPollingRangeConfig>? pollingRange;
        /// <summary>
        /// The set of PollingRanges attached to this RegisterMap.  Always return an array so that
        /// callers don't have to check for null, just spins an empty enumerable.
        /// </summary>
        public virtual List<ModbusRegisterMapPollingRangeConfig> PollingRange { get; set; } = new List<ModbusRegisterMapPollingRangeConfig>();


        /// <summary>
        /// Return an enumeration of all the point indices defined in this Register Map.
        /// </summary>
        /// <returns>Enumerable of Modbus Index values.</returns>
        public abstract IEnumerable<int> Indexes();


        /// <summary>
        /// Check that the given index is covered by at least one of the PollingRanges. If no PollingRanges exist
        /// then the default behavior is to cover all the indices.  So return true in that instance as well.
        /// </summary>
        /// <param name="index">The Modbus Index to check.</param>
        /// <returns>True iff index will be covered by the set of PollingRanges.</returns>
        public bool PollingIncludes(int index)
        {
            return (PollingRange.Count() == 0) || PollingRange.Any(o => o.Includes(index));
        }


        /// <summary>
        /// Validation override, checks that PollingRanges are valid and that all Modbus Indices associated with this RegisterMap are
        /// covered by some index range. Also checks that indexes of all points are within valid register index range for our
        /// implementation of Modbus - [1, 65535]. Finally it checks that there are no duplicate indices on this device.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">If an index is less than 1 or greater than 65535.</exception>
        /// <exception cref="InvalidOperationException">If an index is not inside of a configured polling range or is duplicated.</exception>
        public virtual void Validate()
        {
            const int MinModbusRegister = 1;
            const int MaxModbusRegister = 65535;

            HashSet<int> uniqueRegisters = new HashSet<int>();

            foreach (int index in Indexes())
            {
                if (!PollingIncludes(index))
                {
                    throw new InvalidOperationException($"Invalid ModbusRegisterMap, Index {index} not covered by associated PollingRange elements");
                }

                if (index < MinModbusRegister || index > MaxModbusRegister)
                {
                    throw new IndexOutOfRangeException($"Invalid ModbusRegisterMap, Index {index} outside of allowable register key range: [{MinModbusRegister}, {MaxModbusRegister}]");
                }

                if (!uniqueRegisters.Add(index))
                {
                    throw new InvalidOperationException($"Invalid ModbusRegisterMap, Index {index} is duplicated on this device.");
                }
            }
        }


        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            ModbusRegisterMapConfig? other = obj as ModbusRegisterMapConfig;

            if (other == null)
            {
                return false;
            }

            return Name == other.Name;
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return new { PollingRange, Name }.GetHashCode();
        }

    }
}

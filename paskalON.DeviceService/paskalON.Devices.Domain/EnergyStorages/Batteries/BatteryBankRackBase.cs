// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Base class for all battery bank racks.
    /// </summary>
    public abstract class BatteryBankRackBase
    {
        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        /// <summary>
        /// State of the rack.
        /// </summary>
        public int State
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// State of charge of the rack.
        /// </summary>
        public double? StateOfCharge
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// State of health of the rack.
        /// </summary>
        public double? StateOfHealth
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Total DC voltage of the rack.
        /// </summary>
        public double? TotalDCVoltage
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Total DC current of the rack.
        /// </summary>
        public double? TotalDCCurrent
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples
{
    /// <summary>
    /// Battery bank simple rack description.
    /// </summary>
    public static class BbSimpleRackV1Description
    {
        /// <summary>
        /// Rack register start.
        /// </summary>
        public static ushort RackRegisterStart = 4500;


        /// <summary>
        /// Rack register length.
        /// </summary>
        /// <remarks>
        /// The length is not necessary the length of information to read but it is
        /// used to determine the next start of the next rack.
        /// </remarks>
        public static ushort RackRegisterLength = 500;


        /// <summary>
        /// Total number of modules in all racks.
        /// </summary>
        public static int NumberOfModules = BbSimpleV1Description.RackCount * NumberOfModulesPerRack;


        /// <summary>
        /// Total number of cells in all modules.
        /// </summary>
        public static int NumberOfCells = NumberOfModules * NumberOfCells;


        /// <summary>
        /// Number of modules per rack.
        /// </summary>
        public const ushort NumberOfModulesPerRack = 5;


        /// <summary>
        /// Number of cells per module.
        /// </summary>
        public const ushort NumberOfCellsPerModule = 50;


        /// <summary>
        /// Enumeration for whether a rack is enabled or not.
        /// </summary>
        public enum RackEnabledState
        {
            Enabled = 1,
            Disabled = 2
        }


        /// <summary>
        /// Enumeration of registers for the rack.
        /// </summary>
        public enum RegisterRack
        {
            CurrentState = 0,
            StateOfCharge = 1,
            StateOfHealth = 2,
            DCVoltage = 3,
            DCCurrent = 4,
        }
    }
}

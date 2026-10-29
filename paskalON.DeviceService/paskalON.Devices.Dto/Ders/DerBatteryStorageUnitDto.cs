// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.PhysicalUnits.Electricals.Powers;
using DomainBatteryBankState = paskalON.Devices.Domain.EnergyStorages.Batteries.BatteryBankState;

namespace paskalON.Devices.Dto.Ders
{
    /// <summary>
    /// Data Transfer Object for DER battery storage unit.
    /// </summary>
    public record DerBatteryStorageUnitDto : DerUnitDto
    {
        /// <summary>
        /// Power conversion system for this battery storage unit.
        /// </summary>
        public required PcsDto PowerConversionSystem { get; init; }


        /// <summary>
        /// One or many battery banks for this battery storage unit.
        /// </summary>
        public List<BbDto> BatteryBanks { get; init; } = new List<BbDto>();


        /// <summary>
        /// Include operations sent to parent or PCS in the BatteryStorageUnits.
        /// Default this to true; almost all BatteryStorageUnits will want to behave this way.
        /// </summary>
        public bool IncludeBatteryInOperations { get; init; }


        /// <summary>
        /// Value indicating whether the PCS or any battery bank of this battery storage unit has a communication error.
        /// </summary>
        /// <remarks>
        /// A device without core data yet is not counted as communication error.
        /// </remarks>
        public bool CommunicationError
        {
            get => PowerConversionSystem.Core?.CommunicationError == true || BatteryBanks.Any(batteryBank => batteryBank.Core?.CommunicationError == true);
        }


        /// <summary>
        /// Available active power of this battery storage unit, the active available power of its PCS.
        /// </summary>
        public ActivePower? AvailableActivePower { get => PowerConversionSystem.Core?.ActiveAvailablePower; }


        /// <summary>
        /// Available reactive power of this battery storage unit, the reactive available power of its PCS.
        /// </summary>
        public ReactivePower? AvailableReactivePower { get => PowerConversionSystem.Core?.ReactiveAvailablePower; }


        /// <summary>
        /// Flag whether this unit is in maintenance mode.
        /// </summary>
        /// <remarks>
        /// Taken from the PCS detail, which is updated when the maintenance mode of the unit changes.
        /// The configured value applies until the PCS delivered a detail.
        /// </remarks>
        public override bool IsInMaintenanceMode
        {
            get => PowerConversionSystem.Detail?.IsInMaintenanceMode ?? base.IsInMaintenanceMode;
            init => base.IsInMaintenanceMode = value;
        }


        /// <summary>
        /// State of the PCS of this battery storage unit, null until the PCS delivered core data.
        /// </summary>
        public PcsState? PcsState { get => PowerConversionSystem.Core?.State; }


        /// <summary>
        /// Combined state of the battery banks of this battery storage unit.
        /// </summary>
        /// <remarks>
        /// A single battery bank has its own state. Multiple battery banks are <see cref="BatteryStorageUnitBatteryBankState.Connected"/>
        /// if all are connected, <see cref="BatteryStorageUnitBatteryBankState.PartiallyConnected"/> if at least one but not all are
        /// connected, <see cref="BatteryStorageUnitBatteryBankState.Disconnected"/> if all are disconnected and
        /// <see cref="BatteryStorageUnitBatteryBankState.UnknownCombination"/> otherwise or without battery bank.
        /// A battery bank without core data counts as unknown state.
        /// </remarks>
        public BatteryStorageUnitBatteryBankState BatteryBankState
        {
            get
            {
                List<DomainBatteryBankState> states = BatteryBanks.Select(batteryBank => batteryBank.Core?.State ?? DomainBatteryBankState.Unknown).ToList();

                if (states.Count == 0)
                {
                    return BatteryStorageUnitBatteryBankState.UnknownCombination;
                }

                if (states.Count == 1)
                {
                    return (BatteryStorageUnitBatteryBankState)states[0];
                }

                if (states.All(state => state == DomainBatteryBankState.Connected))
                {
                    return BatteryStorageUnitBatteryBankState.Connected;
                }

                if (states.Any(state => state == DomainBatteryBankState.Connected))
                {
                    return BatteryStorageUnitBatteryBankState.PartiallyConnected;
                }

                if (states.All(state => state == DomainBatteryBankState.Disconnected))
                {
                    return BatteryStorageUnitBatteryBankState.Disconnected;
                }

                return BatteryStorageUnitBatteryBankState.UnknownCombination;
            }
        }
    }
}

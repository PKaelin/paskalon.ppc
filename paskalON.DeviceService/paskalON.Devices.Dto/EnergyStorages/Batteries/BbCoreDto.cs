// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.EnergyStorages.Batteries;

namespace paskalON.Devices.Dto.EnergyStorages.Batteries
{
    /// <summary>
    /// Data Transfer Object for a Battery Bank Core.
    /// </summary>
    /// <remarks>
    /// Used as high frequency DTO update.
    /// </remarks>
    public record BbCoreDto : IDevice
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// Battery bank state.
        /// </summary>
        public BatteryBankState State { get; init; }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError { get; init; }


        /// <summary>
        /// The flow direction of the battery bank, indicating whether it is charging, discharging, or idle.
        /// </summary>
        public BatteryBankFlowDirection? BatteryBankFlowDirection { get; init; }

        /// <summary>
        /// Usable state of charge as a percentage of the battery's capacity.        
        /// </summary>
        public double? StateOfCharge { get; init; }


        /// <summary>
        /// Actual state of charge as a percentage of the battery's actual capacity rather than its usable capacity.
        /// </summary>
        public double? ActualStateOfCharge { get; init; }


        /// <summary>
        /// DC bus voltage of the battery bank.
        /// </summary>
        public double? TotalDCVoltage { get; init; }


        /// <summary>
        /// DC bus current of the battery bank.
        /// </summary>
        public double? TotalDCCurrent { get; init; }
    }
}

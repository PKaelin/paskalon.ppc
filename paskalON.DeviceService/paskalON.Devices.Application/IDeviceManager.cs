// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;

namespace paskalON.Devices.Application
{
    /// <summary>
    /// Device manager interface definition.
    /// </summary>
    public interface IDeviceManager
    {
        /// <summary>
        /// Distributed Energy Resource (DER) root object.
        /// </summary>
        Der Der { get; }


        /// <summary>
        /// Collection of Power Conversion Systems (PCS) that are withing the DER.
        /// </summary>
        ICollection<PowerConversionSystemBase> PowerConversionSystems { get; }


        /// <summary>
        /// Collection of Battery Banks (BB) that are withing the DER.
        /// </summary>
        ICollection<BatteryBankBase> BatteryBanks { get; }


        /// <summary>
        /// Collection of Solar Panels (PV) that are withing the DER.
        /// </summary>
        ICollection<SolarPanelBase> SolarPanels { get; }


        /// <summary>
        /// Collection of External Power Meters (EPM) that are withing the DER.
        /// </summary>
        ICollection<ExternalPowerMeter> ExternalPowerMeters { get; }


        /// <summary>
        /// Collection of Auxiliary Power Meters (APM) that are withing the DER.
        /// </summary>
        ICollection<AuxiliaryPowerMeter> AuxiliaryPowerMeters { get; }


        /// <summary>
        /// Collection of System Power Meters (SPM) that are withing the DER.
        /// </summary>
        ICollection<SystemPowerMeter> SystemPowerMeters { get; }


        /// <summary>
        /// Collection of Circuit Power Meters (CPM) that are withing the DER.
        /// </summary>
        ICollection<CircuitPowerMeter> CircuitPowerMeters { get; }


        /// <summary>
        /// Loads the Distributed Energy Resource (DER) and all its content.
        /// </summary>
        Task LoadDerAsync();


        /// <summary>
        /// Start all PCS.
        /// </summary>
        Task StartAllPcsAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Stop all PCS.
        /// </summary>
        Task StopAllPcsAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Puts all PCS into standby.
        /// </summary>
        Task StandbyAllPcsAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Start specific PCS.
        /// </summary>
        /// <param name="deviceId">Device id the action is executed on.</param>
        Task StartPcsAsync(int deviceId);


        /// <summary>
        /// Stop specific PCS.
        /// </summary>
        /// <param name="deviceId">Device id the action is executed on.</param>
        Task StopPcsAsync(int deviceId);


        /// <summary>
        /// Puts a specific PCS into standby.
        /// </summary>
        /// <param name="deviceId">Device id the action is executed on.</param>
        Task StandbyPcsAsync(int deviceId);


        /// <summary>
        /// Connect a specific BB.
        /// </summary>
        /// <param name="deviceId">Device id the action is executed on.</param>
        Task ConnectBatteryBankAsync(int deviceId);


        /// <summary>
        /// Disconnect a specific BB.
        /// </summary>
        /// <param name="deviceId">Device id the action is executed on.</param>
        Task DisconnectBatteryBankAsync(int deviceId);


        /// <summary>
        /// Puts a specific DER Unit into maintenance.
        /// </summary>
        /// <param name="unitName">DER unit name the action is executed on.</param>
        void PutIntoMaintenance(string unitName);
    }
}

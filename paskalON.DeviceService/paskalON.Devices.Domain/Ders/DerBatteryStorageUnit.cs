// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.PowerConversionSystems;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace paskalON.Devices.Domain.Ders
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// DER battery storage unit for one or multiple battery banks and one power conversion system.
    /// </summary>
    public class DerBatteryStorageUnit : DerUnit, IDisposable
    {
        /// <summary>
        /// DER battery storage unit configuration.
        /// </summary>
        private readonly DerBatteryStorageUnitConfig _config;


        /// <summary>
        /// Power conversion system for this battery storage unit.
        /// </summary>
        public PowerConversionSystemBase? PowerConversionSystem { get; set; }


        /// <summary>
        /// One or many battery banks for this battery storage unit.
        /// </summary>
        public ObservableCollection<BatteryBankBase> BatteryBanks { get; set; } = new ObservableCollection<BatteryBankBase>();


        /// <summary>
        /// Include operations sent to parent or PCS in the BatteryStorageUnits.
        /// Default this to true; almost all BatteryStorageUnits will want to behave this way.
        /// </summary>
        public bool IncludeBatteryInOperations { get => _config.IncludeBatteryInOperations; }


        /// <summary>
        /// Constructor of <see cref="DerBatteryStorageUnit"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The DER battery storage unit configuration.</param>
        /// <param name="derCircuit">The parent DER circuit.</param>
        public DerBatteryStorageUnit(ILogger logger, DerBatteryStorageUnitConfig config, DerCircuit derCircuit) : base(logger, config, derCircuit)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
            BatteryBanks.CollectionChanged += BatteryBanks_CollectionChanged;
        }


        /// <summary>
        /// Triggered when item(s) are removed or added to the battery bank collection.
        /// </summary>
        /// <param name="sender">The object that triggered the change.</param>
        /// <param name="e">The notify collection changed event args.</param>
        private void BatteryBanks_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (BatteryBankBase batteryBank in e.NewItems!)
                {
                    batteryBank.StateChanged += OnBatteryBankStateChange;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (BatteryBankBase batteryBank in e.OldItems!)
                {
                    batteryBank.StateChanged -= OnBatteryBankStateChange;
                }
            }
        }


        /// <summary>
        /// Triggered when the battery bank state changes.
        /// </summary>
        /// <param name="sender">The battery bank instance.</param>
        /// <param name="e">The battery bank event argument.</param>
        private void OnBatteryBankStateChange(object? sender, BatteryBankStateChangedEventArgs e)
        {
            if (BatteryBanks.All(b => b.State == BatteryBankState.Disconnected || b.State == BatteryBankState.Unknown || b.State == BatteryBankState.Fault))
            {
                // Stops the PCS when all battery banks are disconnected, unknown or fault.
                PowerConversionSystem?.StopAsync();
            }
        }


        /// <summary>
        /// Dispose instance.
        /// </summary>
        public void Dispose()
        {
            // Deregister event handlers.
            BatteryBanks.Clear();
        }
    }
}

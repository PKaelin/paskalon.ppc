// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Application.Dispatchers
{
    /// <summary>
    /// Dispatches the power targets of DER units to the device service.
    /// Every time a registered unit receives new targets, they are delivered to the unit's power conversion system (PCS).
    /// A stopped or standby unit is started first and the targets are sent once it reached the started state.
    /// </summary>
    public interface IDerUnitTargetDispatcher : IAsyncDisposable
    {
        /// <summary>
        /// Registers a DER unit so that all its future power targets are dispatched to the device service.
        /// </summary>
        /// <param name="unit">The DER unit power control to register.</param>
        void Register(IDerUnitPowerControl unit);
    }
}

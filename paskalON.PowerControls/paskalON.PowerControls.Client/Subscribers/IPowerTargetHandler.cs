// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Dto.Powers;

namespace paskalON.PowerControls.Client.Subscribers
{
    /// <summary>
    /// Handles the power targets received by the <see cref="IPowerTargetSubscriber"/>. Implemented by each service consuming the power targets.
    /// </summary>
    public interface IPowerTargetHandler
    {
        /// <summary>
        /// Handles the received power targets of a power conversion system (PCS).
        /// </summary>
        /// <param name="targets">The received power targets.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task HandleAsync(CurrentTargetsDto targets);
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging.Health
{
    /// <summary>
    /// Availability state of a dependency, e.g. the message broker or another service.
    /// </summary>
    public enum DependencyState
    {
        /// <summary>
        /// The dependency was never seen or not seen within its loss timeout.
        /// </summary>
        Lost,
        /// <summary>
        /// The dependency was seen within its loss timeout.
        /// </summary>
        Available
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Application
{
    /// <summary>
    /// Result of a system command.
    /// </summary>
    public enum SystemCommandResult
    {
        /// <summary>
        /// The command was executed.
        /// </summary>
        Executed,


        /// <summary>
        /// The command was not executed because the shutdown override operating mode is active.
        /// </summary>
        ShutdownActive,


        /// <summary>
        /// The command was not executed because the shutdown override operating mode is not active.
        /// </summary>
        ShutdownNotActive
    }
}

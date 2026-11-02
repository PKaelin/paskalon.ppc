// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Application exit codes of the Data projects.
    /// </summary>
    public enum ClientDataExitCode
    {
        /// <summary>
        /// Successfully imported.
        /// </summary>
        Success = 0,

        /// <summary>
        /// General execution error.
        /// </summary>
        GeneralError = 1,

        /// <summary>
        /// Invalid arguments.
        /// </summary>
        InvalidArguments = 2,

        /// <summary>
        /// CSV validation error.
        /// </summary>
        CsvValidationError = 3,

        /// <summary>
        /// Database error.
        /// </summary>
        DatabaseError = 4
    }
}

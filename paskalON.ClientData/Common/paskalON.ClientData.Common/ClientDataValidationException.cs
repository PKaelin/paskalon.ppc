// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Thrown when a client project file contains validation errors.
    /// </summary>
    /// <remarks>
    /// Used for specific validation errors and exit code. E.g. when a line is not in the Key=Value format or a required property is missing.
    /// </remarks>
    public class ClientDataValidationException : Exception
    {
        /// <summary>
        /// All the errors found in the file.
        /// </summary>
        public IReadOnlyList<ClientDataError> Errors { get; }


        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="errors">All the errors found in the file.</param>
        public ClientDataValidationException(IReadOnlyList<ClientDataError> errors) : base(string.Join(Environment.NewLine, errors))
        {
            Errors = errors;
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Thrown when the arguments of a Data project are invalid.
    /// </summary>
    /// <remarks>
    /// Used for specific argument errors and exit code. E.g. when a file does not exist or a schema version is invalid.
    /// </remarks>
    public class ClientDataArgumentException : Exception
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">Description of the invalid argument.</param>        
        public ClientDataArgumentException(string message) : base(message)
        {
        }
    }
}

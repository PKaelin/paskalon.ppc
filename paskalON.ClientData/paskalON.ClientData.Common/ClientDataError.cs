// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// A single validation error of a client project file.
    /// </summary>
    /// <param name="File">The client project file.</param>
    /// <param name="LineNumber">The 1-based line number.</param>
    /// <param name="EntityName">The entity name if it is known.</param>
    /// <param name="PropertyName">The property name if the error relates to a property.</param>
    /// <param name="Message">The description of the error.</param>
    public record ClientDataError(string File, int LineNumber, string? EntityName, string? PropertyName, string Message)
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            string entity = EntityName ?? "<unknown>";
            string property = PropertyName ?? "<none>";

            return $"{File}({LineNumber}): Entity '{entity}', property '{property}': {Message}";
        }
    }
}

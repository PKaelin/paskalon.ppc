// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// An entity created from a line of a client data file together with its position in the file.
    /// </summary>
    /// <param name="Entity">The created entity.</param>
    /// <param name="File">The file the entity is defined in.</param>
    /// <param name="LineNumber">The 1-based line number of the entity.</param>
    /// <param name="EntityName">The entity name if the line defines one.</param>
    public record ClientDataEntity(object Entity, string File, int LineNumber, string? EntityName);
}

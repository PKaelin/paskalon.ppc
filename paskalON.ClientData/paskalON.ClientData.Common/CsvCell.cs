// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// A single Key=Value cell of a client project file.
    /// </summary>
    /// <param name="Key">The text in front of the first equal sign.</param>
    /// <param name="Value">The text after the first equal sign without the quotes.</param>
    /// <param name="IsQuoted">Whether the cell contained quotes, a quoted null is the text "null".</param>
    public record CsvCell(string Key, string Value, bool IsQuoted);
}

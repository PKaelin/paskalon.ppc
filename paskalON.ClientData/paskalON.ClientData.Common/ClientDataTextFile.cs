// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// The content of a localized text file of a client project.
    /// </summary>
    /// <param name="File">The name of the file, used for the error messages.</param>
    /// <param name="Language">The language of the file name, e.g. en of DemoBattery.Texts.en.csv.</param>
    /// <param name="Lines">The lines of the file.</param>
    public record ClientDataTextFile(string File, string Language, IReadOnlyList<string> Lines);
}

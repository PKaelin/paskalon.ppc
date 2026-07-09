// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// Interface for C37 data face.
    /// </summary>
    public interface IC37Dataface
    {
        /// <summary>
        /// List of IC37RegisterEntry registrations.
        /// </summary>
        List<IC37RegisterEntry> Registers { get; }
    }
}

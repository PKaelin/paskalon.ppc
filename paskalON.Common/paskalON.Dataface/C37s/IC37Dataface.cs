// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// IC37Dataface is the specific dataface for C36 registrations and communications.
    /// </summary>    
    public interface IC37Dataface : IDataface
    {
        /// <summary>
        /// List of IC37RegisterEntry registrations.
        /// </summary>
        List<IC37RegisterEntry> Registers { get; }
    }
}

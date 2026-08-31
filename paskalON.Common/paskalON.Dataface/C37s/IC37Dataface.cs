// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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

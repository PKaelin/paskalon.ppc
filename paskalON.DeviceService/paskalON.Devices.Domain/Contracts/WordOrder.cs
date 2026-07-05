// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Domains.Contracts
{
    /// <summary>
    /// Defines the order in which bytes are arranged within a word for data representation.
    /// </summary>
    public enum WordOrder
    {
        /// <summary>
        /// No specific order is defined.
        /// </summary>
        None = 0,
        /// <summary>
        /// Big-endian stores the most significant byte (MSB) at the lowest memory address.
        /// </summary>
        BigEndian = 1,
        /// <summary>
        /// Little-endian stores the least significant byte (LSB) at the lowest memory address.
        /// </summary>
        LittleEndian = 2
    }
}

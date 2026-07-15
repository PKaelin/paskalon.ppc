// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Communication.Protocols.C37118.Types
{
    /// <summary>
    /// Transport layer that dictates control logic for C37.118 communications.
    /// Also specifies the underlying protocol (TCP or UDP) C37.118 uses to communicate with remote device.
    /// </summary>
    public enum C37TransportLayer
    {
        /// <summary>
        /// Transmission Control Protocol (TCP)
        /// </summary>
        TCP = 0,
        /// <summary>
        /// User Datagram Protocol (UDP)
        /// </summary>
        UDP = 1,
    }
}

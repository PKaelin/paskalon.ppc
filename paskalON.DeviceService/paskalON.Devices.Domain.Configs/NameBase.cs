// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs
{
    public abstract class NameBase : DomainBase
    {
        /// <summary>
        /// Name of the Distributed Energy Resource (DER).
        /// </summary>
        public required virtual string Name { get; set; }

    }
}

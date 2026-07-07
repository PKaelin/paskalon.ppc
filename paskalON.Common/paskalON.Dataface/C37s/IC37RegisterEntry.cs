// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    public interface IC37RegisterEntry
    {
        object Instance { get; }
        string Name { get; }

        void Update(object value);
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    public interface IC37Register
    {
        void Register<TDevice, TProperty>(TDevice instance, string name, Action<TDevice, TProperty> setter);
    }
}

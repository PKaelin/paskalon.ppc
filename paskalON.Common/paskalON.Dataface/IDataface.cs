// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface
{
    public interface IDataface
    {
        void Register<TDevice, TCom>(Action<TCom> com);
    }
}

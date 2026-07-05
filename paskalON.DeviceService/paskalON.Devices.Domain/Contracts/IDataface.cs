// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Domains.Contracts
{
    /// <summary>
    /// Interface to completely loose couple the data source from the data consumer.
    /// </summary>
    public interface IDataface<T>
    {
        void Register<TCom>(Action<TCom> com);
    }
}

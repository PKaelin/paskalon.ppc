// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Telemetry.Entries
{
    public interface IDeviceRegister<TDevice, TDefinition, TCore, TDetail>
    {
        void Add(TDevice device);

        bool TryGet(int deviceId, out TDevice? device);

        void UpdateDefinition(TDefinition message);

        void UpdateCore(TCore message);

        void UpdateDetail(TDetail message);
    }
}

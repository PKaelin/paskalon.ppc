// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    public interface IModbusRegister
    {
        void Register<TDevice, TProperty>(TDevice instance, string name, Action<TDevice, TProperty> setter, int register, double scale,
            ModbusDataType dataType, int offset = 0);

        void RegisterRange(ushort from, ushort to, ModbusRegistryType registryType, int interval);
    }
}

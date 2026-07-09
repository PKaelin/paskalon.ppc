// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    public interface IModbusRegisterEntry
    {
        object Instance { get; }
        string Name { get; }
        int Register { get; }
        double Scale { get; }
        ModbusDataType DataType { get; }
        int Offset { get; }

        void Update(object value);
    }
}

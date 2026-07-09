// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    public class ModbusPollingRangeEntry
    {
        public ushort From { get; init; }
        public ushort To { get; init; }
        public ModbusRegistryType RegistryType { get; init; }
        public int Interval { get; init; }

        public ModbusPollingRangeEntry(ushort from, ushort to, ModbusRegistryType registryType, int interval)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(to, from);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(from, to);

            From = from;
            To = to;
            RegistryType = registryType;
            Interval = interval;
        }
    }
}

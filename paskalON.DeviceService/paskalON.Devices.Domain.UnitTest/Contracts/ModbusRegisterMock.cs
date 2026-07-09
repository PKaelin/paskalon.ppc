// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Dataface.Modbus;

namespace paskalON.Devices.Domain.UnitTest.Contracts
{
    internal class ModbusRegisterMock
    {
        internal Dictionary<string, IModbusRegisterEntry> Registrations = new Dictionary<string, IModbusRegisterEntry>();

        internal Mock<IModbusRegister> Mock { get; init; }


        internal ModbusRegisterMock()
        {
            Mock = new Mock<IModbusRegister>();
        }

        internal void Setup<TDevice, TProperty>()
        {
            // Correctly set up the generic Register method using It.IsAny<T>() matchers.
            Mock.Setup(x => x.Register<TDevice, TProperty>(
                    It.IsAny<TDevice>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<TDevice, TProperty>>(),
                    It.IsAny<int>(),
                    It.IsAny<double>(),
                    It.IsAny<ModbusDataType>(),
                    It.IsAny<int>()))
                .Callback((TDevice instance, string name, Action<TDevice, TProperty> setter, int register, double scale, ModbusDataType dataType, int offset) =>
                {
                    ArgumentNullException.ThrowIfNull(instance);
                    Registrations[name] = new ModbusRegisterEntry<TDevice, TProperty>(instance, name, setter, register, scale, dataType, offset);
                });
        }
    }
}

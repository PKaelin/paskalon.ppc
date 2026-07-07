// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Dataface;
using paskalON.Dataface.Modbus;

namespace paskalON.Devices.Domain.UnitTest.Contracts
{
    internal class DatafaceModbusMock
    {
        internal Mock<IDataface> Mock { get; init; }

        public ModbusRegisterMock ModbusRegister { get; init; }

        internal DatafaceModbusMock()
        {
            Mock = new Mock<IDataface>();
            ModbusRegister = new ModbusRegisterMock();
        }

        internal void Setup<TDevice, TProperty>()
        {
            ModbusRegister.Setup<TDevice, TProperty>();
            Mock.Setup(x => x.Register<TDevice, TProperty>(
                It.IsAny<Action<TProperty>>()))
            .Callback<Action<IModbusRegister>>(action =>
            {
                action(ModbusRegister.Mock.Object);
            });
        }
    }
}
// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Client.Registers;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Devices.Client.UnitTest.Registers
{
    [TestClass]
    public class DeviceRegisterTest
    {
        [TestMethod]
        public void DeviceRegisterWithNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                { new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(null!); });
        }


        [TestMethod]
        public void DeviceRegisterConstructorTest()
        {
            DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> register =
                new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(NullLogger.Instance);

            Assert.IsNotNull(register);
            Assert.IsNotNull(register.Devices);
            Assert.HasCount(0, register.Devices);
        }


        [TestMethod]
        public void DeviceRegisterAddDeviceTest()
        {
            DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> register =
                new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(NullLogger.Instance);

            PcsDto device = new PcsDto(new PcsDefinitionDto { DeviceId = 1, Name = "Test" });
            register.Add(device);

            Assert.HasCount(1, register.Devices);
            Assert.Contains(device, register.Devices);
        }


        [TestMethod]
        public void DeviceRegisterAddSameDeviceIdTest()
        {
            DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> register =
                new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(NullLogger.Instance);

            PcsDto firstDevice = new PcsDto(new PcsDefinitionDto { DeviceId = 1, Name = "Test" });
            PcsDto secondDevice = new PcsDto(new PcsDefinitionDto { DeviceId = 1, Name = "Test" });
            register.Add(firstDevice);

            Assert.ThrowsExactly<InvalidOperationException>(() => { register.Add(secondDevice); });
        }


        [TestMethod]
        public void DeviceRegisterTryGetTest()
        {
            DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> register =
                new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(NullLogger.Instance);

            PcsDto device = new PcsDto(new PcsDefinitionDto { DeviceId = 1, Name = "Test" });
            register.Add(device);

            bool result = register.TryGet(1, out PcsDto? foundDevice);

            Assert.IsTrue(result);
            Assert.IsNotNull(foundDevice);
            Assert.AreSame(device, foundDevice);
        }


        [TestMethod]
        public void DeviceRegisterTryGetUnregisteredDeviceIdTest()
        {
            DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> register =
                new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(NullLogger.Instance);

            bool result = register.TryGet(999, out PcsDto? foundDevice);

            Assert.IsFalse(result);
            Assert.IsNull(foundDevice);
        }


        [TestMethod]
        public void DeviceRegisterUpdateDefinitionTest()
        {
            DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> register =
                new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(NullLogger.Instance);

            PcsDto device = new PcsDto(new PcsDefinitionDto { DeviceId = 1, Name = "Test" });
            register.Add(device);

            PcsDefinitionDto update = new PcsDefinitionDto { DeviceId = 1, Name = "Updated" };
            register.UpdateDefinition(update);

            Assert.HasCount(1, register.Devices.Where(d => d.DeviceId == device.DeviceId));
            Assert.AreEqual(update.Name, register.Devices.Where(d => d.DeviceId == device.DeviceId).First().Name);
        }


        [TestMethod]
        public void DeviceRegisterUpdateCoreTest()
        {
            DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> register =
                new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(NullLogger.Instance);

            PcsDto device = new PcsDto(new PcsDefinitionDto { DeviceId = 1, Name = "Test" });
            register.Add(device);

            PcsCoreDto update = new PcsCoreDto { DeviceId = 1, State = PcsState.Starting };
            register.UpdateCore(update);

            Assert.HasCount(1, register.Devices.Where(d => d.DeviceId == device.DeviceId));
            Assert.IsNotNull(register.Devices.Where(d => d.DeviceId == device.DeviceId).First().Core);
            Assert.AreEqual(update.State, register.Devices.Where(d => d.DeviceId == device.DeviceId).First().Core!.State);
        }


        [TestMethod]
        public void DeviceRegisterUpdateDetailTest()
        {
            DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> register =
                new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(NullLogger.Instance);

            PcsDto device = new PcsDto(new PcsDefinitionDto { DeviceId = 1, Name = "Test" });
            register.Add(device);

            ActivePower activePower = new ActivePower(11);
            ReactivePower reactivePower = new ReactivePower(22);
            PcsDetailDto update = new PcsDetailDto { DeviceId = 1, ActivePowerTarget = activePower, ReactivePowerTarget = reactivePower };
            register.UpdateDetail(update);

            Assert.HasCount(1, register.Devices.Where(d => d.DeviceId == device.DeviceId));
            Assert.IsNotNull(register.Devices.Where(d => d.DeviceId == device.DeviceId).First().Detail);
            Assert.AreEqual(activePower, register.Devices.Where(d => d.DeviceId == device.DeviceId).First().Detail!.ActivePowerTarget);
            Assert.AreEqual(reactivePower, register.Devices.Where(d => d.DeviceId == device.DeviceId).First().Detail!.ReactivePowerTarget);
        }
    }
}

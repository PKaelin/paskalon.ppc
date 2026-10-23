// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Devices.Application.Subscribers;
using paskalON.PowerControls.Dto.Powers;

namespace paskalON.Devices.Application.UnitTest.Subscribers
{
    [TestClass]
    public sealed class PowerTargetHandlerTest
    {
        [TestMethod]
        public void PowerTargetHandlerNullDeviceManagerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerTargetHandler(null!));
        }


        [TestMethod]
        public async Task HandleAsyncNullTargetsTest()
        {
            Mock<IDeviceManager> deviceManager = new Mock<IDeviceManager>();
            PowerTargetHandler handler = new PowerTargetHandler(deviceManager.Object);

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => handler.HandleAsync(null!));

            deviceManager.Verify(m => m.SetPcsPowerTarget(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        [TestMethod]
        public async Task HandleAsyncSetsPcsPowerTargetTest()
        {
            Mock<IDeviceManager> deviceManager = new Mock<IDeviceManager>();
            deviceManager.Setup(m => m.SetPcsPowerTarget(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>()))
                .Returns(Task.CompletedTask);
            PowerTargetHandler handler = new PowerTargetHandler(deviceManager.Object);
            CurrentTargetsDto targets = new CurrentTargetsDto { DeviceId = 7, ActivePowerWatt = 25000.5, ReactivePowerVar = -4000.25 };

            await handler.HandleAsync(targets);

            deviceManager.Verify(m => m.SetPcsPowerTarget(7, 25000.5, -4000.25), Times.Once);
        }


        [TestMethod]
        public async Task HandleAsyncPropagatesDeviceManagerFailureTest()
        {
            Mock<IDeviceManager> deviceManager = new Mock<IDeviceManager>();
            deviceManager.Setup(m => m.SetPcsPowerTarget(99, 1000, 0))
                .ThrowsAsync(new InvalidOperationException("Device Manager cannot find PCS with Device ID: 99"));
            PowerTargetHandler handler = new PowerTargetHandler(deviceManager.Object);
            CurrentTargetsDto targets = new CurrentTargetsDto { DeviceId = 99, ActivePowerWatt = 1000, ReactivePowerVar = 0 };

            Task handle = handler.HandleAsync(targets);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => handle);
        }
    }
}

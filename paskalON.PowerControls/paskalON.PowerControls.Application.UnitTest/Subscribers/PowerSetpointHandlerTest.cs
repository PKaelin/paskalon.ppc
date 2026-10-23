// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Application.Subscribers;

namespace paskalON.PowerControls.Application.UnitTest.Subscribers
{
    [TestClass]
    public sealed class PowerSetpointHandlerTest
    {
        private const string DerUnitName = "BESS-Unit 1";


        [TestMethod]
        public void PowerSetpointHandlerNullPowerControlManagerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerSetpointHandler(null!));
        }


        [TestMethod]
        public async Task HandleSetpointsAsyncNullSetpointsTest()
        {
            Mock<IPowerControlManager> powerControlManager = new Mock<IPowerControlManager>();
            PowerSetpointHandler handler = new PowerSetpointHandler(powerControlManager.Object);

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => handler.HandleSetpointsAsync(null!));

            powerControlManager.Verify(m => m.SetSystemPowerTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
        }


        [TestMethod]
        public async Task HandleSetpointsAsyncSetsSystemPowerTargetTest()
        {
            Mock<IPowerControlManager> powerControlManager = new Mock<IPowerControlManager>();
            powerControlManager.Setup(m => m.SetSystemPowerTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()))
                .Returns(Task.CompletedTask);
            PowerSetpointHandler handler = new PowerSetpointHandler(powerControlManager.Object);
            CurrentSetpointsDto setpoints = new CurrentSetpointsDto { SetpointActivePowerWatts = 25000.5, SetpointReactivePowerVars = -4000.25 };

            await handler.HandleSetpointsAsync(setpoints);

            powerControlManager.Verify(m => m.SetSystemPowerTarget(new ActivePower(25000.5), new ReactivePower(-4000.25)), Times.Once);
        }


        [TestMethod]
        public async Task HandleDerUnitSetpointsAsyncNullSetpointsTest()
        {
            Mock<IPowerControlManager> powerControlManager = new Mock<IPowerControlManager>();
            PowerSetpointHandler handler = new PowerSetpointHandler(powerControlManager.Object);

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => handler.HandleDerUnitSetpointsAsync(null!));

            powerControlManager.Verify(m => m.SetDerUnitPowerTarget(It.IsAny<string>(), It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
        }


        [TestMethod]
        public async Task HandleDerUnitSetpointsAsyncSetsDerUnitPowerTargetTest()
        {
            Mock<IPowerControlManager> powerControlManager = new Mock<IPowerControlManager>();
            powerControlManager.Setup(m => m.SetDerUnitPowerTarget(It.IsAny<string>(), It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()))
                .Returns(Task.CompletedTask);
            PowerSetpointHandler handler = new PowerSetpointHandler(powerControlManager.Object);
            CurrentDerUnitSetpointsDto setpoints = new CurrentDerUnitSetpointsDto
            {
                DerUnitName = DerUnitName,
                SetpointActivePowerWatts = -30000,
                SetpointReactivePowerVars = 5000
            };

            await handler.HandleDerUnitSetpointsAsync(setpoints);

            powerControlManager.Verify(m => m.SetDerUnitPowerTarget(DerUnitName, new ActivePower(-30000), new ReactivePower(5000)), Times.Once);
        }


        [TestMethod]
        public async Task HandleDerUnitSetpointsAsyncPropagatesManagerFailureTest()
        {
            Mock<IPowerControlManager> powerControlManager = new Mock<IPowerControlManager>();
            powerControlManager.Setup(m => m.SetDerUnitPowerTarget("BESS-Unit 9", It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()))
                .ThrowsAsync(new KeyNotFoundException("DER unit BESS-Unit 9 does not exist"));
            PowerSetpointHandler handler = new PowerSetpointHandler(powerControlManager.Object);
            CurrentDerUnitSetpointsDto setpoints = new CurrentDerUnitSetpointsDto { DerUnitName = "BESS-Unit 9", SetpointActivePowerWatts = 1000, SetpointReactivePowerVars = 0 };

            Task handle = handler.HandleDerUnitSetpointsAsync(setpoints);

            await Assert.ThrowsExactlyAsync<KeyNotFoundException>(() => handle);
        }
    }
}

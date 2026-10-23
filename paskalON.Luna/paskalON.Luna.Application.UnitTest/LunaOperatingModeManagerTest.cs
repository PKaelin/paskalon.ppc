// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Dto.Powers;

namespace paskalON.Luna.Application.UnitTest
{
    [TestClass]
    public sealed class LunaOperatingModeManagerTest
    {
        private const string DerUnitName = "BESS-Unit 1";


        [TestMethod]
        public void GetSetpointsWithoutReceivedSetpointsIsZeroTest()
        {
            LunaOperatingModeManager manager = new LunaOperatingModeManager();

            CurrentSetpointsDto setpoints = manager.GetSetpoints();

            Assert.AreEqual(0, setpoints.SetpointActivePowerWatts);
            Assert.AreEqual(0, setpoints.SetpointReactivePowerVars);
        }


        [TestMethod]
        public async Task HandleSetpointsAsyncNullSetpointsTest()
        {
            LunaOperatingModeManager manager = new LunaOperatingModeManager();

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => manager.HandleSetpointsAsync(null!));
        }


        [TestMethod]
        public async Task HandleSetpointsAsyncReplacesSetpointsTest()
        {
            LunaOperatingModeManager manager = new LunaOperatingModeManager();
            CurrentSetpointsDto previous = new CurrentSetpointsDto { SetpointActivePowerWatts = 25000.5, SetpointReactivePowerVars = -4000.25 };
            CurrentSetpointsDto latest = new CurrentSetpointsDto { SetpointActivePowerWatts = -12000, SetpointReactivePowerVars = 3000 };

            await manager.HandleSetpointsAsync(previous);
            await manager.HandleSetpointsAsync(latest);

            Assert.AreSame(latest, manager.GetSetpoints());
        }


        [TestMethod]
        public void GetDerUnitSetpointsWithoutReceivedSetpointsIsEmptyTest()
        {
            LunaOperatingModeManager manager = new LunaOperatingModeManager();

            IReadOnlyCollection<CurrentDerUnitSetpointsDto> setpoints = manager.GetDerUnitSetpoints();

            Assert.IsEmpty(setpoints);
            Assert.IsNull(manager.GetDerUnitSetpoints(DerUnitName));
        }


        [TestMethod]
        public async Task HandleDerUnitSetpointsAsyncNullSetpointsTest()
        {
            LunaOperatingModeManager manager = new LunaOperatingModeManager();

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => manager.HandleDerUnitSetpointsAsync(null!));
        }


        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public async Task HandleDerUnitSetpointsAsyncWithoutDerUnitNameTest(string derUnitName)
        {
            LunaOperatingModeManager manager = new LunaOperatingModeManager();
            CurrentDerUnitSetpointsDto setpoints = new CurrentDerUnitSetpointsDto
            {
                DerUnitName = derUnitName,
                SetpointActivePowerWatts = 1000,
                SetpointReactivePowerVars = 0
            };

            await Assert.ThrowsExactlyAsync<ArgumentException>(() => manager.HandleDerUnitSetpointsAsync(setpoints));

            Assert.IsEmpty(manager.GetDerUnitSetpoints());
        }


        [TestMethod]
        public async Task HandleDerUnitSetpointsAsyncStoresSetpointsPerDerUnitTest()
        {
            LunaOperatingModeManager manager = new LunaOperatingModeManager();
            CurrentDerUnitSetpointsDto first = new CurrentDerUnitSetpointsDto { DerUnitName = DerUnitName, SetpointActivePowerWatts = -30000, SetpointReactivePowerVars = 5000 };
            CurrentDerUnitSetpointsDto second = new CurrentDerUnitSetpointsDto { DerUnitName = "BESS-Unit 2", SetpointActivePowerWatts = 2000, SetpointReactivePowerVars = 0 };

            await manager.HandleDerUnitSetpointsAsync(first);
            await manager.HandleDerUnitSetpointsAsync(second);

            Assert.AreEqual(first, manager.GetDerUnitSetpoints(DerUnitName));
            Assert.AreEqual(second, manager.GetDerUnitSetpoints("BESS-Unit 2"));
            CollectionAssert.AreEquivalent(new List<CurrentDerUnitSetpointsDto> { first, second }, manager.GetDerUnitSetpoints().ToList());
        }


        [TestMethod]
        public async Task HandleDerUnitSetpointsAsyncReplacesSetpointsOfSameDerUnitTest()
        {
            LunaOperatingModeManager manager = new LunaOperatingModeManager();
            CurrentDerUnitSetpointsDto previous = new CurrentDerUnitSetpointsDto { DerUnitName = DerUnitName, SetpointActivePowerWatts = -30000, SetpointReactivePowerVars = 5000 };
            CurrentDerUnitSetpointsDto latest = new CurrentDerUnitSetpointsDto { DerUnitName = DerUnitName, SetpointActivePowerWatts = 12000, SetpointReactivePowerVars = 3000 };

            await manager.HandleDerUnitSetpointsAsync(previous);
            await manager.HandleDerUnitSetpointsAsync(latest);

            Assert.AreEqual(latest, manager.GetDerUnitSetpoints(DerUnitName));
            Assert.HasCount(1, manager.GetDerUnitSetpoints());
        }
    }
}

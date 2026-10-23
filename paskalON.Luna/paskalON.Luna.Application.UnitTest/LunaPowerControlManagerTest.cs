// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Dto.Powers;

namespace paskalON.Luna.Application.UnitTest
{
    [TestClass]
    public sealed class LunaPowerControlManagerTest
    {
        [TestMethod]
        public void GetTargetsWithoutReceivedTargetsIsEmptyTest()
        {
            LunaPowerControlManager manager = new LunaPowerControlManager();

            IReadOnlyCollection<CurrentTargetsDto> targets = manager.GetTargets();

            Assert.IsEmpty(targets);
        }


        [TestMethod]
        public void GetTargetWithoutReceivedTargetsReturnsNullTest()
        {
            LunaPowerControlManager manager = new LunaPowerControlManager();

            CurrentTargetsDto? targets = manager.GetTarget(7);

            Assert.IsNull(targets);
        }


        [TestMethod]
        public async Task HandleAsyncNullTargetsTest()
        {
            LunaPowerControlManager manager = new LunaPowerControlManager();

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => manager.HandleAsync(null!));
        }


        [TestMethod]
        public async Task HandleAsyncStoresTargetsPerDeviceTest()
        {
            LunaPowerControlManager manager = new LunaPowerControlManager();
            CurrentTargetsDto first = new CurrentTargetsDto { DeviceId = 7, ActivePowerWatt = 25000.5, ReactivePowerVar = -4000.25 };
            CurrentTargetsDto second = new CurrentTargetsDto { DeviceId = 8, ActivePowerWatt = -12000, ReactivePowerVar = 3000 };

            await manager.HandleAsync(first);
            await manager.HandleAsync(second);

            Assert.AreEqual(first, manager.GetTarget(7));
            Assert.AreEqual(second, manager.GetTarget(8));
            CollectionAssert.AreEquivalent(new List<CurrentTargetsDto> { first, second }, manager.GetTargets().ToList());
        }


        [TestMethod]
        public async Task HandleAsyncReplacesTargetsOfSameDeviceTest()
        {
            LunaPowerControlManager manager = new LunaPowerControlManager();
            CurrentTargetsDto previous = new CurrentTargetsDto { DeviceId = 7, ActivePowerWatt = 25000.5, ReactivePowerVar = -4000.25 };
            CurrentTargetsDto latest = new CurrentTargetsDto { DeviceId = 7, ActivePowerWatt = 12000, ReactivePowerVar = 3000 };

            await manager.HandleAsync(previous);
            await manager.HandleAsync(latest);

            Assert.AreEqual(latest, manager.GetTarget(7));
            Assert.HasCount(1, manager.GetTargets());
        }


        [TestMethod]
        public async Task GetTargetsReturnsSnapshotTest()
        {
            LunaPowerControlManager manager = new LunaPowerControlManager();
            await manager.HandleAsync(new CurrentTargetsDto { DeviceId = 7, ActivePowerWatt = 25000.5, ReactivePowerVar = -4000.25 });
            IReadOnlyCollection<CurrentTargetsDto> snapshot = manager.GetTargets();

            await manager.HandleAsync(new CurrentTargetsDto { DeviceId = 8, ActivePowerWatt = -12000, ReactivePowerVar = 3000 });

            Assert.HasCount(1, snapshot);
        }
    }
}

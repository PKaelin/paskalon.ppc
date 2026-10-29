// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Devices.Dto.UnitTest.Ders
{
    /// <summary>
    /// Tests the values <see cref="DerBatteryStorageUnitDto"/> derives from its PCS and battery banks.
    /// </summary>
    [TestClass]
    public sealed class DerBatteryStorageUnitDtoTest
    {
        /// <summary>
        /// Confirms the communication error is set if the PCS or any battery bank has a communication error.
        /// </summary>
        /// <param name="pcsError">The communication error of the PCS.</param>
        /// <param name="firstBatteryBankError">The communication error of the first battery bank.</param>
        /// <param name="secondBatteryBankError">The communication error of the second battery bank.</param>
        /// <param name="expected">The expected communication error of the battery storage unit.</param>
        [TestMethod]
        [DataRow(false, false, false, false)]
        [DataRow(true, false, false, true)]
        [DataRow(false, true, false, true)]
        [DataRow(false, false, true, true)]
        [DataRow(true, true, true, true)]
        public void CommunicationErrorOfPcsOrBatteryBankTest(bool pcsError, bool firstBatteryBankError, bool secondBatteryBankError, bool expected)
        {
            PcsDto pcs = CreatePcs(1);
            pcs.UpdateCore(new PcsCoreDto { DeviceId = 1, CommunicationError = pcsError });
            BbDto firstBatteryBank = CreateBatteryBank(2);
            firstBatteryBank.UpdateCore(new BbCoreDto { DeviceId = 2, CommunicationError = firstBatteryBankError });
            BbDto secondBatteryBank = CreateBatteryBank(3);
            secondBatteryBank.UpdateCore(new BbCoreDto { DeviceId = 3, CommunicationError = secondBatteryBankError });
            DerBatteryStorageUnitDto unit = CreateUnit(pcs, new List<BbDto> { firstBatteryBank, secondBatteryBank });

            bool communicationError = unit.CommunicationError;

            Assert.AreEqual(expected, communicationError);
        }


        /// <summary>
        /// Confirms devices without core data are not counted as communication error.
        /// </summary>
        [TestMethod]
        public void CommunicationErrorWithoutCoreDataIsFalseTest()
        {
            DerBatteryStorageUnitDto unit = CreateUnit(CreatePcs(1), new List<BbDto> { CreateBatteryBank(2) });

            bool communicationError = unit.CommunicationError;

            Assert.IsFalse(communicationError);
        }


        /// <summary>
        /// Confirms the available powers are the available powers of the PCS.
        /// </summary>
        [TestMethod]
        public void AvailablePowersAreTakenFromPcsTest()
        {
            PcsDto pcs = CreatePcs(1);
            pcs.UpdateCore(new PcsCoreDto
            {
                DeviceId = 1,
                ActiveAvailablePower = new ActivePower(2500000),
                ReactiveAvailablePower = new ReactivePower(-1200000)
            });
            DerBatteryStorageUnitDto unit = CreateUnit(pcs, new List<BbDto>());

            ActivePower? availableActivePower = unit.AvailableActivePower;
            ReactivePower? availableReactivePower = unit.AvailableReactivePower;

            Assert.IsNotNull(availableActivePower);
            Assert.IsNotNull(availableReactivePower);
            Assert.AreEqual(2500000, availableActivePower.Value.Watts);
            Assert.AreEqual(-1200000, availableReactivePower.Value.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms the available powers are null while the PCS has no core data.
        /// </summary>
        [TestMethod]
        public void AvailablePowersWithoutPcsCoreDataAreNullTest()
        {
            DerBatteryStorageUnitDto unit = CreateUnit(CreatePcs(1), new List<BbDto>());

            ActivePower? availableActivePower = unit.AvailableActivePower;
            ReactivePower? availableReactivePower = unit.AvailableReactivePower;

            Assert.IsNull(availableActivePower);
            Assert.IsNull(availableReactivePower);
        }


        /// <summary>
        /// Confirms the maintenance mode is taken from the PCS detail, which overrides the configured value.
        /// </summary>
        /// <param name="configured">The configured maintenance mode of the unit.</param>
        /// <param name="pcsDetail">The maintenance mode of the PCS detail.</param>
        [TestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public void IsInMaintenanceModeIsTakenFromPcsDetailTest(bool configured, bool pcsDetail)
        {
            PcsDto pcs = CreatePcs(1);
            pcs.UpdateDetail(new PcsDetailDto { DeviceId = 1, IsInMaintenanceMode = pcsDetail });
            DerBatteryStorageUnitDto unit = CreateUnit(pcs, new List<BbDto>()) with { IsInMaintenanceMode = configured };

            bool isInMaintenanceMode = unit.IsInMaintenanceMode;

            Assert.AreEqual(pcsDetail, isInMaintenanceMode);
        }


        /// <summary>
        /// Confirms the configured maintenance mode applies while the PCS has no detail.
        /// </summary>
        /// <param name="configured">The configured maintenance mode of the unit.</param>
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void IsInMaintenanceModeWithoutPcsDetailIsConfiguredValueTest(bool configured)
        {
            DerBatteryStorageUnitDto unit = CreateUnit(CreatePcs(1), new List<BbDto>()) with { IsInMaintenanceMode = configured };

            bool isInMaintenanceMode = unit.IsInMaintenanceMode;

            Assert.AreEqual(configured, isInMaintenanceMode);
        }


        /// <summary>
        /// Confirms the PCS state is the state of the PCS core, null without core data.
        /// </summary>
        /// <param name="state">The state of the PCS core, null for a PCS without core data.</param>
        [TestMethod]
        [DataRow(PcsState.Started)]
        [DataRow(PcsState.Standby)]
        [DataRow(null)]
        public void PcsStateIsTakenFromPcsCoreTest(PcsState? state)
        {
            PcsDto pcs = CreatePcs(1);

            if (state is not null)
            {
                pcs.UpdateCore(new PcsCoreDto { DeviceId = 1, State = state.Value });
            }

            DerBatteryStorageUnitDto unit = CreateUnit(pcs, new List<BbDto>());

            PcsState? pcsState = unit.PcsState;

            Assert.AreEqual(state, pcsState);
        }


        /// <summary>
        /// Confirms the combined state of the battery banks. Null stands for a battery bank without core data.
        /// </summary>
        /// <param name="states">The states of the battery banks.</param>
        /// <param name="expected">The expected combined state.</param>
        [TestMethod]
        [DataRow(new object?[] { }, BatteryStorageUnitBatteryBankState.UnknownCombination)]
        [DataRow(new object?[] { BatteryBankState.Standby }, BatteryStorageUnitBatteryBankState.Standby)]
        [DataRow(new object?[] { BatteryBankState.Fault }, BatteryStorageUnitBatteryBankState.Fault)]
        [DataRow(new object?[] { null }, BatteryStorageUnitBatteryBankState.Unknown)]
        [DataRow(new object?[] { BatteryBankState.Connected, BatteryBankState.Connected }, BatteryStorageUnitBatteryBankState.Connected)]
        [DataRow(new object?[] { BatteryBankState.Connected, BatteryBankState.Fault }, BatteryStorageUnitBatteryBankState.PartiallyConnected)]
        [DataRow(new object?[] { BatteryBankState.Connected, BatteryBankState.Connected, null }, BatteryStorageUnitBatteryBankState.PartiallyConnected)]
        [DataRow(new object?[] { BatteryBankState.Disconnected, BatteryBankState.Disconnected }, BatteryStorageUnitBatteryBankState.Disconnected)]
        [DataRow(new object?[] { BatteryBankState.Disconnected, BatteryBankState.Connecting }, BatteryStorageUnitBatteryBankState.UnknownCombination)]
        [DataRow(new object?[] { BatteryBankState.Disconnected, null }, BatteryStorageUnitBatteryBankState.UnknownCombination)]
        public void BatteryBankStateCombinesBatteryBankStatesTest(object?[] states, BatteryStorageUnitBatteryBankState expected)
        {
            List<BbDto> batteryBanks = new List<BbDto>();

            for (int index = 0; index < states.Length; index++)
            {
                BbDto batteryBank = CreateBatteryBank(index + 2);

                if (states[index] is BatteryBankState state)
                {
                    batteryBank.UpdateCore(new BbCoreDto { DeviceId = index + 2, State = state });
                }

                batteryBanks.Add(batteryBank);
            }

            DerBatteryStorageUnitDto unit = CreateUnit(CreatePcs(1), batteryBanks);

            BatteryStorageUnitBatteryBankState batteryBankState = unit.BatteryBankState;

            Assert.AreEqual(expected, batteryBankState);
        }


        /// <summary>
        /// Confirms every battery bank state has a combined state with the same name and number, so a single battery bank keeps its state.
        /// </summary>
        [TestMethod]
        public void BatteryBankStateHasAllBatteryBankStatesTest()
        {
            BatteryBankState[] states = Enum.GetValues<BatteryBankState>();

            List<string> mismatches = states
                .Where(state => ((BatteryStorageUnitBatteryBankState)state).ToString() != state.ToString())
                .Select(state => state.ToString())
                .ToList();

            Assert.IsEmpty(mismatches);
        }


        /// <summary>
        /// Creates a battery storage unit.
        /// </summary>
        /// <param name="pcs">The PCS of the battery storage unit.</param>
        /// <param name="batteryBanks">The battery banks of the battery storage unit.</param>
        /// <returns>The battery storage unit DTO.</returns>
        private DerBatteryStorageUnitDto CreateUnit(PcsDto pcs, List<BbDto> batteryBanks)
        {
            return new DerBatteryStorageUnitDto { Name = "BatteryUnit1", PowerConversionSystem = pcs, BatteryBanks = batteryBanks };
        }


        /// <summary>
        /// Creates a PCS without core data.
        /// </summary>
        /// <param name="deviceId">The device ID.</param>
        /// <returns>The PCS DTO.</returns>
        private PcsDto CreatePcs(int deviceId)
        {
            return new PcsDto(new PcsDefinitionDto { DeviceId = deviceId, Name = $"Pcs{deviceId}", TargetAddress = "localhost", TargetPort = 502 });
        }


        /// <summary>
        /// Creates a battery bank without core data.
        /// </summary>
        /// <param name="deviceId">The device ID.</param>
        /// <returns>The battery bank DTO.</returns>
        private BbDto CreateBatteryBank(int deviceId)
        {
            return new BbDto(new BbDefinitionDto { DeviceId = deviceId, Name = $"Bb{deviceId}", TargetAddress = "localhost", TargetPort = 502 });
        }
    }
}

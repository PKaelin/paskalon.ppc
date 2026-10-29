// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
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

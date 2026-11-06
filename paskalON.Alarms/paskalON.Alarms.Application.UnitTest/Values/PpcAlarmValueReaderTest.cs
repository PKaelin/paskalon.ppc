// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Application.Values;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Dto.Powers;

namespace paskalON.Alarms.Application.UnitTest.Values
{
    /// <summary>
    /// Tests <see cref="PpcAlarmValueReader"/>.
    /// </summary>
    [TestClass]
    public class PpcAlarmValueReaderTest
    {
        /// <summary>
        /// The boolean properties and the dictionary entries of a PCS detail are read, boolean values as 1 and 0.
        /// </summary>
        [TestMethod]
        public void ReadPcsDetailReadsBooleansAndDictionaryEntriesTest()
        {
            PcsDetailDto pcsDetail = CreatePcsDetail();

            Dictionary<string, double> values = PpcAlarmValueReader.Read(pcsDetail);

            Assert.AreEqual(1, values["IsInMaintenanceMode"]);
            Assert.AreEqual(0, values["IsACBreakerClosed"]);
            Assert.AreEqual(1, values["FaultStates.OverTemperature"]);
            Assert.AreEqual(0, values["FaultStates.GridLoss"]);
            Assert.AreEqual(1, values["WarningStates.FanFailure"]);
            Assert.AreEqual(0, values["VendorEvents.FirmwareUpdate"]);
            Assert.AreEqual(1, values["HasActiveFaults"]);
        }


        /// <summary>
        /// Numeric properties are read as double.
        /// </summary>
        [TestMethod]
        public void ReadPcsDetailReadsNumericAsDoubleTest()
        {
            PcsDetailDto pcsDetail = CreatePcsDetail();

            Dictionary<string, double> values = PpcAlarmValueReader.Read(pcsDetail);

            Assert.AreEqual(7, values["DeviceId"]);
        }


        /// <summary>
        /// Null values and not supported types (arrays) are ignored.
        /// </summary>
        [TestMethod]
        public void ReadPcsDetailIgnoresNullAndArraysTest()
        {
            PcsDetailDto pcsDetail = CreatePcsDetail() with { IsACBreakerClosed = null };

            Dictionary<string, double> values = PpcAlarmValueReader.Read(pcsDetail);

            Assert.IsFalse(values.ContainsKey("IsACBreakerClosed"));
            Assert.IsFalse(values.Keys.Any(name => name.StartsWith("IsDcContactorClosed", StringComparison.Ordinal)));
        }


        /// <summary>
        /// The boolean and the nullable numeric properties of a PCS core are read.
        /// </summary>
        [TestMethod]
        public void ReadPcsCoreReadsBooleanAndNumericsTest()
        {
            PcsCoreDto pcsCore = CreatePcsCore();

            Dictionary<string, double> values = PpcAlarmValueReader.Read(pcsCore);

            Assert.AreEqual(7, values["DeviceId"]);
            Assert.AreEqual(1, values["CommunicationError"]);
            Assert.AreEqual(49.98, values["Frequency"]);
            Assert.AreEqual(812.5, values["DCVoltage"]);
            Assert.AreEqual(1230.4, values["DCCurrent"]);
            Assert.AreEqual(690.2, values["ACVoltage"]);
            Assert.AreEqual(1440.7, values["ACCurrent"]);
        }


        /// <summary>
        /// Nullable numeric properties without a value (e.g. lost communication) are ignored.
        /// </summary>
        [TestMethod]
        public void ReadPcsCoreIgnoresNullNumericsTest()
        {
            PcsCoreDto pcsCore = CreatePcsCore() with { Frequency = null, DCVoltage = null };

            Dictionary<string, double> values = PpcAlarmValueReader.Read(pcsCore);

            Assert.IsFalse(values.ContainsKey("Frequency"));
            Assert.IsFalse(values.ContainsKey("DCVoltage"));
            Assert.IsTrue(values.ContainsKey("ACVoltage"));
        }


        /// <summary>
        /// Enums (state) and physical units (powers) are not supported yet and ignored.
        /// </summary>
        [TestMethod]
        [DataRow("State")]
        [DataRow("ActivePower")]
        [DataRow("ActivePowerTarget")]
        [DataRow("ActiveAvailablePower")]
        [DataRow("ReactivePower")]
        [DataRow("ReactivePowerTarget")]
        [DataRow("ReactiveAvailablePower")]
        [DataRow("ApparentPower")]
        public void ReadPcsCoreIgnoresEnumAndPhysicalUnitsTest(string propertyName)
        {
            PcsCoreDto pcsCore = CreatePcsCore();

            Dictionary<string, double> values = PpcAlarmValueReader.Read(pcsCore);

            Assert.IsFalse(values.Keys.Any(name => name.StartsWith(propertyName, StringComparison.Ordinal)));
        }


        /// <summary>
        /// The setpoints of a sequenced message are read, the sequence (changes with every message) is ignored.
        /// </summary>
        [TestMethod]
        public void ReadSequencedMessageIgnoresSequenceTest()
        {
            CurrentSetpointsDto setpoints = new CurrentSetpointsDto
            {
                SetpointActivePowerWatts = 2500000,
                SetpointReactivePowerVars = -300000,
                SourceInstance = "OperatingMode-1a2b3c",
                Sequence = 4711,
                Timestamp = new DateTimeOffset(2026, 11, 6, 8, 15, 30, TimeSpan.Zero)
            };

            Dictionary<string, double> values = PpcAlarmValueReader.Read(setpoints);

            CollectionAssert.AreEquivalent(new List<string> { "SetpointActivePowerWatts", "SetpointReactivePowerVars" }, values.Keys.ToList());
            Assert.AreEqual(2500000, values["SetpointActivePowerWatts"]);
            Assert.AreEqual(-300000, values["SetpointReactivePowerVars"]);
        }


        /// <summary>
        /// The power target of a PCS published by the power control is read with its device id.
        /// </summary>
        [TestMethod]
        public void ReadPowerTargetReadsNumericsTest()
        {
            CurrentTargetsDto target = new CurrentTargetsDto { DeviceId = 7, ActivePowerWatt = 1000000, ReactivePowerVar = 200000 };

            Dictionary<string, double> values = PpcAlarmValueReader.Read(target);

            Assert.AreEqual(7, values["DeviceId"]);
            Assert.AreEqual(1000000, values["ActivePowerWatt"]);
            Assert.AreEqual(200000, values["ReactivePowerVar"]);
        }


        /// <summary>
        /// Creates a started PCS core with a communication error and measurements.
        /// </summary>
        private PcsCoreDto CreatePcsCore()
        {
            return new PcsCoreDto
            {
                DeviceId = 7,
                State = PcsState.Fault,
                CommunicationError = true,
                ActivePowerTarget = new ActivePower(1000000),
                ActivePower = new ActivePower(998500),
                ActiveAvailablePower = new ActivePower(2500000),
                ReactivePowerTarget = new ReactivePower(200000),
                ReactivePower = new ReactivePower(199200),
                ReactiveAvailablePower = new ReactivePower(1200000),
                ApparentPower = new ApparentPower(1018200),
                Frequency = 49.98,
                DCVoltage = 812.5,
                DCCurrent = 1230.4,
                ACVoltage = 690.2,
                ACCurrent = 1440.7
            };
        }


        /// <summary>
        /// Creates a PCS detail with an over temperature fault and a fan failure warning.
        /// </summary>
        private PcsDetailDto CreatePcsDetail()
        {
            return new PcsDetailDto
            {
                DeviceId = 7,
                IsInMaintenanceMode = true,
                IsACBreakerClosed = false,
                IsDcContactorClosed = new bool[] { true, false },
                FaultStates = new Dictionary<string, bool> { { "OverTemperature", true }, { "GridLoss", false } },
                WarningStates = new Dictionary<string, bool> { { "FanFailure", true } },
                VendorEvents = new Dictionary<string, bool> { { "FirmwareUpdate", false } }
            };
        }
    }
}

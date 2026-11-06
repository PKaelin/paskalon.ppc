// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Application.Stores;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;

namespace paskalON.Alarms.Application.UnitTest.Stores
{
    /// <summary>
    /// Tests <see cref="PpcAlarmValueStore"/>.
    /// </summary>
    [TestClass]
    public class PpcAlarmValueStoreTest
    {
        /// <summary>
        /// Timestamp of the first update.
        /// </summary>
        private static readonly DateTimeOffset FirstReceivedOn = new DateTimeOffset(2026, 11, 5, 10, 45, 50, TimeSpan.Zero);


        /// <summary>
        /// Timestamp of the second update.
        /// </summary>
        private static readonly DateTimeOffset SecondReceivedOn = new DateTimeOffset(2026, 11, 5, 10, 45, 51, TimeSpan.Zero);


        /// <summary>
        /// Key of PCS 7.
        /// </summary>
        private static readonly PpcAlarmKey Pcs7Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 7 };


        /// <summary>
        /// Values received the first time are changed values.
        /// </summary>
        [TestMethod]
        public void UpdateFirstValuesReturnsAllAsChangedTest()
        {
            PpcAlarmValueStore store = new PpcAlarmValueStore();
            Dictionary<string, double> values = new Dictionary<string, double> { { "FaultStates.OverTemperature", 1 }, { "FaultStates.GridLoss", 0 } };

            List<PpcAlarmKeyValue> changedValues = store.Update(Pcs7Key, values, FirstReceivedOn);

            Assert.HasCount(2, changedValues);
            Assert.IsTrue(changedValues.All(value => value.ValueChanged && value.ValueChangedOn == FirstReceivedOn));
        }


        /// <summary>
        /// Only changed values are returned, unchanged values keep their last change timestamp.
        /// </summary>
        [TestMethod]
        public void UpdateUnchangedValuesReturnsOnlyChangedTest()
        {
            PpcAlarmValueStore store = new PpcAlarmValueStore();
            store.Update(Pcs7Key, new Dictionary<string, double> { { "FaultStates.OverTemperature", 1 }, { "FaultStates.GridLoss", 0 } }, FirstReceivedOn);

            List<PpcAlarmKeyValue> changedValues = store.Update(Pcs7Key,
                new Dictionary<string, double> { { "FaultStates.OverTemperature", 0 }, { "FaultStates.GridLoss", 0 } }, SecondReceivedOn);

            Assert.HasCount(1, changedValues);
            Assert.AreEqual("FaultStates.OverTemperature", changedValues[0].Name);
            Assert.AreEqual(0, changedValues[0].Value);
            Assert.AreEqual(SecondReceivedOn, changedValues[0].ValueChangedOn);
            PpcAlarmKeyValue gridLoss = store.GetValues(Pcs7Key).Single(value => value.Name == "FaultStates.GridLoss");
            Assert.IsFalse(gridLoss.ValueChanged);
            Assert.AreEqual(FirstReceivedOn, gridLoss.ValueChangedOn);
        }


        /// <summary>
        /// The values of different publishers are stored independently.
        /// </summary>
        [TestMethod]
        public void UpdateOtherDeviceDoesNotAffectValuesTest()
        {
            PpcAlarmValueStore store = new PpcAlarmValueStore();
            PpcAlarmKey pcs8Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 8 };
            store.Update(Pcs7Key, new Dictionary<string, double> { { "FaultStates.OverTemperature", 1 } }, FirstReceivedOn);

            List<PpcAlarmKeyValue> changedValues = store.Update(pcs8Key, new Dictionary<string, double> { { "FaultStates.OverTemperature", 1 } }, SecondReceivedOn);

            Assert.HasCount(1, changedValues);
            Assert.AreEqual(1, store.GetValues(Pcs7Key).Single().Value);
            Assert.AreEqual(FirstReceivedOn, store.GetValues(Pcs7Key).Single().ValueChangedOn);
        }


        /// <summary>
        /// Values missing in an update keep their latest value.
        /// </summary>
        [TestMethod]
        public void UpdateMissingValueKeepsLatestValueTest()
        {
            PpcAlarmValueStore store = new PpcAlarmValueStore();
            store.Update(Pcs7Key, new Dictionary<string, double> { { "FaultStates.OverTemperature", 1 }, { "FaultStates.GridLoss", 1 } }, FirstReceivedOn);

            List<PpcAlarmKeyValue> changedValues = store.Update(Pcs7Key, new Dictionary<string, double> { { "FaultStates.OverTemperature", 1 } }, SecondReceivedOn);

            Assert.IsEmpty(changedValues);
            Assert.AreEqual(1, store.GetValues(Pcs7Key).Single(value => value.Name == "FaultStates.GridLoss").Value);
        }


        /// <summary>
        /// An unknown publisher has no values.
        /// </summary>
        [TestMethod]
        public void GetValuesUnknownPublisherReturnsEmptyTest()
        {
            PpcAlarmValueStore store = new PpcAlarmValueStore();

            List<PpcAlarmKeyValue> values = store.GetValues(Pcs7Key);

            Assert.IsEmpty(values);
        }
    }
}

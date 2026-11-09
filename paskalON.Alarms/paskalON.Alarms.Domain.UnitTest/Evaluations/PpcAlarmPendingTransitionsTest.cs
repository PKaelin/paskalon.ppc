// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;

namespace paskalON.Alarms.Domain.UnitTest.Evaluations
{
    /// <summary>
    /// Tests <see cref="PpcAlarmPendingTransitions"/>.
    /// </summary>
    [TestClass]
    public class PpcAlarmPendingTransitionsTest
    {
        /// <summary>
        /// Time the first value changed.
        /// </summary>
        private static readonly DateTimeOffset ChangedOn = new DateTimeOffset(2026, 11, 9, 8, 0, 0, TimeSpan.Zero);


        /// <summary>
        /// Key of PCS 7.
        /// </summary>
        private static readonly PpcAlarmKey Pcs7Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 7 };


        /// <summary>
        /// Key of PCS 8.
        /// </summary>
        private static readonly PpcAlarmKey Pcs8Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 8 };


        /// <summary>
        /// The pending transitions under test.
        /// </summary>
        private readonly PpcAlarmPendingTransitions _transitions = new PpcAlarmPendingTransitions();


        /// <summary>
        /// Starting a transition of the same kind again keeps the first one with its due time,
        /// a transition of the other kind replaces it.
        /// </summary>
        [TestMethod]
        [DataRow(PpcAlarmTransition.Raise, 3)]
        [DataRow(PpcAlarmTransition.Normalize, 10)]
        public void StartSameKindKeepsOtherKindReplacesTest(PpcAlarmTransition secondTransition, int expectedDueSeconds)
        {
            _transitions.Start(CreateTransition(Pcs7Key, 1, PpcAlarmTransition.Raise, ChangedOn, 3));

            _transitions.Start(CreateTransition(Pcs7Key, 1, secondTransition, ChangedOn.AddSeconds(5), 5));

            PpcAlarmPendingTransition? pending = _transitions.Get(Pcs7Key, 1, "CommunicationError");
            Assert.IsNotNull(pending);
            Assert.AreEqual(secondTransition, pending.Transition);
            Assert.AreEqual(ChangedOn.AddSeconds(expectedDueSeconds), pending.DueOn);
            Assert.AreEqual(1, _transitions.Count);
        }


        /// <summary>
        /// Only the publishers with an elapsed delay are due, the due transitions are taken in the order of their value changes.
        /// </summary>
        [TestMethod]
        public void TakeDueReturnsElapsedTransitionsInOrderTest()
        {
            _transitions.Start(CreateTransition(Pcs7Key, 2, PpcAlarmTransition.Raise, ChangedOn.AddSeconds(1), 3));
            _transitions.Start(CreateTransition(Pcs7Key, 1, PpcAlarmTransition.Raise, ChangedOn, 3));
            _transitions.Start(CreateTransition(Pcs7Key, 3, PpcAlarmTransition.Raise, ChangedOn, 60));
            _transitions.Start(CreateTransition(Pcs8Key, 1, PpcAlarmTransition.Raise, ChangedOn, 60));
            DateTimeOffset now = ChangedOn.AddSeconds(4);

            IReadOnlyList<PpcAlarmKey> dueKeys = _transitions.GetDueKeys(now);
            IReadOnlyList<PpcAlarmPendingTransition> due = _transitions.TakeDue(Pcs7Key, now);

            CollectionAssert.AreEqual(new List<PpcAlarmKey> { Pcs7Key }, dueKeys.ToList());
            CollectionAssert.AreEqual(new List<int> { 1, 2 }, due.Select(transition => transition.PpcAlarmConfigId).ToList());
            Assert.AreEqual(2, _transitions.Count);
        }


        /// <summary>
        /// Cancelling the transitions of a publisher keeps the transitions of the other publishers.
        /// </summary>
        [TestMethod]
        public void CancelAllRemovesTransitionsOfPublisherOnlyTest()
        {
            _transitions.Start(CreateTransition(Pcs7Key, 1, PpcAlarmTransition.Raise, ChangedOn, 3));
            _transitions.Start(CreateTransition(Pcs7Key, 2, PpcAlarmTransition.Normalize, ChangedOn, 3));
            _transitions.Start(CreateTransition(Pcs8Key, 1, PpcAlarmTransition.Raise, ChangedOn, 3));

            _transitions.CancelAll(Pcs7Key);

            Assert.AreEqual(1, _transitions.Count);
            Assert.IsNotNull(_transitions.Get(Pcs8Key, 1, "CommunicationError"));
        }


        /// <summary>
        /// The transitions of other configurations of a property (e.g. overridden by a more specific configuration) are cancelled.
        /// </summary>
        [TestMethod]
        public void CancelOthersKeepsGivenConfigurationsTest()
        {
            _transitions.Start(CreateTransition(Pcs7Key, 1, PpcAlarmTransition.Raise, ChangedOn, 3));
            _transitions.Start(CreateTransition(Pcs7Key, 2, PpcAlarmTransition.Raise, ChangedOn, 3));

            _transitions.CancelOthers(Pcs7Key, "CommunicationError", new List<int> { 2 });

            Assert.IsNull(_transitions.Get(Pcs7Key, 1, "CommunicationError"));
            Assert.IsNotNull(_transitions.Get(Pcs7Key, 2, "CommunicationError"));
        }


        /// <summary>
        /// Creates a transition of the communication error that is due after the delay.
        /// </summary>
        private static PpcAlarmPendingTransition CreateTransition(PpcAlarmKey key, int configId, PpcAlarmTransition transition,
            DateTimeOffset changedOn, int delaySeconds)
        {
            return new PpcAlarmPendingTransition
            {
                Key = key,
                PpcAlarmConfigId = configId,
                PropertyName = "CommunicationError",
                Transition = transition,
                Value = new PpcAlarmKeyValue { Name = "CommunicationError", Value = 1, ValueChanged = true, ValueChangedOn = changedOn },
                DueOn = changedOn.AddSeconds(delaySeconds)
            };
        }
    }
}

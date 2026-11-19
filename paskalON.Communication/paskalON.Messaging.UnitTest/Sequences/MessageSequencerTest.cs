// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Time.Testing;
using paskalON.Messaging.Sequences;
using System.Collections.Concurrent;

namespace paskalON.Messaging.UnitTest.Sequences
{
    [TestClass]
    public sealed class MessageSequencerTest
    {
        private const string SourceInstance = "PowerControl-ppc-core-node1-3f2b9c1d8e7a4f60b5c2d1e0a9b8c7d6";


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void ConstructorInvalidSourceInstanceThrowsTest(string? sourceInstance)
        {
            Func<MessageSequencer> act = () => new MessageSequencer(sourceInstance!, TimeProvider.System);

            Assert.Throws<ArgumentException>(act);
        }


        [TestMethod]
        public void ConstructorNullTimeProviderThrowsTest()
        {
            Func<MessageSequencer> act = () => new MessageSequencer(SourceInstance, null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void NextIncreasesSequenceAndStampsCurrentTimeTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 29, 8, 15, 0, TimeSpan.Zero));
            MessageSequencer sequencer = new MessageSequencer(SourceInstance, timeProvider);

            MessageStamp first = sequencer.Next();
            timeProvider.Advance(TimeSpan.FromMilliseconds(200));
            MessageStamp second = sequencer.Next();

            Assert.AreEqual(1, first.Sequence);
            Assert.AreEqual(2, second.Sequence);
            Assert.AreEqual(SourceInstance, first.SourceInstance);
            Assert.AreEqual(SourceInstance, second.SourceInstance);
            Assert.AreEqual(new DateTimeOffset(2026, 10, 29, 8, 15, 0, TimeSpan.Zero), first.Timestamp);
            Assert.AreEqual(new DateTimeOffset(2026, 10, 29, 8, 15, 0, 200, TimeSpan.Zero), second.Timestamp);
        }


        [TestMethod]
        public void NextConcurrentCallsCreateUniqueSequencesTest()
        {
            MessageSequencer sequencer = new MessageSequencer(SourceInstance, TimeProvider.System);
            ConcurrentBag<long> sequences = new ConcurrentBag<long>();

            Parallel.For(0, 1000, _ => sequences.Add(sequencer.Next().Sequence));

            Assert.AreEqual(1000, sequences.Distinct().Count());
            Assert.AreEqual(1, sequences.Min());
            Assert.AreEqual(1000, sequences.Max());
        }


        [TestMethod]
        public void CreateSourceInstanceIsUniquePerCallTest()
        {
            string first = MessageSequencer.CreateSourceInstance("PowerControl");
            string second = MessageSequencer.CreateSourceInstance("PowerControl");

            Assert.AreNotEqual(first, second);
            Assert.StartsWith("PowerControl-", first);
            Assert.Contains(Environment.MachineName, first);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void CreateSourceInstanceInvalidServiceNameThrowsTest(string? serviceName)
        {
            Func<string> act = () => MessageSequencer.CreateSourceInstance(serviceName!);

            Assert.Throws<ArgumentException>(act);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Messaging.Heartbeats;
using paskalON.Messaging.Sequences;

namespace paskalON.Messaging.UnitTest.Sequences
{
    [TestClass]
    public sealed class MessageOrderGuardTest
    {
        private const string PrimaryInstance = "PowerControl-ppc-core-node1-3f2b9c1d8e7a4f60b5c2d1e0a9b8c7d6";
        private const string SecondaryInstance = "PowerControl-ppc-core-node2-9a8b7c6d5e4f40312a1b0c9d8e7f6a5b";


        [TestMethod]
        public void TryAcceptNullThrowsTest()
        {
            MessageOrderGuard guard = new MessageOrderGuard();

            Action act = () => guard.TryAccept(null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        [DataRow(1L)]
        [DataRow(4711L)]
        public void TryAcceptFirstMessageAcceptedTest(long sequence)
        {
            MessageOrderGuard guard = new MessageOrderGuard();

            bool isAccepted = guard.TryAccept(CreateHeartbeat(PrimaryInstance, sequence));

            Assert.IsTrue(isAccepted);
        }


        [TestMethod]
        [DataRow(6L)]
        [DataRow(100L)]
        public void TryAcceptNewerSequenceAcceptedTest(long sequence)
        {
            MessageOrderGuard guard = new MessageOrderGuard();
            guard.TryAccept(CreateHeartbeat(PrimaryInstance, 5));

            bool isAccepted = guard.TryAccept(CreateHeartbeat(PrimaryInstance, sequence));

            Assert.IsTrue(isAccepted);
        }


        [TestMethod]
        [DataRow(5L)]
        [DataRow(4L)]
        [DataRow(1L)]
        public void TryAcceptOutdatedOrDuplicateSequenceRejectedTest(long sequence)
        {
            MessageOrderGuard guard = new MessageOrderGuard();
            guard.TryAccept(CreateHeartbeat(PrimaryInstance, 5));

            bool isAccepted = guard.TryAccept(CreateHeartbeat(PrimaryInstance, sequence));

            Assert.IsFalse(isAccepted);
        }


        [TestMethod]
        public void TryAcceptOtherSourceInstanceWithLowerSequenceAcceptedTest()
        {
            MessageOrderGuard guard = new MessageOrderGuard();
            guard.TryAccept(CreateHeartbeat(PrimaryInstance, 5000));

            bool isSecondaryAccepted = guard.TryAccept(CreateHeartbeat(SecondaryInstance, 1));
            bool isSecondaryNextAccepted = guard.TryAccept(CreateHeartbeat(SecondaryInstance, 2));
            bool isSecondaryDuplicateAccepted = guard.TryAccept(CreateHeartbeat(SecondaryInstance, 2));

            Assert.IsTrue(isSecondaryAccepted);
            Assert.IsTrue(isSecondaryNextAccepted);
            Assert.IsFalse(isSecondaryDuplicateAccepted);
        }


        private static HeartbeatDto CreateHeartbeat(string sourceInstance, long sequence)
        {
            return new HeartbeatDto
            {
                ServiceName = "PowerControl",
                SourceInstance = sourceInstance,
                Sequence = sequence,
                Timestamp = new DateTimeOffset(2026, 10, 29, 8, 15, 0, TimeSpan.Zero)
            };
        }
    }
}

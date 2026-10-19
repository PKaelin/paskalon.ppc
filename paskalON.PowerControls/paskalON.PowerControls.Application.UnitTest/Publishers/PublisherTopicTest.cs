// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.Configs;
using paskalON.PowerControls.Application.Publishers;
using paskalON.PowerControls.Domain.Configs;

namespace paskalON.PowerControls.Application.UnitTest.Publishers
{
    [TestClass]
    public sealed class PublisherTopicTest
    {
        [TestMethod]
        [DataRow("ppc:pc:targets")]
        [DataRow(null)]
        public void PublisherTopicCreateReadsSystemConfigTest(string? powerTargetTopic)
        {
            SystemConfig config = new SystemConfig
            {
                ChangedBy = "UnitTest",
                Type = PowerControlType.Bess,
                PublisherTopicPowerTarget = powerTargetTopic
            };

            PublisherTopic topic = PublisherTopic.Create(config);

            Assert.AreEqual(powerTargetTopic, topic.PowerTargetTopic);
        }


        [TestMethod]
        public void PublisherTopicCreateNullConfigThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => PublisherTopic.Create(null!));
        }
    }
}

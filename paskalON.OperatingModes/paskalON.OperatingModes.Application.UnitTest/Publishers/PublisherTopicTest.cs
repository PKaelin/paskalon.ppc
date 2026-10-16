// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Application.Publishers;
using paskalON.OperatingModes.Domain.Configs;

namespace paskalON.OperatingModes.Application.UnitTest.Publishers
{
    /// <summary>
    /// Tests <see cref="PublisherTopic"/>.
    /// </summary>
    [TestClass]
    public sealed class PublisherTopicTest
    {
        /// <summary>
        /// Confirms the topics are read from the system configuration.
        /// </summary>
        [TestMethod]
        public void PublisherTopicCreateReadsSystemConfigTest()
        {
            SystemConfig config = ApplicationTestData.CreateSystemConfig();
            config.PublisherTopicCurrentSetpoint = "ppc:om:setpoints";
            config.PublisherTopicCurrentDerUnitSetpoint = "ppc:om:unitsetpoints";

            PublisherTopic topic = PublisherTopic.Create(config);

            Assert.AreEqual("ppc:om:setpoints", topic.CurrentSetpointTopic);
            Assert.AreEqual("ppc:om:unitsetpoints", topic.CurrentDerUnitSetpointTopic);
        }


        /// <summary>
        /// Confirms a null configuration is rejected.
        /// </summary>
        [TestMethod]
        public void PublisherTopicCreateNullConfigThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => PublisherTopic.Create(null!));
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Application.Publishers;

namespace paskalON.Devices.Application.UnitTest.Publishers
{
    [TestClass]
    public class PublisherTopicEntryTest
    {
        [TestMethod]
        public void PublisherTopicEntryConstructorTest()
        {
            string coreTopic = "ppc:pcs:core";
            string detailTopic = "ppc:pcs:detail";
            string definitionTopic = "ppc:pcs:definition";

            PublisherTopicEntry entry = new PublisherTopicEntry(coreTopic, detailTopic, definitionTopic);

            Assert.AreEqual(coreTopic, entry.CoreTopic);
            Assert.AreEqual(detailTopic, entry.DetailTopic);
            Assert.AreEqual(definitionTopic, entry.DefinitionTopic);
        }


        [TestMethod]
        public void PublisherTopicEntryConstructorDefaultDefinitionTopicTest()
        {
            string coreTopic = "ppc:bb:core";
            string detailTopic = "ppc:bb:detail";

            PublisherTopicEntry entry = new PublisherTopicEntry(coreTopic, detailTopic);

            Assert.AreEqual(coreTopic, entry.CoreTopic);
            Assert.AreEqual(detailTopic, entry.DetailTopic);
            Assert.AreEqual(string.Empty, entry.DefinitionTopic);
        }


        [TestMethod]
        public void PublisherTopicEntrySetTopicsTest()
        {
            PublisherTopicEntry entry = new PublisherTopicEntry("old:core", "old:detail", "old:definition");

            entry.CoreTopic = "new:core";
            entry.DetailTopic = "new:detail";
            entry.DefinitionTopic = "new:definition";

            Assert.AreEqual("new:core", entry.CoreTopic);
            Assert.AreEqual("new:detail", entry.DetailTopic);
            Assert.AreEqual("new:definition", entry.DefinitionTopic);
        }
    }
}

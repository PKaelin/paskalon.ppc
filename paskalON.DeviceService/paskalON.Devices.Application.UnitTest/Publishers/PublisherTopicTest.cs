// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Application.Publishers;
using paskalON.Devices.Domain.Configs;
using paskalON.Domains.Configs;
using System.Reflection;

namespace paskalON.Devices.Application.UnitTest.Publishers
{
    [TestClass]
    public class PublisherTopicTest
    {
        [TestMethod]
        public void PublisherTopicConstructorAllTopicsUnsetTest()
        {
            PublisherTopic topic = new PublisherTopic();

            Assert.IsNull(topic.PowerConversionSystemTopic);
            Assert.IsNull(topic.BatteryBankTopic);
            Assert.IsNull(topic.SolarPanelTopic);
            Assert.IsNull(topic.ExternalPowerMeterTopic);
            Assert.IsNull(topic.AuxiliaryPowerMeterTopic);
            Assert.IsNull(topic.CircuitPowerMeterTopic);
            Assert.IsNull(topic.SystemPowerMeterTopic);
        }


        [TestMethod]
        public void PublisherTopicCreateNullConfigTest()
        {
            SystemConfig config = null!;

            Assert.ThrowsExactly<ArgumentNullException>(() => PublisherTopic.Create(config));
        }


        [TestMethod]
        public void PublisherTopicCreateNoTopicsConfiguredTest()
        {
            SystemConfig config = CreateConfig();

            PublisherTopic topic = PublisherTopic.Create(config);

            Assert.IsNull(topic.PowerConversionSystemTopic);
            Assert.IsNull(topic.BatteryBankTopic);
            Assert.IsNull(topic.SolarPanelTopic);
            Assert.IsNull(topic.ExternalPowerMeterTopic);
            Assert.IsNull(topic.AuxiliaryPowerMeterTopic);
            Assert.IsNull(topic.CircuitPowerMeterTopic);
            Assert.IsNull(topic.SystemPowerMeterTopic);
        }


        [TestMethod]
        public void PublisherTopicCreateAllTopicsConfiguredTest()
        {
            SystemConfig config = CreateConfig();
            config.PublisherTopicPcsCore = "ppc:pcs:core";
            config.PublisherTopicPcsDetail = "ppc:pcs:detail";
            config.PublisherTopicBatteryBankCore = "ppc:bb:core";
            config.PublisherTopicBatteryBankDetail = "ppc:bb:detail";
            config.PublisherTopicSolarPanelCore = "ppc:pv:core";
            config.PublisherTopicSolarPanelDetail = "ppc:pv:detail";
            config.PublisherTopicExternalPowerMeterCore = "ppc:pm:external:core";
            config.PublisherTopicExternalPowerMeterDetail = "ppc:pm:external:detail";
            config.PublisherTopicAuxiliaryPowerMeterCore = "ppc:pm:auxiliary:core";
            config.PublisherTopicAuxiliaryPowerMeterDetail = "ppc:pm:auxiliary:detail";
            config.PublisherTopicCircuitPowerMeterCore = "ppc:pm:circuit:core";
            config.PublisherTopicCircuitPowerMeterDetail = "ppc:pm:circuit:detail";
            config.PublisherTopicSystemPowerMeterCore = "ppc:pm:system:core";
            config.PublisherTopicSystemPowerMeterDetail = "ppc:pm:system:detail";

            PublisherTopic topic = PublisherTopic.Create(config);

            AssertEntry(topic.PowerConversionSystemTopic, "ppc:pcs:core", "ppc:pcs:detail");
            AssertEntry(topic.BatteryBankTopic, "ppc:bb:core", "ppc:bb:detail");
            AssertEntry(topic.SolarPanelTopic, "ppc:pv:core", "ppc:pv:detail");
            AssertEntry(topic.ExternalPowerMeterTopic, "ppc:pm:external:core", "ppc:pm:external:detail");
            AssertEntry(topic.AuxiliaryPowerMeterTopic, "ppc:pm:auxiliary:core", "ppc:pm:auxiliary:detail");
            AssertEntry(topic.CircuitPowerMeterTopic, "ppc:pm:circuit:core", "ppc:pm:circuit:detail");
            AssertEntry(topic.SystemPowerMeterTopic, "ppc:pm:system:core", "ppc:pm:system:detail");
        }


        [TestMethod]
        [DataRow(nameof(SystemConfig.PublisherTopicPcsCore), nameof(SystemConfig.PublisherTopicPcsDetail), nameof(PublisherTopic.PowerConversionSystemTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicBatteryBankCore), nameof(SystemConfig.PublisherTopicBatteryBankDetail), nameof(PublisherTopic.BatteryBankTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicSolarPanelCore), nameof(SystemConfig.PublisherTopicSolarPanelDetail), nameof(PublisherTopic.SolarPanelTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicExternalPowerMeterCore), nameof(SystemConfig.PublisherTopicExternalPowerMeterDetail), nameof(PublisherTopic.ExternalPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicAuxiliaryPowerMeterCore), nameof(SystemConfig.PublisherTopicAuxiliaryPowerMeterDetail), nameof(PublisherTopic.AuxiliaryPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicCircuitPowerMeterCore), nameof(SystemConfig.PublisherTopicCircuitPowerMeterDetail), nameof(PublisherTopic.CircuitPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicSystemPowerMeterCore), nameof(SystemConfig.PublisherTopicSystemPowerMeterDetail), nameof(PublisherTopic.SystemPowerMeterTopic))]
        public void PublisherTopicCreateSingleTopicConfiguredTest(string coreProperty, string detailProperty, string topicProperty)
        {
            SystemConfig config = CreateConfig();
            SetConfigTopic(config, coreProperty, "single:core");
            SetConfigTopic(config, detailProperty, "single:detail");

            PublisherTopic topic = PublisherTopic.Create(config);

            foreach (PropertyInfo property in typeof(PublisherTopic).GetProperties().Where(p => p.PropertyType == typeof(PublisherTopicEntry)))
            {
                PublisherTopicEntry? entry = (PublisherTopicEntry?)property.GetValue(topic);

                if (property.Name == topicProperty)
                {
                    AssertEntry(entry, "single:core", "single:detail");
                }
                else
                {
                    Assert.IsNull(entry, $"{property.Name} must not be created.");
                }
            }
        }


        [TestMethod]
        [DataRow(nameof(SystemConfig.PublisherTopicPcsCore), nameof(PublisherTopic.PowerConversionSystemTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicPcsDetail), nameof(PublisherTopic.PowerConversionSystemTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicBatteryBankCore), nameof(PublisherTopic.BatteryBankTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicBatteryBankDetail), nameof(PublisherTopic.BatteryBankTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicSolarPanelCore), nameof(PublisherTopic.SolarPanelTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicSolarPanelDetail), nameof(PublisherTopic.SolarPanelTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicExternalPowerMeterCore), nameof(PublisherTopic.ExternalPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicExternalPowerMeterDetail), nameof(PublisherTopic.ExternalPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicAuxiliaryPowerMeterCore), nameof(PublisherTopic.AuxiliaryPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicAuxiliaryPowerMeterDetail), nameof(PublisherTopic.AuxiliaryPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicCircuitPowerMeterCore), nameof(PublisherTopic.CircuitPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicCircuitPowerMeterDetail), nameof(PublisherTopic.CircuitPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicSystemPowerMeterCore), nameof(PublisherTopic.SystemPowerMeterTopic))]
        [DataRow(nameof(SystemConfig.PublisherTopicSystemPowerMeterDetail), nameof(PublisherTopic.SystemPowerMeterTopic))]
        public void PublisherTopicCreateIncompleteTopicPairIgnoredTest(string configuredProperty, string topicProperty)
        {
            SystemConfig config = CreateConfig();
            SetConfigTopic(config, configuredProperty, "incomplete/topic");

            PublisherTopic topic = PublisherTopic.Create(config);

            PropertyInfo? property = typeof(PublisherTopic).GetProperty(topicProperty);
            Assert.IsNotNull(property);
            Assert.IsNull(property.GetValue(topic));
        }


        [TestMethod]
        public void PublisherTopicCreateEmptyTopicsCreatesEntryTest()
        {
            SystemConfig config = CreateConfig();
            config.PublisherTopicPcsCore = string.Empty;
            config.PublisherTopicPcsDetail = string.Empty;

            PublisherTopic topic = PublisherTopic.Create(config);

            AssertEntry(topic.PowerConversionSystemTopic, string.Empty, string.Empty);
        }


        private void AssertEntry(PublisherTopicEntry? entry, string expectedCore, string expectedDetail)
        {
            Assert.IsNotNull(entry);
            Assert.AreEqual(expectedCore, entry.CoreTopic);
            Assert.AreEqual(expectedDetail, entry.DetailTopic);
            Assert.AreEqual(string.Empty, entry.DefinitionTopic);
        }


        private void SetConfigTopic(SystemConfig config, string propertyName, string value)
        {
            PropertyInfo? property = typeof(SystemConfig).GetProperty(propertyName);
            Assert.IsNotNull(property);
            property.SetValue(config, value);
        }


        private SystemConfig CreateConfig()
        {
            return new SystemConfig
            {
                ChangedBy = "UnitTest",
                Type = PowerControlType.Bess
            };
        }
    }
}

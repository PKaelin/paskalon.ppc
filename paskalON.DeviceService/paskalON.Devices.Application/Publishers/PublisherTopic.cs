// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs;

namespace paskalON.Devices.Application.Publishers
{
    /// <summary>
    /// Message publisher topic.
    /// </summary>
    public class PublisherTopic
    {
        /// <summary>
        /// Power Conversion System Topic to publish to.
        /// </summary>
        public PublisherTopicEntry? PowerConversionSystemTopic { get; set; }


        /// <summary>
        /// Battery Bank Topic to publish to.
        /// </summary>
        public PublisherTopicEntry? BatteryBankTopic { get; set; }


        /// <summary>
        /// Solar Panel Topic to publish to.
        /// </summary>
        public PublisherTopicEntry? SolarPanelTopic { get; set; }


        /// <summary>
        /// External Power Meter Topic to publish to.
        /// </summary>
        public PublisherTopicEntry? ExternalPowerMeterTopic { get; set; }


        /// <summary>
        /// Auxiliary Power Meter Topic to publish to.
        /// </summary>
        public PublisherTopicEntry? AuxiliaryPowerMeterTopic { get; set; }


        /// <summary>
        /// Circuit Power Meter Topic to publish to.
        /// </summary>
        public PublisherTopicEntry? CircuitPowerMeterTopic { get; set; }


        /// <summary>
        /// System Power Meter Topic to publish to.
        /// </summary>
        public PublisherTopicEntry? SystemPowerMeterTopic { get; set; }


        /// <summary>
        /// Creates a publisher topic with a configuration input.
        /// </summary>
        /// <param name="config">SystemConfig with publisher topic configuration.</param>
        /// <returns></returns>
        public static PublisherTopic Create(SystemConfig config)
        {
            PublisherTopic topic = new PublisherTopic();

            if (config.PublisherTopicPcsCore != null && config.PublisherTopicPcsDetail != null)
            {
                topic.PowerConversionSystemTopic = new PublisherTopicEntry(config.PublisherTopicPcsCore, config.PublisherTopicPcsDetail);
            }

            if (config.PublisherTopicBatteryBankCore != null && config.PublisherTopicBatteryBankDetail != null)
            {
                topic.BatteryBankTopic = new PublisherTopicEntry(config.PublisherTopicBatteryBankCore, config.PublisherTopicBatteryBankDetail);
            }

            if (config.PublisherTopicSolarPanelCore != null && config.PublisherTopicSolarPanelDetail != null)
            {
                topic.SolarPanelTopic = new PublisherTopicEntry(config.PublisherTopicSolarPanelCore, config.PublisherTopicSolarPanelDetail);
            }

            if (config.PublisherTopicExternalPowerMeterCore != null && config.PublisherTopicExternalPowerMeterDetail != null)
            {
                topic.ExternalPowerMeterTopic = new PublisherTopicEntry(config.PublisherTopicExternalPowerMeterCore, config.PublisherTopicExternalPowerMeterDetail);
            }

            if (config.PublisherTopicAuxiliaryPowerMeterCore != null && config.PublisherTopicAuxiliaryPowerMeterDetail != null)
            {
                topic.AuxiliaryPowerMeterTopic = new PublisherTopicEntry(config.PublisherTopicAuxiliaryPowerMeterCore, config.PublisherTopicAuxiliaryPowerMeterDetail);
            }

            if (config.PublisherTopicSystemPowerMeterCore != null && config.PublisherTopicSystemPowerMeterDetail != null)
            {
                topic.SystemPowerMeterTopic = new PublisherTopicEntry(config.PublisherTopicSystemPowerMeterCore, config.PublisherTopicSystemPowerMeterDetail);
            }

            if (config.PublisherTopicCircuitPowerMeterCore != null && config.PublisherTopicCircuitPowerMeterDetail != null)
            {
                topic.CircuitPowerMeterTopic = new PublisherTopicEntry(config.PublisherTopicCircuitPowerMeterCore, config.PublisherTopicCircuitPowerMeterDetail);
            }

            return topic;
        }
    }
}

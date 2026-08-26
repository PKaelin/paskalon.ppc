// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
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
    }
}

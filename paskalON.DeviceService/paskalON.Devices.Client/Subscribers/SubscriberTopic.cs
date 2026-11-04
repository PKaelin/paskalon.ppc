// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Client.Subscribers
{
    /// <summary>
    /// Message subscriber topic.
    /// </summary>
    public class SubscriberTopic
    {
        /// <summary>
        /// Power Conversion System Topic to subscribe to.
        /// </summary>
        public SubscriberTopicEntry? PowerConversionSystemTopic { get; set; }


        /// <summary>
        /// Battery Bank Topic to subscribe to.
        /// </summary>
        public SubscriberTopicEntry? BatteryBankTopic { get; set; }


        /// <summary>
        /// Solar Panel Topic to subscribe to.
        /// </summary>
        public SubscriberTopicEntry? SolarPanelTopic { get; set; }


        /// <summary>
        /// External Power Meter Topic to subscribe to.
        /// </summary>
        public SubscriberTopicEntry? ExternalPowerMeterTopic { get; set; }


        /// <summary>
        /// Auxiliary Power Meter Topic to subscribe to.
        /// </summary>
        public SubscriberTopicEntry? AuxiliaryPowerMeterTopic { get; set; }


        /// <summary>
        /// Circuit Power Meter Topic to subscribe to.
        /// </summary>
        public SubscriberTopicEntry? CircuitPowerMeterTopic { get; set; }


        /// <summary>
        /// System Power Meter Topic to subscribe to.
        /// </summary>
        public SubscriberTopicEntry? SystemPowerMeterTopic { get; set; }
    }
}

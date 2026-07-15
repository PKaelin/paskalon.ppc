// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Configs
{
    public class PmuLayoutMetadata
    {
        /// <summary>
        /// The unique station Id.
        /// </summary>
        public ushort StationId { get; set; }


        /// <summary>
        /// The station name.
        /// </summary>
        public string StationName { get; set; } = string.Empty;


        /// <summary>
        /// Data type of phasor data.
        /// </summary>
        public C37DataType PhasorDataType { get; set; }


        /// <summary>
        /// Data type of analog data.
        /// </summary>
        public C37DataType AnalogDataType { get; set; }


        /// <summary>
        /// Data type of frequency data.
        /// </summary>
        public C37DataType FrequencyDataType { get; set; }


        /// <summary>
        /// Number of phasors.
        /// </summary>
        public int NumberOfPhasors { get; set; }


        /// <summary>
        /// Number of analogs.
        /// </summary>
        public int NumberOfAnalogs { get; set; }


        /// <summary>
        /// Number of digitals.
        /// </summary>
        public int NumberOfDigitals { get; set; }


        /// <summary>
        /// Data frame segment start.
        /// </summary>
        public int PmuDataStartOffset { get; set; }


        /// <summary>
        /// Phasor offset in bytes calculated relative to the start of the PMU data segment.
        /// </summary>
        public int PhasorOffsetBytes => 2; // Directly follows the 2-byte status word


        /// <summary>
        /// Frequency offset in bytes calculated relative to the start of the PMU data segment.
        /// </summary>
        public int FrequencyOffsetBytes => PhasorOffsetBytes + (NumberOfPhasors * (PhasorDataType == C37DataType.Float ? 8 : 4));


        /// <summary>
        /// Analog offset in bytes calculated relative to the start of the PMU data segment.
        /// </summary>
        public int AnalogOffsetBytes => FrequencyOffsetBytes + (FrequencyDataType == C37DataType.Float ? 8 : 4);


        /// <summary>
        /// Digital offset in bytes calculated relative to the start of the PMU data segment.
        /// </summary>
        public int DigitalOffsetBytes => AnalogOffsetBytes + (NumberOfAnalogs * (AnalogDataType == C37DataType.Float ? 4 : 2));


        /// <summary>
        /// Total length of bytes in the PMU.
        /// </summary>
        public int TotalPmuLengthBytes => DigitalOffsetBytes + (NumberOfDigitals * 2);
    }
}

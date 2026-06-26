namespace paskalON.Devices.Domain.Configs.Ders
{
    /// <summary>
    /// DER group that can be split up onto different device service services 
    /// and therefore run on different machines. Logical grouping.
    /// </summary>
    public class DerGroupConfig : NameBase
    {
        /// <summary>
        /// Parent relationship to DerConfig Id.
        /// </summary>
        public int? DerConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerConfig.
        /// </summary>
        public required DerConfig DerConfig { get; set; }


        /// <summary>
        /// Child relationships to a list of DerCircuitConfig.
        /// </summary>
        public List<DerCircuitConfig> DerCircuits { get; set; } = new List<DerCircuitConfig>();

    }
}

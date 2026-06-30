
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.EnergyStorages.Batteries
{
    /// <summary>
    /// Battery bank configuration.
    /// </summary>
    public class BatteryBankConfig : DeviceIdNameBase
    {
        /// <summary>
        /// Id of the device.
        /// </summary>
        public override required int DeviceId
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
            }
        }


        /// <summary>
        /// Parent relationship to DerUnitConfig Id.
        /// </summary>
        public int DerUnitConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerUnitConfig.
        /// </summary>
        public required DerUnitConfig DerUnitConfig { get; set; }


        /// <summary>
        /// Relationship to BatteryBankDeviceConfig Id.
        /// </summary>
        public int BatteryBankDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to BatteryBankDeviceConfig.
        /// </summary>
        public required BatteryBankDeviceConfig BatteryBankDeviceConfig { get; set; }


        /// <summary>
        /// Relationship to ModbusConfig Id.
        /// </summary>
        public int ModbusConfigId { get; set; }


        /// <summary>
        /// Relationship to ModbusConfig Id.
        /// </summary>
        public required ModbusConfig ModbusConfig { get; set; }


        /// <summary>
        /// Indicates whether the battery bank is initially connected or not.
        /// </summary>
        public bool InitiallyConnected { get; set; } = true;

    }
}

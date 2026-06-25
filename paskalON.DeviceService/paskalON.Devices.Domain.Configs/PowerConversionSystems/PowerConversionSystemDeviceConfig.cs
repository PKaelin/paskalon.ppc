using System.ComponentModel.DataAnnotations;

namespace paskalON.Devices.Domain.Configs.PowerConversionSystems
{
    public class PowerConversionSystemDeviceConfig : NameBase
    {
        /// <summary>
        /// The class name of the type to instantiate.
        /// This uniquely identifies the eventually used type of component. (e.g. a ManufacturerPcs, ManufacturerBattery etc.)
        /// </summary>
        [Required]
        public string? ClassName { get; set; }


        /// <summary>
        /// Theoretical maximum active power output.
        /// </summary>
        [Required]
        public double NameplateMaximumActivePower { get; set; }


        /// <summary>
        /// Theoretical maximum reactive power output.
        /// </summary>
        [Required]
        public double NameplateMaximumReactivePower { get; set; }


        /// <summary>
        /// Theoretical maximum apparent power output.
        /// </summary>
        [Required]
        public double NameplateMaximumApparentPower { get; set; }


        /// <summary>
        /// Theoretical maximum AC current output.
        /// </summary>
        public double NameplateMaximumACCurrent { get; set; }


        /// <summary>
        /// Theoretical minimum DC voltage output.
        /// </summary>
        public double MinimumDCVoltage { get; set; }


        /// <summary>
        /// Theoretical maximum DC voltage output.
        /// </summary>
        public double MaximumDCVoltage { get; set; }


        /// <summary>
        /// Configured value determining whether the proxy should report 0 real and reactive power
        /// in the event of communication loss.
        /// </summary>
        /// <remarks>
        /// This setting is intended to keep loss compensation calculations accurate in situations where
        /// a PCS communication error is experienced and the DG-AC isn't configured to 
        /// shut down the affected DER Unit.
        /// </remarks>
        public bool ZeroOutputOnCommLoss { get; set; } = true;

    }
}

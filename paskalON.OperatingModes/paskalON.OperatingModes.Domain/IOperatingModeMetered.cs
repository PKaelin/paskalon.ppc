using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain
{
    public interface IOperatingModeMetered : IOperatingMode
    {
        /// <summary>
        /// Error adjustment calculated from input and used to make adjustments in real time.
        /// </summary>
        ComplexPower ErrorAdjustment { get; }


        /// <summary>
        /// Gets the adjusted complex power target for the operating mode.
        /// </summary>
        ComplexPower TargetAdjusted { get; }


        /// <summary>
        /// Calculates the operating modes power target.
        /// </summary>
        Task CalculateAsync(CancellationToken cancellationToken, string todo_actual_meter);
        // TODO: implement actual meter
    }
}

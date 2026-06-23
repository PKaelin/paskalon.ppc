
namespace paskalON.OperatingModes.Domain.Ramps
{
    /// <summary>
    /// Interface for Ramp controller for controlling ramps according to its configurations.
    /// </summary>
    public interface IRampController
    {
        /// <summary>
        /// Gets the start value of the ramp.
        /// </summary>
        double StartValue { get; }


        /// <summary>
        /// Gets the target value of the ramp.
        /// </summary>
        double TargetValue { get; }


        /// <summary>
        /// Gets the current value of the ramp.
        /// </summary>
        double CurrentValue { get; }


        /// <summary>
        /// Get the start date of the ramp when it was started.
        /// </summary>
        DateTimeOffset StartDate { get; }


        /// <summary>
        /// Starts the ramping.
        /// </summary>
        /// <param name="startValue">Start value of the ramp.</param>
        /// <param name="targetValue">Target value of the ramp.</param>
        void Start(double startValue, double targetValue);


        /// <summary>
        /// Stops the ramping regardless of its current position.
        /// </summary>
        void Stop();


        /// <summary>
        /// Calculate current ramp.
        /// </summary>
        /// <returns>Current value of the ramp which is also assigned to the CurrentValue property.</returns>
        /// <exception cref="NotImplementedException">Returns exception if the ramp configuration is not implemented.</exception>
        double Calculate();


        /// <summary>
        /// Calculate current ramp.
        /// </summary>
        /// <param name="precision">Precision of the current value.</param>
        /// <returns>Current value of the ramp which is also assigned to the CurrentValue property.</returns>
        /// <exception cref="NotImplementedException">Returns exception if the ramp configuration is not implemented.</exception>
        double CalculatePrecision(int precision = 3);
    }
}

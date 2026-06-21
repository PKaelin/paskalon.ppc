namespace paskalON.OperatingModes.Domain.Curves
{
    /// <summary>
    /// Voltage Fault Ride-Through (VFRT) base class.
    /// </summary>
    /// <remarks>
    /// No-Trip Zone: The curve represents the lower and upper limits of acceptable system or grid voltage.
    /// If the grid voltage dips but stays above or on the curve, the generating facility is required to "ride through" the fault.
    /// Before VFRT standards were introduced, power plants would immediately disconnect from the grid at the slightest voltage dip.
    /// While this protected individual machines, the sudden loss of thousands of megawatts of power usually caused the entire grid to collapse.
    /// </remarks>
    /// <example>
    /// Example of a Voltage Fault Ride-Through Curve connected to a 480 Volt AC grid.
    /// Point 1 (Extreme Over-Voltage) X-axis: 576 V (20% above nominal voltage) -> Y-axis: 0.16 seconds (plant Momentary Cessation/Trip within 160 milliseconds)
    /// Point 2 (Moderate Over-Voltage) X-axis: 528 V (10% above nominal voltage) -> Y-axis: 1.00 second (ride through spike)
    /// Point 3 (Moderate Under-Voltage) X-axis: 422 V (12% below nominal voltage) -> Y-axis: 2.00 seconds (ride through dip)
    /// Point 4 (Severe Under-Voltage) X-axis: 240 V (50% below nominal voltage) -> Y-axis: 1.00 second (stay connected for 1 second before Momentary Cessation/Trip)
    /// Point 5 (Near Total Blackout) X-axis: 216 V (55% below nominal voltage) -> Y-axis: 0.16 seconds (plant Momentary Cessation/Trip within 160 milliseconds)
    /// 
    /// Combine Momentary Cessation and Trip:
    /// Voltage Level:     [  Normal  ] --> [ 528V to 576V ] --------> [  Above 576V  ]
    //  Time Duration:     [  Time    ]     [Under 1 Second]           [Instant(0.16s)]
    /// PPC Action:                      (Momentary Cessation)       (Hard Trip Shutdown)
    /// </example>
    public abstract class VfrtCurveBaseConfig : CurveBaseConfig
    {
        /// <summary>
        /// X-Points represent the seconds the fault has been happening.
        /// </summary>
        public override CurveUnit XUnit { get; init; } = CurveUnit.Time;


        /// <summary>
        /// Y-Points represent the measured system or grid voltage at the point of common coupling.
        /// </summary>
        public override CurveUnit YUnit { get; init; } = CurveUnit.Voltage;

    }
}

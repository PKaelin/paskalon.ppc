namespace paskalON.OperatingModes.Domain.Curves
{
    /// <summary>
    /// VfrtHighMomentaryCessationCurve commands the plant to pause power production temporarily. 
    /// The inverters stay awake and connected to the grid but stop sending electricity.
    /// </summary>
    public class VfrtHighMomentaryCessationCurveConfig : VfrtCurveBaseConfig
    {
        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"{nameof(VfrtHighMomentaryCessationCurveConfig)} Name: {Name} XUnit: {XUnit} YUnit: {YUnit} Points: {Points.Select(p => p.ToString())}";
        }
    }
}

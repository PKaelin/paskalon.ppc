namespace paskalON.OperatingModes.Domain.Curves
{
    /// <summary>
    /// Enum to represent the X, Y curve unit.
    /// </summary>
    public enum CurveUnit
    {
        NotApplicable = 0,
        Voltage = 1,
        Frequency = 2,
        ActivePower = 3,
        ReactivePower = 4,
        ApparentPower = 5,
        PowerFactor = 6,
        Current = 7,
        Percentage = 8,
        Temperature = 9,
        Time = 10
    }
}

namespace paskalON.PhysicalUnits
{
    /// <summary>
    /// A metric prefix is a unit prefix that precedes a basic unit of measure to indicate a multiple or submultiple of the unit
    /// </summary>
    /// <remarks>
    /// Definition is the following:
    /// Name = Base 10
    /// Kilo = 10 = 10 to the power of 3 = 1000
    /// Yocto= -24 = 10 to the power of -24 =  0.000000000000000000000001
    /// </remarks>
    public enum MetricPrefix
    {
        Yocto = -24,
        Zepto = -21,
        Atto = -18,
        Femto = -15,
        Pico = -12,
        Nano = -9,
        Micro = -6,
        Milli = -3,
        Centi = -2,
        Deci = -1,
        None = 0,
        Deca = 1,
        Hecto = 2,
        Kilo = 3,
        Mega = 6,
        Giga = 9,
        Tera = 12,
        Peta = 15,
        Exa = 18,
        Zetta = 21,
        Yotta = 24
    }
}
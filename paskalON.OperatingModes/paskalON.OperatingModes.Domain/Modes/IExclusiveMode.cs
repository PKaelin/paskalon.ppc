namespace paskalON.OperatingModes.Domain.Modes
{
    /// <summary>
    /// Operating modes implementing this interface are exclusive and
    /// cannot simultaneously be enabled with other operating modes.
    /// </summary>
    public interface IExclusiveMode : IOperatingMode
    {
    }
}

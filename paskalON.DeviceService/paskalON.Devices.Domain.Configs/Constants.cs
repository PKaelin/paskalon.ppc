namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// Constants for local configuration.
    /// </summary>
    /// <remarks>
    /// Port numbers 0-1023 – Well known ports. These are allocated to server services by the Internet Assigned Numbers Authority (IANA).
    /// Ports 1024-49151- Registered Port -These can be registered for services with the IANA and should be treated as semi-reserved. 
    /// Ports 49152-65535– These are used by client programs and you are free to use these in client programs. 
    /// </remarks>
    public static class Constants
    {
        public const string Ip4Localhost = "127.0.0.1";
        public const int PortStartContainer = 4300;
        public const int PortStartPcs = 4400;
        public const int PortStartBms = 4600;
        public const int PortStartMeter = 4800;
        public const int PortStartGmd = 4900;
    }
}

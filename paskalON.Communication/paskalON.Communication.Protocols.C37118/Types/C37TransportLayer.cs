namespace paskalON.Communication.Protocols.C37118.Types
{
    /// <summary>
    /// Transport layer that dictates control logic for C37.118 communications.
    /// Also specifies the underlying protocol (TCP or UDP) C37.118 uses to communicate with remote device.
    /// </summary>
    public enum C37TransportLayer
    {
        TCP = 0,
        UDP = 1,
    }
}

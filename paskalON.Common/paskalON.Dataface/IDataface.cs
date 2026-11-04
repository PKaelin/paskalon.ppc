// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface
{
    /// <summary>
    /// IDataface is to register the data face for a decoupled communication.
    /// </summary>
    public interface IDataface
    {
        /// <summary>
        /// Name of the dataface.
        /// </summary>
        /// <remarks>
        /// Communications can have host address and such but it is still easier to have
        /// a name for the data face such as "Battery Bank 1" or "System Power Meter"
        /// </remarks>
        string Name { get; }


        /// <summary>
        /// Register a dataface.
        /// </summary>
        /// <typeparam name="TDevice">The device type that register its data interface.</typeparam>
        /// <typeparam name="TCom">The communication interface that gets registered.</typeparam>
        /// <param name="com"></param>
        /// <remarks>
        /// At the moment only <see cref="IModbusRegister"/> and <see cref="IC37Register"/> are implemented.
        /// In the future we could have more communication methods between the devices.
        /// </remarks>
        void Register<TDevice, TCom>(Action<TCom> com);
    }
}

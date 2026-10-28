// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118
{
    /// <summary>
    /// Event argument class for C37 client state changed events.
    /// </summary>
    public class C37ClientStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// C37 client state.
        /// </summary>
        public C37ClientState State { get; private set; }


        /// <summary>
        /// Constructor of <see cref="C37ClientStateChangedEventArgs"/>.
        /// </summary>
        /// <param name="state">The C37 client state.</param>
        public C37ClientStateChangedEventArgs(C37ClientState state)
        {
            State = state;
        }
    }
}

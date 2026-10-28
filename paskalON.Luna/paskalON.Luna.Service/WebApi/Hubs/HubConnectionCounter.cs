// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// Thread safe counter of the clients connected to a hub.
    /// </summary>
    public class HubConnectionCounter
    {
        /// <summary>
        /// Number of connected clients.
        /// </summary>
        private int _count;


        /// <summary>
        /// Value indicating whether at least one client is connected.
        /// </summary>
        public bool HasConnections { get => Volatile.Read(ref _count) > 0; }


        /// <summary>
        /// Counts a connected client.
        /// </summary>
        public void Add()
        {
            Interlocked.Increment(ref _count);
        }


        /// <summary>
        /// Counts a disconnected client.
        /// </summary>
        public void Remove()
        {
            Interlocked.Decrement(ref _count);
        }
    }
}

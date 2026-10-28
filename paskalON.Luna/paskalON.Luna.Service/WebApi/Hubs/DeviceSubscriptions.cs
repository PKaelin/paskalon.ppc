// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Collections.Concurrent;

namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// Thread safe register of the device each client of the <see cref="DeviceHub"/> subscribed to.
    /// </summary>
    /// <remarks>
    /// A client subscribes to one device at a time, the device shown in the content area of the UI.
    /// </remarks>
    public class DeviceSubscriptions
    {
        /// <summary>
        /// Subscription of each connection, by connection ID.
        /// </summary>
        private readonly ConcurrentDictionary<string, DeviceSubscription> _subscriptions = new ConcurrentDictionary<string, DeviceSubscription>();


        /// <summary>
        /// Value indicating whether no client subscribed to a device.
        /// </summary>
        public bool IsEmpty { get => _subscriptions.IsEmpty; }


        /// <summary>
        /// Subscribes a connection to a device, replacing its previous subscription.
        /// </summary>
        /// <param name="connectionId">ID of the connection.</param>
        /// <param name="subscription">The device the connection subscribes to.</param>
        public void Subscribe(string connectionId, DeviceSubscription subscription)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
            ArgumentNullException.ThrowIfNull(subscription);

            _subscriptions[connectionId] = subscription;
        }


        /// <summary>
        /// Removes the subscription of a connection, if it has one.
        /// </summary>
        /// <param name="connectionId">ID of the connection.</param>
        public void Unsubscribe(string connectionId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

            _subscriptions.TryRemove(connectionId, out _);
        }


        /// <summary>
        /// Gets a snapshot of all subscriptions.
        /// </summary>
        /// <returns>The subscriptions with the ID of their connection.</returns>
        public IReadOnlyList<KeyValuePair<string, DeviceSubscription>> GetAll()
        {
            return _subscriptions.ToArray();
        }
    }
}

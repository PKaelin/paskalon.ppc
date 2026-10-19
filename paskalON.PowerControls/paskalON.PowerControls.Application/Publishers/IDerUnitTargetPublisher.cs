// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Application.Publishers
{
    /// <summary>
    /// Publishes the power targets of DER units to a message broker.
    /// Every time a registered unit receives new targets, they are published for the unit's power conversion system (PCS).
    /// A stopped or standby unit is started first and the targets are published once it reached the started state.
    /// </summary>
    public interface IDerUnitTargetPublisher : IAsyncDisposable
    {
        /// <summary>
        /// Initializes the publisher with the topics to publish to. Must be called before any unit is registered.
        /// </summary>
        /// <param name="topics">Message publisher topics.</param>
        void Initialize(PublisherTopic topics);


        /// <summary>
        /// Registers a DER unit so that all its future power targets are published.
        /// </summary>
        /// <param name="unit">The DER unit power control to register.</param>
        void Register(IDerUnitPowerControl unit);
    }
}

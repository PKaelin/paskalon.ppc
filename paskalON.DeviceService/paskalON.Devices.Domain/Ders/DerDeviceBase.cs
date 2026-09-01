// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Domains;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.Ders
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Base class for all distributed energy resources (DERs) that symbolizes a device.
    /// </summary>
    public abstract class DerDeviceBase : DerBase
    {
        /// <summary>
        /// Interface for registering and publishing metrics for a given type T.
        /// </summary>
        public IMetricsPublisher MetricsPublisher { get; init; }


        /// <summary>
        ///  IDataface is to register the data face for a decoupled communication.
        /// </summary>
        public IDataface Dataface { get; init; }


        /// <summary>
        /// Constructor of <see cref="DerDeviceBase{T}"/>
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="nameBase">The name base configuration.</param>
        /// <param name="publisher">The metrics publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        protected DerDeviceBase(ILogger logger, NameBase nameBase, IMetricsPublisher publisher, IDataface dataface) : base(logger, nameBase)
        {
            ArgumentNullException.ThrowIfNull(nameBase);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(dataface);

            MetricsPublisher = publisher;
            Dataface = dataface;
        }


        /// <summary>
        /// Register metrics at the publisher.
        /// </summary>
        protected abstract void RegisterMetrics();


        /// <summary>
        /// Register the data interface for interface separations.
        /// </summary>
        protected abstract void RegisterDataface();
    }
}

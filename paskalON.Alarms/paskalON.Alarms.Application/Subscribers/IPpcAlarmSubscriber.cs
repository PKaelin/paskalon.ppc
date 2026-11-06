// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Devices.Dto;

namespace paskalON.Alarms.Application.Subscribers
{
    /// <summary>
    /// Subscribes to the published data of the other services and queues it for the alarm evaluation.
    /// </summary>
    public interface IPpcAlarmSubscriber
    {
        /// <summary>
        /// Subscribes to the published data of devices, e.g. the PcsDetailDto of the device service.
        /// </summary>
        /// <typeparam name="TDevice">Type of the published device data.</typeparam>
        /// <param name="topic">Topic of the published data, e.g. ppc:pcs:detail.</param>
        /// <param name="ppcAlarmType">PPC alarm type of the devices, e.g. PcsAlarm.</param>
        void SubscribeDevice<TDevice>(string topic, PpcAlarmType ppcAlarmType) where TDevice : class, IDevice;
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface;

namespace paskalON.Devices.Domain.PowerConversionSystems
{
    public class PowerConversionSystemDevice : IPowerConversionSystem
    {
        public IDataface Dataface { get; init; }


        public PowerConversionSystemDevice(IDataface dataface)
        {
            Dataface = dataface;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Standby(double? standbyActivePower = null)
        {
            throw new NotImplementedException();
        }

        public void SetActivePowerTarget(double? value)
        {
            throw new NotImplementedException();
        }

        public void SetReactivePowerTarget(double? value)
        {
            throw new NotImplementedException();
        }
    }
}
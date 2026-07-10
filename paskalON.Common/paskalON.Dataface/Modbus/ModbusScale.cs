// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// Class to define Modbus scales.
    /// </summary>
    /// <remarks>
    /// Modbus register typically can only hold whole numbers.
    /// Modbus scaling is the method of multiplying these raw values by a specific factor.
    /// </remarks>
    public class ModbusScale
    {
        /// <summary>
        /// No scaling.
        /// </summary>
        public const double NoScale = 1;
        /// <summary>
        /// Modbus value / 10
        /// </summary>
        public const double Factor10 = 0.1;
        /// <summary>
        /// Modbus value / 100
        /// </summary>
        public const double Factor100 = 0.01;
        /// <summary>
        /// Modbus value / 1000
        /// </summary>
        public const double Factor1000 = 0.001;
        /// <summary>
        /// Modbus value / 10000
        /// </summary>
        public const double Factor10000 = 0.0001;
    }
}

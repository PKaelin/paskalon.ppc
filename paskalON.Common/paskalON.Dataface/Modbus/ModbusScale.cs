// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
    /// <example>
    ///  Upscale: To send a temperature of 22.56 Celsius you multiply by 100. The Modbus register stores this as the integer (2256).
    ///  Downscale: You read the integer 2256 from the register. You multiply this by 0.01 and your domain value becomes 22.56
    /// </example>
    public class ModbusScale
    {
        /// <summary>
        /// No scaling.
        /// </summary>
        public const double NoScale = 1;
        /// <summary>
        /// Modbus value / 10
        /// </summary>
        public const double Downscale10 = 0.1;
        /// <summary>
        /// Modbus value / 100
        /// </summary>
        public const double Downscale100 = 0.01;
        /// <summary>
        /// Modbus value / 1000
        /// </summary>
        public const double Downscale1000 = 0.001;
        /// <summary>
        /// Modbus value / 10000
        /// </summary>
        public const double Downscale10000 = 0.0001;
        /// <summary>
        /// Modbus value * 10
        /// </summary>
        public const double Upscale10 = 10;
        /// <summary>
        /// Modbus value * 100
        /// </summary>
        public const double Upscale100 = 100;
        /// <summary>
        /// Modbus value * 1000
        /// </summary>
        public const double Upscale1000 = 1000;
        /// <summary>
        /// Modbus value * 10000
        /// </summary>
        public const double Upscale10000 = 10000;
    }
}

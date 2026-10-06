// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData
{
    /// <summary>
    /// A sample device entity persisted by the <see cref="SampleContext"/>.
    /// </summary>
    public class SampleDevice
    {
        /// <summary>
        /// The maximum length of the device name.
        /// </summary>
        public const int NameMaxLength = 100;


        /// <summary>
        /// Gets or sets the unique identifier of the device.
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// Gets or sets the unique name of the device.
        /// </summary>
        public string Name { get; set; } = string.Empty;


        /// <summary>
        /// Gets or sets the rated power of the device in kilowatt.
        /// </summary>
        public double RatedPowerInKilowatt { get; set; }
    }
}

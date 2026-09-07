// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using System.Net.Http.Json;

namespace paskalON.Devices.Client
{
    /// <summary>
    /// Device server for device service actions.
    /// </summary>
    public class DeviceServer : IDeviceServer
    {
        /// <summary>
        /// Endpoint address.
        /// </summary>
        private readonly string _address;


        /// <summary>
        /// Constructor of <see cref="DeviceServer"/>.
        /// </summary>
        /// <param name="address">Endpoint address.</param>
        public DeviceServer(string address)
        {
            ArgumentNullException.ThrowIfNull(address);

            _address = address;
        }


        /// <summary>
        /// Gets the DER DTO root object and all its content.
        /// </summary>
        /// <returns>The DER DTO root object and all its content.</returns>
        /// <exception cref="InvalidOperationException">Throws an invalid operation exception when the DER is null.</exception>
        public async Task<DerDto> GetDer()
        {
            HttpClient _client = new HttpClient();
            HttpResponseMessage response = await _client.GetAsync(_address);
            response.EnsureSuccessStatusCode();

            DerDto? dto = await response.Content.ReadFromJsonAsync<DerDto>();

            if (dto == null)
            {
                throw new InvalidOperationException("Device server returned an empty DER data transfer object");
            }

            return dto;
        }
    }
}

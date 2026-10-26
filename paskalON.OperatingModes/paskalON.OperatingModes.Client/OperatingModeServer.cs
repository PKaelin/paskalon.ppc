// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Dto.Stacks;
using paskalON.OperatingModes.Service.Dto.V1.Requests;
using System.Net.Http.Json;

namespace paskalON.OperatingModes.Client
{
    /// <summary>
    /// Operating mode server for operating mode service actions.
    /// </summary>
    public class OperatingModeServer : IOperatingModeServer
    {
        /// <summary>
        /// Relative path of the get available operating modes endpoint.
        /// </summary>
        private const string GetAvailableOperatingModesPath = "stack/getavailableoperatingmodes";


        /// <summary>
        /// Relative path of the get active operating modes endpoint.
        /// </summary>
        private const string GetActiveOperatingModesPath = "stack/getactiveoperatingmodes";


        /// <summary>
        /// Relative path of the add operating mode endpoint.
        /// </summary>
        private const string AddOperatingModePath = "stack/addoperatingmode";


        /// <summary>
        /// Relative path of the move operating mode endpoint.
        /// </summary>
        private const string MoveOperatingModePath = "stack/moveoperatingmode";


        /// <summary>
        /// Relative path of the remove operating mode endpoint.
        /// </summary>
        private const string RemoveOperatingModePath = "stack/removeoperatingmode";


        /// <summary>
        /// Relative path of the replace operating mode endpoint.
        /// </summary>
        private const string ReplaceOperatingModePath = "stack/replaceoperatingmode";


        /// <summary>
        /// Relative path of the set system setpoint endpoint.
        /// </summary>
        private const string SetSystemSetpointPath = "system/setsetpoint";


        /// <summary>
        /// HTTP client with the base address of the operating mode service API.
        /// </summary>
        private readonly HttpClient _client;


        /// <summary>
        /// Constructor of <see cref="OperatingModeServer"/>.
        /// </summary>
        /// <param name="client">HTTP client with the base address of the operating mode service API (e.g. http://host:8080/api/v1/).</param>
        public OperatingModeServer(HttpClient client)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(client.BaseAddress);

            _client = client;
        }


        /// <inheritdoc/>
        public async Task<IReadOnlyList<OperatingModeStackDto>> GetAvailableOperatingModes(CancellationToken cancellationToken = default)
        {
            return await GetOperatingModes(GetAvailableOperatingModesPath, cancellationToken);
        }


        /// <inheritdoc/>
        public async Task<IReadOnlyList<OperatingModeStackDto>> GetActiveOperatingModes(CancellationToken cancellationToken = default)
        {
            return await GetOperatingModes(GetActiveOperatingModesPath, cancellationToken);
        }


        /// <inheritdoc/>
        public async Task AddOperatingMode(AddOperatingModeStackRequest request, CancellationToken cancellationToken = default)
        {
            await Post(AddOperatingModePath, request, cancellationToken);
        }


        /// <inheritdoc/>
        public async Task MoveOperatingMode(MoveOperatingModeStackRequest request, CancellationToken cancellationToken = default)
        {
            await Post(MoveOperatingModePath, request, cancellationToken);
        }


        /// <inheritdoc/>
        public async Task RemoveOperatingMode(RemoveOperatingModeStackRequest request, CancellationToken cancellationToken = default)
        {
            await Post(RemoveOperatingModePath, request, cancellationToken);
        }


        /// <inheritdoc/>
        public async Task ReplaceOperatingMode(ReplaceOperatingModeStackRequest request, CancellationToken cancellationToken = default)
        {
            await Post(ReplaceOperatingModePath, request, cancellationToken);
        }


        /// <inheritdoc/>
        public async Task SetSystemSetpoint(SetSystemSetpointRequest request, CancellationToken cancellationToken = default)
        {
            await Post(SetSystemSetpointPath, request, cancellationToken);
        }


        /// <summary>
        /// Gets the stack DTOs of an endpoint.
        /// </summary>
        /// <remarks>
        /// The stack DTOs are deserialized polymorphic, so the specialized DTOs keep their properties.
        /// </remarks>
        /// <param name="path">The relative path of the endpoint.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The stack DTOs.</returns>
        /// <exception cref="InvalidOperationException">The operating mode server returned no content.</exception>
        private async Task<IReadOnlyList<OperatingModeStackDto>> GetOperatingModes(string path, CancellationToken cancellationToken)
        {
            using HttpResponseMessage response = await _client.GetAsync(path, cancellationToken);
            response.EnsureSuccessStatusCode();

            List<OperatingModeStackDto>? dtos = await response.Content.ReadFromJsonAsync<List<OperatingModeStackDto>>(cancellationToken);

            if (dtos == null)
            {
                throw new InvalidOperationException("Operating mode server returned an empty list of operating mode stack data transfer objects");
            }

            return dtos;
        }


        /// <summary>
        /// Posts a request to an endpoint.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="path">The relative path of the endpoint.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        private async Task Post<TRequest>(string path, TRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            using HttpResponseMessage response = await _client.PostAsJsonAsync(path, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}

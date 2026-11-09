// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Loads the entities of supplementary files that belong to a client project file, e.g. the localized texts.
    /// </summary>
    public interface IClientDataSupplement
    {
        /// <summary>
        /// The entity types that are only defined in the supplementary files and therefore not allowed in the client project file.
        /// </summary>
        IReadOnlyCollection<Type> EntityTypes { get; }


        /// <summary>
        /// Loads and validates the entities of the supplementary files of the client project file.
        /// </summary>
        /// <param name="clientProjectFile">The client project file.</param>
        /// <param name="projectEntities">The entities of the client project file with their positions.</param>
        /// <param name="changedDate">The date all the entities are changed at.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The entities of the supplementary files.</returns>
        /// <exception cref="ClientDataValidationException">Thrown with all the errors if a supplementary file or a reference to it is not valid.</exception>
        Task<IReadOnlyList<object>> LoadAsync(string clientProjectFile, IReadOnlyList<ClientDataEntity> projectEntities, DateTimeOffset changedDate,
            CancellationToken cancellationToken = default);
    }
}

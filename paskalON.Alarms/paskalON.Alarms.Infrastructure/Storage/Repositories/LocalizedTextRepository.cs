// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Domain.Configs.Localizations;
using paskalON.Infrastructure.Repositories;

namespace paskalON.Alarms.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Localized text repository for reading the texts of the localization keys from the database.
    /// </summary>
    public class LocalizedTextRepository : RepositoryBase<AlarmContext>, ILocalizedTextRepository
    {
        /// <summary>
        /// Constructor of <see cref="LocalizedTextRepository"/>.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="context">The alarm context.</param>
        public LocalizedTextRepository(ILogger<LocalizedTextRepository> logger, AlarmContext context) : base(logger, context)
        {
        }


        /// <inheritdoc/>
        public async Task<List<LocalizedText>> GetAsync(IReadOnlyCollection<string> languages, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(languages);

            List<string> languageList = languages.ToList();

            return await Context.LocalizedTexts
                .Where(text => languageList.Contains(text.Language))
                .OrderBy(text => text.Id)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}

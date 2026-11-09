// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Alarms.Domain.Configs.Localizations;
using paskalON.Alarms.Infrastructure.Storage;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Infrastructure.IntegrationTest.Storage
{
    /// <summary>
    /// Tests <see cref="LocalizedTextRepository"/> against a migrated PostgreSQL database.
    /// </summary>
    [TestClass]
    public class LocalizedTextRepositoryTest : DatabaseTestBase
    {
        /// <summary>
        /// Seeds the English, German and French texts of the PCS communication error.
        /// </summary>
        protected override async Task SeedAsync(AlarmContext db)
        {
            // override DatabaseTestBase -> PostgresTestBase
            db.LocalizedTexts.Add(CreateText("PCS.CommunicationError.Name", "en", "PCS communication error"));
            db.LocalizedTexts.Add(CreateText("PCS.CommunicationError.Desc", "en", "The PPC cannot communicate with the power conversion system."));
            db.LocalizedTexts.Add(CreateText("PCS.CommunicationError.Name", "de", "PCS Kommunikationsfehler"));
            db.LocalizedTexts.Add(CreateText("PCS.CommunicationError.Name", "fr", "Erreur de communication PCS"));
            await db.SaveChangesAsync();
        }


        /// <summary>
        /// Only the texts of the requested languages are returned.
        /// </summary>
        /// <param name="languages">Requested languages.</param>
        /// <param name="expectedTexts">Expected texts as language:key.</param>
        [TestMethod]
        [DataRow(new string[] { "en" }, new string[] { "en:PCS.CommunicationError.Name", "en:PCS.CommunicationError.Desc" })]
        [DataRow(new string[] { "de", "en" }, new string[] { "de:PCS.CommunicationError.Name", "en:PCS.CommunicationError.Name", "en:PCS.CommunicationError.Desc" })]
        [DataRow(new string[] { "it" }, new string[] { })]
        public async Task GetAsyncReturnsTextsOfLanguagesTest(string[] languages, string[] expectedTexts)
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            LocalizedTextRepository repository = new LocalizedTextRepository(NullLogger<LocalizedTextRepository>.Instance, context);

            List<LocalizedText> texts = await repository.GetAsync(languages);

            CollectionAssert.AreEquivalent(expectedTexts, texts.Select(text => $"{text.Language}:{text.Name}").ToArray());
            Assert.IsEmpty(context.ChangeTracker.Entries());
        }


        /// <summary>
        /// A key can only have one text per language.
        /// </summary>
        [TestMethod]
        public async Task SaveDuplicatedKeyOfLanguageThrowsTest()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            context.LocalizedTexts.Add(CreateText("PCS.CommunicationError.Name", "en", "Communication error of the PCS"));

            Func<Task> save = () => context.SaveChangesAsync();

            await Assert.ThrowsExactlyAsync<DbUpdateException>(save);
        }


        /// <summary>
        /// Creates a text of the system initialization.
        /// </summary>
        /// <param name="name">Localization key.</param>
        /// <param name="language">Language of the text.</param>
        /// <param name="text">Text of the key.</param>
        /// <returns>The localized text.</returns>
        private LocalizedText CreateText(string name, string language, string text)
        {
            return new LocalizedText { ChangedBy = "System Init", Name = name, Language = language, Text = text };
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Alarms.Application.Localizations;
using paskalON.Alarms.Domain.Configs.Localizations;
using paskalON.Alarms.Dto.Localizations;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.UnitTest.Localizations
{
    /// <summary>
    /// Tests <see cref="LocalizedTextQueryHandler"/> with a mocked text repository.
    /// </summary>
    [TestClass]
    public class LocalizedTextQueryHandlerTest
    {
        /// <summary>
        /// Mocked text repository.
        /// </summary>
        private readonly Mock<ILocalizedTextRepository> _textRepository = new Mock<ILocalizedTextRepository>();


        /// <summary>
        /// The constructor validates the repository.
        /// </summary>
        [TestMethod]
        public void LocalizedTextQueryHandlerConstructorWithoutRepositoryThrowsTest()
        {
            Action create = () => new LocalizedTextQueryHandler(null!);

            Assert.ThrowsExactly<ArgumentNullException>(create);
        }


        /// <summary>
        /// The English texts are read for English only.
        /// </summary>
        [TestMethod]
        public async Task GetTextsAsyncDefaultLanguageReturnsEnglishTextsTest()
        {
            IReadOnlyCollection<string>? passedLanguages = null;
            _textRepository.Setup(repository => repository.GetAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
                .Callback<IReadOnlyCollection<string>, CancellationToken>((languages, _) => passedLanguages = languages)
                .ReturnsAsync(new List<LocalizedText>
                {
                    CreateText("PCS.CommunicationError.Name", "en", "PCS communication error"),
                    CreateText("PCS.CommunicationError.Desc", "en", "The PPC cannot communicate with the power conversion system.")
                });
            LocalizedTextQueryHandler handler = new LocalizedTextQueryHandler(_textRepository.Object);

            LocalizedTextsDto texts = await handler.GetTextsAsync("en");

            CollectionAssert.AreEqual(new string[] { "en" }, passedLanguages!.ToArray());
            Assert.AreEqual("en", texts.Language);
            Assert.HasCount(2, texts.Texts);
            Assert.AreEqual("PCS communication error", texts.Texts["PCS.CommunicationError.Name"]);
            Assert.AreEqual("The PPC cannot communicate with the power conversion system.", texts.Texts["PCS.CommunicationError.Desc"]);
        }


        /// <summary>
        /// The texts of another language replace the English texts, a key without a text in the language keeps its English text.
        /// </summary>
        [TestMethod]
        public async Task GetTextsAsyncOtherLanguageFallsBackToEnglishTest()
        {
            IReadOnlyCollection<string>? passedLanguages = null;
            _textRepository.Setup(repository => repository.GetAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
                .Callback<IReadOnlyCollection<string>, CancellationToken>((languages, _) => passedLanguages = languages)
                .ReturnsAsync(new List<LocalizedText>
                {
                    CreateText("PCS.CommunicationError.Name", "de", "PCS Kommunikationsfehler"),
                    CreateText("PCS.CommunicationError.Name", "en", "PCS communication error"),
                    CreateText("PCS.CommunicationError.Desc", "en", "The PPC cannot communicate with the power conversion system.")
                });
            LocalizedTextQueryHandler handler = new LocalizedTextQueryHandler(_textRepository.Object);

            LocalizedTextsDto texts = await handler.GetTextsAsync("de");

            CollectionAssert.AreEquivalent(new string[] { "de", "en" }, passedLanguages!.ToArray());
            Assert.AreEqual("de", texts.Language);
            Assert.HasCount(2, texts.Texts);
            Assert.AreEqual("PCS Kommunikationsfehler", texts.Texts["PCS.CommunicationError.Name"]);
            Assert.AreEqual("The PPC cannot communicate with the power conversion system.", texts.Texts["PCS.CommunicationError.Desc"]);
        }


        /// <summary>
        /// A language with an invalid format is rejected without reading the texts.
        /// </summary>
        /// <param name="language">Invalid language.</param>
        [TestMethod]
        [DataRow("")]
        [DataRow("english")]
        [DataRow("EN")]
        public async Task GetTextsAsyncInvalidLanguageThrowsTest(string language)
        {
            LocalizedTextQueryHandler handler = new LocalizedTextQueryHandler(_textRepository.Object);

            Func<Task> getTexts = () => handler.GetTextsAsync(language);

            await Assert.ThrowsExactlyAsync<ArgumentException>(getTexts);
            _textRepository.Verify(repository => repository.GetAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// Creates a text of the system initialization.
        /// </summary>
        /// <param name="name">Localization key.</param>
        /// <param name="language">Language of the text.</param>
        /// <param name="text">Text of the key.</param>
        /// <returns>The localized text.</returns>
        private static LocalizedText CreateText(string name, string language, string text)
        {
            return new LocalizedText { ChangedBy = "System Init", Name = name, Language = language, Text = text };
        }
    }
}

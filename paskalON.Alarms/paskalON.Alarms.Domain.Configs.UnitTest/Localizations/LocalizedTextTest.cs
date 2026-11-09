// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Localizations;

namespace paskalON.Alarms.Domain.Configs.UnitTest.Localizations
{
    /// <summary>
    /// Tests <see cref="LocalizedText"/>.
    /// </summary>
    [TestClass]
    public class LocalizedTextTest
    {
        /// <summary>
        /// A text with a valid key, language and text keeps its values.
        /// </summary>
        /// <param name="language">Valid language.</param>
        [TestMethod]
        [DataRow("en")]
        [DataRow("de")]
        [DataRow("de-CH")]
        public void LocalizedTextValidValuesAreKeptTest(string language)
        {
            LocalizedText text = CreateText();

            text.Language = language;

            Assert.AreEqual("PCS.CommunicationError.Name", text.Name);
            Assert.AreEqual(language, text.Language);
            Assert.AreEqual("PCS communication error", text.Text);
        }


        /// <summary>
        /// A language that is not a lowercase language code with an optional uppercase region code is rejected.
        /// </summary>
        /// <param name="language">Invalid language.</param>
        [TestMethod]
        [DataRow("")]
        [DataRow("EN")]
        [DataRow("eng")]
        [DataRow("de-ch")]
        [DataRow("de_CH")]
        [DataRow("en ")]
        public void LocalizedTextInvalidLanguageThrowsTest(string language)
        {
            LocalizedText text = CreateText();

            Action setLanguage = () => text.Language = language;

            Assert.ThrowsExactly<ArgumentException>(setLanguage);
        }


        /// <summary>
        /// A key that is empty or written with braces is rejected.
        /// </summary>
        /// <param name="name">Invalid key.</param>
        [TestMethod]
        [DataRow(" ")]
        [DataRow("{{PCS.CommunicationError.Name}}")]
        [DataRow("PCS.{CommunicationError}.Name")]
        public void LocalizedTextInvalidNameThrowsTest(string name)
        {
            LocalizedText text = CreateText();

            Action setName = () => text.Name = name;

            Assert.Throws<ArgumentException>(setName);
        }


        /// <summary>
        /// An empty text is rejected.
        /// </summary>
        [TestMethod]
        public void LocalizedTextEmptyTextThrowsTest()
        {
            LocalizedText text = CreateText();

            Action setText = () => text.Text = " ";

            Assert.ThrowsExactly<ArgumentException>(setText);
        }


        /// <summary>
        /// The format of the language is checked without an instance, e.g. for a requested language.
        /// </summary>
        /// <param name="language">Language to check.</param>
        /// <param name="expectedIsValid">Whether the language is expected to be valid.</param>
        [TestMethod]
        [DataRow("en", true)]
        [DataRow("fr-CH", true)]
        [DataRow(null, false)]
        [DataRow("english", false)]
        public void IsValidLanguageChecksFormatTest(string? language, bool expectedIsValid)
        {
            bool isValid = LocalizedText.IsValidLanguage(language);

            Assert.AreEqual(expectedIsValid, isValid);
        }


        /// <summary>
        /// Creates the English text of the PCS communication error name.
        /// </summary>
        /// <returns>The text.</returns>
        private static LocalizedText CreateText()
        {
            return new LocalizedText
            {
                ChangedBy = "Test",
                Name = "PCS.CommunicationError.Name",
                Language = LocalizedText.DefaultLanguage,
                Text = "PCS communication error"
            };
        }
    }
}

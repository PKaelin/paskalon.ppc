// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common.UnitTest
{
    /// <summary>
    /// Tests of the class <see cref="LocalizedTextSupplement{TText}"/> with in-memory text files.
    /// </summary>
    [TestClass]
    public class LocalizedTextSupplementTest
    {
        private const string ProjectFile = "DemoBattery.csv";
        private const string EnglishFile = "DemoBattery.Texts.en.csv";
        private const string GermanFile = "DemoBattery.Texts.de.csv";
        private const string NameText = "Type=TextEntity,Name=PCS.CommunicationError.Name,Language=en,Text=PCS communication error";
        private const string DescText = "Type=TextEntity,Name=PCS.CommunicationError.Desc,Language=en,Text=\"The PPC cannot communicate with the PCS, check the cable.\"";

        private static readonly DateTimeOffset _changedDate = new DateTimeOffset(2026, 11, 9, 10, 0, 0, TimeSpan.Zero);


        [TestMethod]
        public void LocalizedTextSupplementConstructorInvalidArgumentsThrowsTest()
        {
            Action withoutKey = () => new LocalizedTextSupplement<TextEntity>(null!, x => x.Language, GetLocalizableValues, "en");
            Action withoutLanguage = () => new LocalizedTextSupplement<TextEntity>(x => x.Name, null!, GetLocalizableValues, "en");
            Action withoutValues = () => new LocalizedTextSupplement<TextEntity>(x => x.Name, x => x.Language, null!, "en");
            Action withoutDefaultLanguage = () => new LocalizedTextSupplement<TextEntity>(x => x.Name, x => x.Language, GetLocalizableValues, " ");

            Assert.ThrowsExactly<ArgumentNullException>(withoutKey);
            Assert.ThrowsExactly<ArgumentNullException>(withoutLanguage);
            Assert.ThrowsExactly<ArgumentNullException>(withoutValues);
            Assert.ThrowsExactly<ArgumentException>(withoutDefaultLanguage);
        }


        [TestMethod]
        public void EntityTypesIsTextTypeTest()
        {
            LocalizedTextSupplement<TextEntity> supplement = CreateSupplement();

            IReadOnlyCollection<Type> entityTypes = supplement.EntityTypes;

            CollectionAssert.AreEqual(new Type[] { typeof(TextEntity) }, entityTypes.ToArray());
        }


        [TestMethod]
        public void LoadReturnsTextsOfAllFilesTest()
        {
            LocalizedTextSupplement<TextEntity> supplement = CreateSupplement();
            ClientDataTextFile english = new ClientDataTextFile(EnglishFile, "en", ["# English texts", NameText, DescText]);
            ClientDataTextFile german = new ClientDataTextFile(GermanFile, "de", ["Type=TextEntity,Name=PCS.CommunicationError.Name,Language=de,Text=PCS Kommunikationsfehler"]);
            List<ClientDataEntity> projectEntities = [CreateProjectEntity("{{PCS.CommunicationError.Name}}", 7), CreateProjectEntity("{{PCS.CommunicationError.Desc}}", 8)];

            IReadOnlyList<object> texts = supplement.Load(ProjectFile, [english, german], projectEntities, _changedDate);

            Assert.HasCount(3, texts);
            TextEntity description = (TextEntity)texts[1];
            Assert.AreEqual("PCS.CommunicationError.Desc", description.Name);
            Assert.AreEqual("en", description.Language);
            Assert.AreEqual("The PPC cannot communicate with the PCS, check the cable.", description.Text);
            Assert.AreEqual(ClientDataLoader.ChangedBy, description.ChangedBy);
            Assert.AreEqual(_changedDate, description.ChangedDate);
            Assert.AreEqual("de", ((TextEntity)texts[2]).Language);
        }


        [TestMethod]
        public void LoadKeyWithoutDefaultLanguageTextReportsProjectPositionTest()
        {
            LocalizedTextSupplement<TextEntity> supplement = CreateSupplement();
            ClientDataTextFile english = new ClientDataTextFile(EnglishFile, "en", [NameText]);
            ClientDataTextFile german = new ClientDataTextFile(GermanFile, "de", ["Type=TextEntity,Name=PCS.CommunicationError.Desc,Language=de,Text=Keine Verbindung zum PCS."]);
            List<ClientDataEntity> projectEntities = [CreateProjectEntity("{{PCS.CommunicationError.Name}}", 7), CreateProjectEntity("{{PCS.CommunicationError.Desc}}", 8)];

            ClientDataValidationException exception = Assert.ThrowsExactly<ClientDataValidationException>(
                () => supplement.Load(ProjectFile, [english, german], projectEntities, _changedDate));

            Assert.HasCount(1, exception.Errors);
            ClientDataError error = exception.Errors[0];
            Assert.AreEqual(ProjectFile, error.File);
            Assert.AreEqual(8, error.LineNumber);
            Assert.AreEqual("Con8", error.EntityName);
            Assert.AreEqual(nameof(ConnectionEntity.Description), error.PropertyName);
            StringAssert.Contains(error.Message, "'PCS.CommunicationError.Desc'");
            StringAssert.Contains(error.Message, EnglishFile);
        }


        [TestMethod]
        public void LoadWithoutTextFilesAndPlainTextsReturnsNoTextsTest()
        {
            LocalizedTextSupplement<TextEntity> supplement = CreateSupplement();
            List<ClientDataEntity> projectEntities = [CreateProjectEntity("Communication error of the PCS", 7), CreateProjectEntity(null, 8)];

            IReadOnlyList<object> texts = supplement.Load(ProjectFile, [], projectEntities, _changedDate);

            Assert.IsEmpty(texts);
        }


        [TestMethod]
        public void LoadLanguageNotMatchingFileNameReportsErrorTest()
        {
            LocalizedTextSupplement<TextEntity> supplement = CreateSupplement();
            ClientDataTextFile german = new ClientDataTextFile(GermanFile, "de", ["# German texts", NameText]);

            ClientDataValidationException exception = Assert.ThrowsExactly<ClientDataValidationException>(
                () => supplement.Load(ProjectFile, [german], [], _changedDate));

            Assert.HasCount(1, exception.Errors);
            ClientDataError error = exception.Errors[0];
            Assert.AreEqual(GermanFile, error.File);
            Assert.AreEqual(2, error.LineNumber);
            Assert.AreEqual("PCS.CommunicationError.Name", error.EntityName);
            Assert.AreEqual("Language", error.PropertyName);
        }


        [TestMethod]
        public void LoadOtherEntityTypeInTextFileReportsUnknownTypeTest()
        {
            LocalizedTextSupplement<TextEntity> supplement = CreateSupplement();
            ClientDataTextFile english = new ClientDataTextFile(EnglishFile, "en", [NameText, "Type=ConnectionEntity,Name=Con1,RetryCount=3"]);

            ClientDataValidationException exception = Assert.ThrowsExactly<ClientDataValidationException>(
                () => supplement.Load(ProjectFile, [english], [], _changedDate));

            Assert.HasCount(1, exception.Errors);
            Assert.AreEqual(EnglishFile, exception.Errors[0].File);
            Assert.AreEqual(2, exception.Errors[0].LineNumber);
            StringAssert.Contains(exception.Errors[0].Message, "Unknown entity type 'ConnectionEntity'");
        }


        [TestMethod]
        public void LoadDuplicatedKeyInTextFileReportsErrorTest()
        {
            LocalizedTextSupplement<TextEntity> supplement = CreateSupplement();
            ClientDataTextFile english = new ClientDataTextFile(EnglishFile, "en", [NameText, NameText]);

            ClientDataValidationException exception = Assert.ThrowsExactly<ClientDataValidationException>(
                () => supplement.Load(ProjectFile, [english], [], _changedDate));

            Assert.HasCount(1, exception.Errors);
            Assert.AreEqual(2, exception.Errors[0].LineNumber);
            StringAssert.Contains(exception.Errors[0].Message, "already defined");
        }


        [TestMethod]
        [DataRow("DemoBattery.csv", "DemoBattery.Texts.en.csv", "en")]
        [DataRow(@"C:\ClientData\Demo\DemoBattery.csv", @"C:\ClientData\Demo\DemoBattery.Texts.de-CH.csv", "de-CH")]
        public void GetLanguageReturnsLanguageOfFileNameTest(string clientProjectFile, string textFile, string expectedLanguage)
        {
            string language = LocalizedTextSupplement<TextEntity>.GetLanguage(clientProjectFile, textFile);

            Assert.AreEqual(expectedLanguage, language);
        }


        [TestMethod]
        public void GetLanguageOfOtherFileThrowsTest()
        {
            Action otherProject = () => LocalizedTextSupplement<TextEntity>.GetLanguage(ProjectFile, "DemoSolar.Texts.en.csv");

            Assert.ThrowsExactly<ArgumentException>(otherProject);
        }


        /// <summary>
        /// Creates the supplement with English as default language and the description of the connections as localizable value.
        /// </summary>
        /// <returns>The supplement.</returns>
        private static LocalizedTextSupplement<TextEntity> CreateSupplement()
        {
            return new LocalizedTextSupplement<TextEntity>(x => x.Name, x => x.Language, GetLocalizableValues, "en");
        }


        /// <summary>
        /// Gets the description of a connection as localizable value.
        /// </summary>
        /// <param name="entity">The entity of the client project file.</param>
        /// <returns>The description of a connection, nothing for other entities.</returns>
        private static IEnumerable<KeyValuePair<string, string?>> GetLocalizableValues(object entity)
        {
            if (entity is ConnectionEntity connection)
            {
                return [new KeyValuePair<string, string?>(nameof(ConnectionEntity.Description), connection.Description)];
            }

            return [];
        }


        /// <summary>
        /// Creates a connection of the client project file with the description at the line.
        /// </summary>
        /// <param name="description">The description, a localization key or a plain text.</param>
        /// <param name="lineNumber">The line number in the client project file.</param>
        /// <returns>The connection with its position.</returns>
        private static ClientDataEntity CreateProjectEntity(string? description, int lineNumber)
        {
            ConnectionEntity connection = new ConnectionEntity { Name = $"Con{lineNumber}", RetryCount = 3, ChangedBy = ClientDataLoader.ChangedBy, Description = description };

            return new ClientDataEntity(connection, ProjectFile, lineNumber, connection.Name);
        }
    }
}

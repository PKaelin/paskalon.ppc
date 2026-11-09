// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ClientData.Common.UnitTest
{
    /// <summary>
    /// Tests of the class <see cref="ClientDataLoader"/>.
    /// </summary>
    [TestClass]
    public class ClientDataLoaderTest
    {
        private const string FileName = "Demo.csv";
        private const string Connection = "Type=ConnectionEntity,Name=Con1,RetryCount=3";

        private static readonly DateTimeOffset _changedDate = new DateTimeOffset(2026, 11, 1, 10, 0, 0, TimeSpan.Zero);

       
        [TestMethod]
        public void LoadPopulatesScalarsAndAutomaticPropertiesTest()
        {
            ClientDataLoader loader = CreateLoader();
            string[] lines = ["Type=ConnectionEntity,Name=Con1,RetryCount=3,Description=\"a, b\",Day=Monday"];

            IReadOnlyList<object> entities = loader.Load(FileName, lines);

            ConnectionEntity entity = (ConnectionEntity)entities[0];
            Assert.AreEqual("Con1", entity.Name);
            Assert.AreEqual(3, entity.RetryCount);
            Assert.AreEqual("a, b", entity.Description);
            Assert.AreEqual(DayOfWeek.Monday, entity.Day);
            Assert.AreEqual(ClientDataLoader.ChangedBy, entity.ChangedBy);
            Assert.AreEqual(_changedDate, entity.ChangedDate);
        }


        [TestMethod]
        public void LoadResolvesReferenceAndKeepsNullReferenceTest()
        {
            ClientDataLoader loader = CreateLoader();
            string[] lines = [Connection, "Type=DeviceEntity,Name=Dev1,Connection=Con1,Backup=null"];

            IReadOnlyList<object> entities = loader.Load(FileName, lines);

            DeviceEntity device = (DeviceEntity)entities[1];
            Assert.AreSame(entities[0], device.Connection);
            Assert.IsNull(device.Backup);
        }


        [TestMethod]
        public void LoadAddsReferencesToListTest()
        {
            ClientDataLoader loader = CreateLoader();
            string[] lines = [Connection, "Type=OtherEntity,Name=Other1", "Type=OtherEntity,Name=Other 2", "Type=DeviceEntity,Name=Dev1,Connection=Con1,Others=Other1; Other 2"];

            IReadOnlyList<object> entities = loader.Load(FileName, lines);

            DeviceEntity device = (DeviceEntity)entities[3];
            Assert.HasCount(2, device.Others);
            Assert.AreSame(entities[1], device.Others.ElementAt(0));
            Assert.AreSame(entities[2], device.Others.ElementAt(1));
        }


        [TestMethod]
        public void LoadDistinguishesNullFromQuotedNullTest()
        {
            ClientDataLoader loader = CreateLoader();
            string[] lines = ["Type=ConnectionEntity,Name=A,RetryCount=1,Description=null", "Type=ConnectionEntity,Name=B,RetryCount=1,Description=\"null\""];

            IReadOnlyList<object> entities = loader.Load(FileName, lines);

            Assert.IsNull(((ConnectionEntity)entities[0]).Description);
            Assert.AreEqual("null", ((ConnectionEntity)entities[1]).Description);
        }


        [TestMethod]
        public void LoadIgnoresCommentsOnAnyLineAndBlankLinesTest()
        {
            ClientDataLoader loader = CreateLoader();
            string[] lines = ["# Comment", "", Connection, "#Comment", "Type=OtherEntity,Name=Other1", "# Comment"];

            IReadOnlyList<object> entities = loader.Load(FileName, lines);

            Assert.HasCount(2, entities);
        }


        [TestMethod]
        public void LoadRejectsHashThatIsNotFirstCharacterTest()
        {
            string[] lines = [Connection, " # Comment"];

            ClientDataError error = LoadSingleError(lines);

            Assert.AreEqual(2, error.LineNumber);
        }


        [TestMethod]
        public void LoadUsesNameOnlyAsIdentifierWhenDomainHasNoNamePropertyTest()
        {
            ClientDataLoader loader = CreateLoader();
            string[] lines = ["Type=SettingEntity,Name=Setting1,Type=Friday"];

            IReadOnlyList<object> entities = loader.Load(FileName, lines);

            Assert.AreEqual(DayOfWeek.Friday, ((SettingEntity)entities[0]).Type);
        }


        [TestMethod]
        public void LoadAllowsMissingNameWhenNotInheritingFromDomainBaseTest()
        {
            ClientDataLoader loader = CreateLoader();
            string[] lines = ["Type=PlainEntity,Value=5"];

            IReadOnlyList<object> entities = loader.Load(FileName, lines);

            Assert.AreEqual(5, ((PlainEntity)entities[0]).Value);
        }


        [TestMethod]
        public void LoadRequiresNameWhenInheritingFromDomainBaseTest()
        {
            string[] lines = ["Type=SettingEntity,Type=Friday"];

            ClientDataError error = LoadSingleError(lines);

            Assert.Contains("Name=", error.Message);
        }


        [TestMethod]
        [DataRow("Type=Unknown,Name=X", null, "Unknown entity type")]
        [DataRow("Name=X,Type=OtherEntity", null, "first column")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=1,Unknown=1", "Unknown", "Unknown property")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=1,ChangedBy=Me", "ChangedBy", "automatically")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=1,ChangedDate=2026-11-01T10:00:00Z", "ChangedDate", "automatically")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=1,Id=4", "Id", "automatically")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=abc", "RetryCount", "cannot be converted")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=-1", "RetryCount", "non-negative")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=null", "RetryCount", "must not be null")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=", "RetryCount", "empty")]
        [DataRow("Type=ConnectionEntity,Name=X", "RetryCount", "required")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=1,RetryCount=2", "RetryCount", "more than once")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=1,Name=Y", "Name", "more than once")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=1,Day=Someday", "Day", "not a member")]
        [DataRow("Type=ConnectionEntity,Name=X,RetryCount=1,Day=1", "Day", "not a member")]
        public void LoadRejectsInvalidLineTest(string line, string? propertyName, string expectedMessage)
        {
            string[] lines = ["# Comment", line];

            ClientDataError error = LoadSingleError(lines);

            Assert.AreEqual(FileName, error.File);
            Assert.AreEqual(2, error.LineNumber);
            Assert.AreEqual(propertyName, error.PropertyName);
            Assert.Contains(expectedMessage, error.Message);
        }


        [TestMethod]
        [DataRow("Type=DeviceEntity,Name=D,Connection=Later", "Connection", "not defined earlier")]
        [DataRow("Type=DeviceEntity,Name=D,Connection=Other1", "Connection", "expected")]
        [DataRow("Type=DeviceEntity,Name=D,Connection=null", "Connection", "must not be null")]
        [DataRow("Type=DeviceEntity,Name=D,Connection=Con1,ConnectionId=1", "ConnectionId", "foreign key")]
        [DataRow("Type=DeviceEntity,Name=D,Connection=Con1,Tags=Tag1", "Tags", "cannot be set")]
        [DataRow("Type=DeviceEntity,Name=D,Connection=Con1,Others=Other1;Later", "Others", "not defined earlier")]
        [DataRow("Type=DeviceEntity,Name=D,Connection=Con1,Others=Other1;Con1", "Others", "expected")]
        [DataRow("Type=DeviceEntity,Name=D,Connection=Con1,Others=Other1; Other1", "Others", "more than once")]
        [DataRow("Type=DeviceEntity,Name=D,Connection=Con1,Others=Other1;", "Others", "empty reference")]
        [DataRow("Type=DeviceEntity,Name=D,Connection=Con1,Others=null", "Others", "at least one")]
        public void LoadRejectsInvalidReferenceTest(string line, string propertyName, string expectedMessage)
        {
            string[] lines = [Connection, "Type=OtherEntity,Name=Other1", line];

            ClientDataError error = LoadSingleError(lines);

            Assert.AreEqual("D", error.EntityName);
            Assert.AreEqual(propertyName, error.PropertyName);
            Assert.Contains(expectedMessage, error.Message);
        }


        [TestMethod]
        public void LoadRejectsDuplicatedNameAcrossTypesTest()
        {
            string[] lines = [Connection, "Type=OtherEntity,Name=Con1"];

            ClientDataError error = LoadSingleError(lines);

            Assert.AreEqual(2, error.LineNumber);
            Assert.AreEqual("Con1", error.EntityName);
        }


        [TestMethod]
        public void LoadReportsAllErrorsWithoutFollowUpErrorsTest()
        {
            ClientDataLoader loader = CreateLoader();
            string[] lines = ["Type=ConnectionEntity,Name=Con1,RetryCount=abc", "Type=DeviceEntity,Name=Dev1,Connection=Con1,Unknown=1"];

            ClientDataValidationException exception = Assert.ThrowsExactly<ClientDataValidationException>(() => loader.Load(FileName, lines));

            Assert.HasCount(2, exception.Errors);
            Assert.AreEqual(1, exception.Errors[0].LineNumber);
            Assert.AreEqual(2, exception.Errors[1].LineNumber);
        }


        [TestMethod]
        public void LoadEntitiesReturnsEntitiesWithPositionsTest()
        {
            ClientDataLoader loader = CreateLoader();
            string[] lines = ["# Connections", Connection, "", "Type=PlainEntity,Value=5"];

            IReadOnlyList<ClientDataEntity> entities = loader.LoadEntities(FileName, lines);

            Assert.HasCount(2, entities);
            Assert.IsInstanceOfType<ConnectionEntity>(entities[0].Entity);
            Assert.AreEqual(FileName, entities[0].File);
            Assert.AreEqual(2, entities[0].LineNumber);
            Assert.AreEqual("Con1", entities[0].EntityName);
            Assert.IsInstanceOfType<PlainEntity>(entities[1].Entity);
            Assert.AreEqual(4, entities[1].LineNumber);
            Assert.IsNull(entities[1].EntityName);
        }


        [TestMethod]
        public void LoadNameRejectedByDomainReportsErrorTest()
        {
            string[] lines = ["Type=TextEntity,Name={{PCS.CommunicationError.Name}},Language=en,Text=PCS communication error"];

            ClientDataError error = LoadSingleError(lines);

            Assert.AreEqual(1, error.LineNumber);
            Assert.AreEqual("{{PCS.CommunicationError.Name}}", error.EntityName);
            Assert.AreEqual("Name", error.PropertyName);
            StringAssert.Contains(error.Message, "without braces");
        }


        /// <summary>
        /// Creates the loader with all test entity types.
        /// </summary>
        /// <returns>The loader.</returns>
        private ClientDataLoader CreateLoader()
        {
            Dictionary<string, Type> types = new Dictionary<string, Type>
            {
                [nameof(ConnectionEntity)] = typeof(ConnectionEntity),
                [nameof(OtherEntity)] = typeof(OtherEntity),
                [nameof(DeviceEntity)] = typeof(DeviceEntity),
                [nameof(SettingEntity)] = typeof(SettingEntity),
                [nameof(TextEntity)] = typeof(TextEntity),
                [nameof(PlainEntity)] = typeof(PlainEntity)
            };

            return new ClientDataLoader(types, _changedDate);
        }


        /// <summary>
        /// Loads lines that are expected to be invalid and returns the single error.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>The error.</returns>
        private ClientDataError LoadSingleError(string[] lines)
        {
            ClientDataLoader loader = CreateLoader();

            ClientDataValidationException exception = Assert.ThrowsExactly<ClientDataValidationException>(() => loader.Load(FileName, lines));

            Assert.HasCount(1, exception.Errors);

            return exception.Errors[0];
        }

    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.UnitTest.TestDoubles;

namespace paskalON.Domains.UnitTest
{
    [TestClass]
    public sealed class ConfigurationBaseTest
    {
        [TestMethod]
        public void ConfigurationBaseHoldsKeyValueDescriptionTest()
        {
            DateTimeOffset changedDate = new(2026, 4, 1, 10, 0, 0, TimeSpan.FromHours(2));

            TestConfiguration configuration = new()
            {
                Id = 42,
                ChangedBy = "admin",
                ChangedDate = changedDate,
                Key = "AuditRetentionSpan",
                Value = "90.00:00:00",
                Description = "Retention span of the audit table entries.",
            };

            Assert.AreEqual(42, configuration.Id);
            Assert.AreEqual("admin", configuration.ChangedBy);
            Assert.AreEqual(changedDate, configuration.ChangedDate);
            Assert.AreEqual(TimeSpan.Zero, configuration.ChangedDate.Offset);
            Assert.AreEqual("AuditRetentionSpan", configuration.Key);
            Assert.AreEqual("90.00:00:00", configuration.Value);
            Assert.AreEqual("Retention span of the audit table entries.", configuration.Description);
        }


        [TestMethod]
        public void ConfigurationBaseValueCanBeUpdatedTest()
        {
            TestConfiguration configuration = new()
            {
                ChangedBy = "admin",
                Key = "ArchiveUrl",
                Value = "https://example.com/v1",
                Description = "Archive URL of aggregated warranty data.",
            };

            configuration.Value = "https://example.com/v2";

            Assert.AreEqual("ArchiveUrl", configuration.Key);
            Assert.AreEqual("https://example.com/v2", configuration.Value);
        }


        [TestMethod]
        public void ConfigurationBaseIsDomainBaseTest()
        {
            TestConfiguration configuration = new()
            {
                ChangedBy = "admin",
                Key = "Key",
                Value = "Value",
                Description = "Description",
            };

            DomainBase domain = configuration;

            Assert.IsInstanceOfType<DomainBase>(domain);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using paskalON.Domains.UnitTest.TestDoubles;

namespace paskalON.Domains.UnitTest.Configurations
{
    [TestClass]
    public sealed class HistoryConfigurationTest
    {
        [TestMethod]
        public void ConfigureMigrationIdIsPrimaryKeyTest()
        {
            using TestDomainContext context = new();

            IKey? primaryKey = context.GetDesignTimeModel().FindEntityType(typeof(History))?.FindPrimaryKey();

            Assert.IsNotNull(primaryKey);
            Assert.HasCount(1, primaryKey.Properties);
            Assert.AreEqual(nameof(History.MigrationId), primaryKey.Properties[0].Name);
            Assert.AreEqual(typeof(string), primaryKey.Properties[0].ClrType);
        }


        [TestMethod]
        public void ConfigureMapsToMigrationHistoryTableTest()
        {
            using TestDomainContext context = new();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(typeof(History));

            Assert.IsNotNull(entityType);
            Assert.AreEqual("__EFMigrationsHistory", entityType.GetTableName());
            Assert.IsNull(entityType.BaseType);
        }


        [TestMethod]
        public void ConfigureProductVersionIsRequiredTest()
        {
            using TestDomainContext context = new();

            IProperty? property = context.GetDesignTimeModel().FindEntityType(typeof(History))?.FindProperty(nameof(History.ProductVersion));

            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsNullable);
        }
    }
}

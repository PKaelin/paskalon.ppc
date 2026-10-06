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
    public sealed class ConfigurationBaseConfigurationTest
    {
        [TestMethod]
        public void ConfigureUsesTablePerConcreteTypeTest()
        {
            using TestDomainContext context = new();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(typeof(ConfigurationBase));

            Assert.IsNotNull(entityType);
            Assert.AreEqual(RelationalAnnotationNames.TpcMappingStrategy, entityType.GetMappingStrategy());
            Assert.AreEqual(typeof(DomainBase), entityType.BaseType?.ClrType);
            Assert.IsNull(entityType.GetTableName());
        }


        [TestMethod]
        [DataRow(nameof(ConfigurationBase.Key), 250)]
        [DataRow(nameof(ConfigurationBase.Value), 500)]
        [DataRow(nameof(ConfigurationBase.Description), 800)]
        public void ConfigurePropertyIsRequiredWithMaxLengthTest(string propertyName, int expectedMaxLength)
        {
            using TestDomainContext context = new();

            IProperty? property = context.GetDesignTimeModel().FindEntityType(typeof(ConfigurationBase))?.FindProperty(propertyName);

            Assert.IsNotNull(property);
            Assert.AreEqual(expectedMaxLength, property.GetMaxLength());
            Assert.IsFalse(property.IsNullable);
        }


        [TestMethod]
        public void ConfigureKeyHasUniqueIndexTest()
        {
            using TestDomainContext context = new();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(typeof(TestConfiguration));

            Assert.IsNotNull(entityType);
            IProperty? keyProperty = entityType.FindProperty(nameof(ConfigurationBase.Key));
            Assert.IsNotNull(keyProperty);
            IIndex? index = entityType.FindIndex(keyProperty);
            Assert.IsNotNull(index);
            Assert.IsTrue(index.IsUnique);
            Assert.HasCount(1, index.Properties);
        }


        [TestMethod]
        public void ConfigureValueAndDescriptionHaveNoIndexTest()
        {
            using TestDomainContext context = new();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(typeof(TestConfiguration));

            Assert.IsNotNull(entityType);
            IProperty? valueProperty = entityType.FindProperty(nameof(ConfigurationBase.Value));
            IProperty? descriptionProperty = entityType.FindProperty(nameof(ConfigurationBase.Description));
            Assert.IsNotNull(valueProperty);
            Assert.IsNotNull(descriptionProperty);
            Assert.IsNull(entityType.FindIndex(valueProperty));
            Assert.IsNull(entityType.FindIndex(descriptionProperty));
        }


        [TestMethod]
        public void ConfigureConcreteTableContainsConfigurationColumnsTest()
        {
            using TestDomainContext context = new();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(typeof(TestConfiguration));

            Assert.IsNotNull(entityType);
            string? tableName = entityType.GetTableName();
            Assert.IsNotNull(tableName);
            StoreObjectIdentifier table = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());
            Assert.AreEqual("Key", entityType.FindProperty(nameof(ConfigurationBase.Key))?.GetColumnName(table));
            Assert.AreEqual("Value", entityType.FindProperty(nameof(ConfigurationBase.Value))?.GetColumnName(table));
            Assert.AreEqual("Description", entityType.FindProperty(nameof(ConfigurationBase.Description))?.GetColumnName(table));
        }
    }
}

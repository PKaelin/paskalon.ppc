// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using paskalON.Domains.UnitTest.TestDoubles;

namespace paskalON.Domains.UnitTest.Configurations
{
    [TestClass]
    public sealed class DomainBaseConfigurationTest
    {
        [TestMethod]
        public void ConfigureUsesTablePerConcreteTypeTest()
        {
            using TestDomainContext context = new TestDomainContext();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(typeof(DomainBase));

            Assert.IsNotNull(entityType);
            Assert.AreEqual(RelationalAnnotationNames.TpcMappingStrategy, entityType.GetMappingStrategy());
            Assert.IsNull(entityType.GetTableName());
        }


        [TestMethod]
        public void ConfigureIdIsPrimaryKeyTest()
        {
            using TestDomainContext context = new TestDomainContext();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(typeof(DomainBase));

            Assert.IsNotNull(entityType);
            IKey? primaryKey = entityType.FindPrimaryKey();
            Assert.IsNotNull(primaryKey);
            Assert.HasCount(1, primaryKey.Properties);
            Assert.AreEqual(nameof(DomainBase.Id), primaryKey.Properties[0].Name);
        }


        [TestMethod]
        [DataRow(typeof(TestDomain))]
        [DataRow(typeof(TestConfiguration))]
        [DataRow(typeof(TestNamedDomain))]
        public void ConfigureIdIsGeneratedFromSharedSequenceTest(Type concreteType)
        {
            using TestDomainContext context = new TestDomainContext();

            IModel model = context.GetDesignTimeModel();
            IProperty? idProperty = model.FindEntityType(concreteType)?.FindProperty(nameof(DomainBase.Id));

            Assert.IsNotNull(idProperty);
            Assert.AreEqual(ValueGenerated.OnAdd, idProperty.ValueGenerated);
            Assert.AreEqual(NpgsqlValueGenerationStrategy.Sequence, idProperty.GetValueGenerationStrategy());
            Assert.IsNotNull(model.FindSequence("DomainBaseSequence"));
        }


        [TestMethod]
        public void ConfigureChangedByIsRequiredWithMaxLengthTest()
        {
            using TestDomainContext context = new TestDomainContext();

            IProperty? property = context.GetDesignTimeModel().FindEntityType(typeof(DomainBase))?.FindProperty(nameof(DomainBase.ChangedBy));

            Assert.IsNotNull(property);
            Assert.AreEqual(250, property.GetMaxLength());
            Assert.IsFalse(property.IsNullable);
        }


        [TestMethod]
        public void ConfigureChangedDateIsRequiredTest()
        {
            using TestDomainContext context = new TestDomainContext();

            IProperty? property = context.GetDesignTimeModel().FindEntityType(typeof(DomainBase))?.FindProperty(nameof(DomainBase.ChangedDate));

            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsNullable);
            Assert.AreEqual(typeof(DateTimeOffset), property.ClrType);
        }


        [TestMethod]
        [DataRow(typeof(TestDomain))]
        [DataRow(typeof(TestConfiguration))]
        [DataRow(typeof(TestNamedDomain))]
        public void ConfigureConcreteTypeOwnsTableWithBaseColumnsTest(Type concreteType)
        {
            using TestDomainContext context = new TestDomainContext();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(concreteType);

            Assert.IsNotNull(entityType);
            string? tableName = entityType.GetTableName();
            Assert.IsNotNull(tableName);
            StoreObjectIdentifier table = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());
            Assert.AreEqual("Id", entityType.FindProperty(nameof(DomainBase.Id))?.GetColumnName(table));
            Assert.AreEqual("ChangedBy", entityType.FindProperty(nameof(DomainBase.ChangedBy))?.GetColumnName(table));
            Assert.AreEqual("ChangedDate", entityType.FindProperty(nameof(DomainBase.ChangedDate))?.GetColumnName(table));
        }


        [TestMethod]
        public void ConfigureConcreteTypesUseDistinctTablesTest()
        {
            using TestDomainContext context = new TestDomainContext();

            IModel model = context.GetDesignTimeModel();
            string?[] tableNames =
            [
                model.FindEntityType(typeof(TestDomain))?.GetTableName(),
                model.FindEntityType(typeof(TestConfiguration))?.GetTableName(),
                model.FindEntityType(typeof(TestNamedDomain))?.GetTableName(),
            ];

            Assert.IsTrue(tableNames.All(x => x is not null));
            Assert.HasCount(3, tableNames.Distinct());
        }
    }
}

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
    public sealed class NameBaseConfigurationTest
    {
        [TestMethod]
        public void ConfigureUsesTablePerConcreteTypeTest()
        {
            using TestDomainContext context = new();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(typeof(NameBase));

            Assert.IsNotNull(entityType);
            Assert.AreEqual(RelationalAnnotationNames.TpcMappingStrategy, entityType.GetMappingStrategy());
            Assert.AreEqual(typeof(DomainBase), entityType.BaseType?.ClrType);
            Assert.IsNull(entityType.GetTableName());
        }


        [TestMethod]
        public void ConfigureNameIsRequiredWithMaxLengthTest()
        {
            using TestDomainContext context = new();

            IProperty? property = context.GetDesignTimeModel().FindEntityType(typeof(NameBase))?.FindProperty(nameof(NameBase.Name));

            Assert.IsNotNull(property);
            Assert.AreEqual(250, property.GetMaxLength());
            Assert.IsFalse(property.IsNullable);
        }


        [TestMethod]
        public void ConfigureNameUsesCaseInsensitiveCollationTest()
        {
            using TestDomainContext context = new();

            IProperty? property = context.GetDesignTimeModel().FindEntityType(typeof(NameBase))?.FindProperty(nameof(NameBase.Name));

            Assert.IsNotNull(property);
            Assert.AreEqual("case_insensitive_collation", property.GetCollation());
        }


        [TestMethod]
        public void ConfigureNameHasUniqueIndexTest()
        {
            using TestDomainContext context = new();

            IEntityType? entityType = context.GetDesignTimeModel().FindEntityType(typeof(TestNamedDomain));

            Assert.IsNotNull(entityType);
            IProperty? nameProperty = entityType.FindProperty(nameof(NameBase.Name));
            Assert.IsNotNull(nameProperty);
            IIndex? index = entityType.FindIndex(nameProperty);
            Assert.IsNotNull(index);
            Assert.IsTrue(index.IsUnique);
            Assert.HasCount(1, index.Properties);
        }
    }
}

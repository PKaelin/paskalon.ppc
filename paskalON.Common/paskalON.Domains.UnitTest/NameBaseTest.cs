// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.UnitTest.TestDoubles;
using System.Reflection;

namespace paskalON.Domains.UnitTest
{
    [TestClass]
    public sealed class NameBaseTest
    {
        [TestMethod]
        public void NameBaseHoldsNameAndDomainValuesTest()
        {
            DateTimeOffset changedDate = new(2026, 5, 20, 18, 45, 0, TimeSpan.FromHours(-4));

            TestNamedDomain domain = new()
            {
                Id = 7,
                ChangedBy = "commissioning-tool",
                ChangedDate = changedDate,
                Name = "BESS Container 01",
            };

            Assert.AreEqual(7, domain.Id);
            Assert.AreEqual("commissioning-tool", domain.ChangedBy);
            Assert.AreEqual(changedDate, domain.ChangedDate);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
            Assert.AreEqual("BESS Container 01", domain.Name);
        }


        [TestMethod]
        public void NameBaseNameIsVirtualTest()
        {
            PropertyInfo? nameProperty = typeof(NameBase).GetProperty(nameof(NameBase.Name));
            MethodInfo? getter = nameProperty?.GetGetMethod();

            Assert.IsNotNull(getter);
            Assert.IsTrue(getter.IsVirtual);
            Assert.IsFalse(getter.IsFinal);
        }
    }
}

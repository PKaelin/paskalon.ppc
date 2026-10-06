// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.UnitTest.TestDoubles;

namespace paskalON.Domains.UnitTest
{
    [TestClass]
    public sealed class DomainBaseTest
    {
        [TestMethod]
        public void DomainBaseDefaultValuesTest()
        {
            DateTimeOffset before = DateTimeOffset.UtcNow;
            TestDomain domain = new() { ChangedBy = "operator@site-zurich" };
            DateTimeOffset after = DateTimeOffset.UtcNow;

            Assert.AreEqual(0, domain.Id);
            Assert.AreEqual("operator@site-zurich", domain.ChangedBy);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
            Assert.IsGreaterThanOrEqualTo(before, domain.ChangedDate);
            Assert.IsLessThanOrEqualTo(after, domain.ChangedDate);
        }


        [TestMethod]
        public void DomainBaseDefaultChangedDateIsPerInstanceTest()
        {
            TestDomain first = new() { ChangedBy = "system" };
            TestDomain second = new() { ChangedBy = "system" };

            Assert.IsLessThanOrEqualTo(second.ChangedDate, first.ChangedDate);
            Assert.AreEqual(TimeSpan.Zero, second.ChangedDate.Offset);
        }


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(2, 0)]
        [DataRow(-5, 0)]
        [DataRow(5, 45)]
        [DataRow(-3, -30)]
        [DataRow(14, 0)]
        [DataRow(-14, 0)]
        public void ChangedDateSetNormalizesToUtcTest(int offsetHours, int offsetMinutes)
        {
            TimeSpan offset = new(offsetHours, offsetMinutes, 0);
            DateTimeOffset localTime = new(2026, 3, 29, 1, 30, 15, 123, offset);
            TestDomain domain = new() { ChangedBy = "system" };
            domain.ChangedDate = localTime;

            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
            Assert.AreEqual(localTime.UtcDateTime, domain.ChangedDate.DateTime);
            Assert.AreEqual(localTime.UtcTicks, domain.ChangedDate.UtcTicks);
        }


        [TestMethod]
        public void ChangedDateSetAcrossDateBoundaryTest()
        {
            DateTimeOffset localTime = new(2026, 1, 1, 0, 30, 0, TimeSpan.FromHours(2));
            TestDomain domain = new() { ChangedBy = "system" };
            domain.ChangedDate = localTime;

            Assert.AreEqual(new DateTimeOffset(2025, 12, 31, 22, 30, 0, TimeSpan.Zero), domain.ChangedDate);
            Assert.AreEqual(2025, domain.ChangedDate.Year);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
        }


        [TestMethod]
        public void ChangedDateSetMinValueTest()
        {
            TestDomain domain = new() { ChangedBy = "system" };
            domain.ChangedDate = DateTimeOffset.MinValue;

            Assert.AreEqual(DateTimeOffset.MinValue, domain.ChangedDate);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
        }


        [TestMethod]
        public void ChangedDateSetMaxValueTest()
        {
            TestDomain domain = new() { ChangedBy = "system" };
            domain.ChangedDate = DateTimeOffset.MaxValue;

            Assert.AreEqual(DateTimeOffset.MaxValue, domain.ChangedDate);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
        }


        [TestMethod]
        public void ChangedDateSetLowestRepresentableInstantWithPositiveOffsetTest()
        {
            DateTimeOffset localTime = new(DateTime.MinValue.AddHours(14), TimeSpan.FromHours(14));
            TestDomain domain = new() { ChangedBy = "system" };
            domain.ChangedDate = localTime;

            Assert.AreEqual(DateTime.MinValue, domain.ChangedDate.DateTime);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
        }


        [TestMethod]
        public void ChangedDateSetHighestRepresentableInstantWithNegativeOffsetTest()
        {
            DateTimeOffset localTime = new(DateTime.MaxValue.AddHours(-14), TimeSpan.FromHours(-14));
            TestDomain domain = new() { ChangedBy = "system" };
            domain.ChangedDate = localTime;

            Assert.AreEqual(DateTime.MaxValue, domain.ChangedDate.DateTime);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
        }


        [TestMethod]
        public void ChangedDateSetKeepsSubMillisecondPrecisionTest()
        {
            DateTimeOffset localTime = new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.FromHours(1)).AddTicks(1234567);
            TestDomain domain = new() { ChangedBy = "system" };
            domain.ChangedDate = localTime;

            Assert.AreEqual(localTime.UtcTicks, domain.ChangedDate.Ticks);
        }


        [TestMethod]
        public void ChangedDateSetOverwritesPreviousValueTest()
        {
            TestDomain domain = new() { ChangedBy = "system" };
            domain.ChangedDate = new DateTimeOffset(2026, 1, 1, 8, 0, 0, TimeSpan.FromHours(1));
            domain.ChangedDate = new DateTimeOffset(2026, 7, 1, 8, 0, 0, TimeSpan.FromHours(2));

            Assert.AreEqual(new DateTimeOffset(2026, 7, 1, 6, 0, 0, TimeSpan.Zero), domain.ChangedDate);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
        }


        [TestMethod]
        public void ChangedDateObjectInitializerNormalizesToUtcTest()
        {
            DateTimeOffset localTime = new(2026, 10, 5, 9, 15, 0, TimeSpan.FromHours(-7));
            TestDomain domain = new() { ChangedBy = "system", ChangedDate = localTime };

            Assert.AreEqual(new DateTimeOffset(2026, 10, 5, 16, 15, 0, TimeSpan.Zero), domain.ChangedDate);
            Assert.AreEqual(TimeSpan.Zero, domain.ChangedDate.Offset);
        }
    }
}

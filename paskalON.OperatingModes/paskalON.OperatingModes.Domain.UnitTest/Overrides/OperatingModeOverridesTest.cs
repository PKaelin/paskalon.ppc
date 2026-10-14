// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Transitions;

namespace paskalON.OperatingModes.Domain.UnitTest.Overrides
{
    /// <summary>
    /// Tests the cardinality rules of <see cref="OperatingModeOverrides"/>.
    /// </summary>
    [TestClass]
    public class OperatingModeOverridesTest
    {
        /// <summary>
        /// Confirms overrides of different DER units can be active at the same time.
        /// </summary>
        [TestMethod]
        public void OperatingModeOverridesAddDifferentUnitsTest()
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();
            TestOverrideMode firstOverride = new TestOverrideMode("MaintenanceUnit1", "Bess1");
            TestOverrideMode secondOverride = new TestOverrideMode("MaintenanceUnit2", "Bess2");

            overrides.Add(firstOverride);
            overrides.Add(secondOverride);

            CollectionAssert.AreEqual(new[] { firstOverride, secondOverride }, overrides.OperatingModes.ToArray());
        }


        /// <summary>
        /// Confirms a second override of the same target is rejected.
        /// </summary>
        /// <param name="targetDerUnit">The overridden DER unit or null for the whole plant.</param>
        [TestMethod]
        [DataRow("Bess1")]
        [DataRow(null)]
        public void OperatingModeOverridesAddSameTargetTest(string? targetDerUnit)
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();
            TestOverrideMode firstOverride = new TestOverrideMode("FirstOverride", targetDerUnit);
            TestOverrideMode secondOverride = new TestOverrideMode("SecondOverride", targetDerUnit);
            overrides.Add(firstOverride);

            Assert.ThrowsExactly<InvalidOperationException>(() => overrides.Add(secondOverride));
            Assert.HasCount(1, overrides.OperatingModes);
        }


        /// <summary>
        /// Confirms the same override cannot be added twice.
        /// </summary>
        [TestMethod]
        public void OperatingModeOverridesAddSameModeTwiceTest()
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();
            TestOverrideMode maintenance = new TestOverrideMode("MaintenanceUnit1", "Bess1");
            overrides.Add(maintenance);

            Assert.ThrowsExactly<InvalidOperationException>(() => overrides.Add(maintenance));
        }


        /// <summary>
        /// Confirms the overridden DER units are reported.
        /// </summary>
        /// <param name="derUnitName">The DER unit to look up.</param>
        /// <param name="expected">Whether the DER unit is expected to be overridden.</param>
        [TestMethod]
        [DataRow("Bess1", true)]
        [DataRow("Bess2", false)]
        public void OperatingModeOverridesIsOverriddenTest(string derUnitName, bool expected)
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();
            overrides.Add(new TestOverrideMode("MaintenanceUnit1", "Bess1"));

            bool isOverridden = overrides.IsOverridden(derUnitName);

            Assert.AreEqual(expected, isOverridden);
        }


        /// <summary>
        /// Confirms a removed override no longer overrides its DER unit.
        /// </summary>
        [TestMethod]
        public void OperatingModeOverridesRemoveTest()
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();
            TestOverrideMode maintenance = new TestOverrideMode("MaintenanceUnit1", "Bess1");
            overrides.Add(maintenance);

            overrides.Remove(maintenance);

            Assert.IsEmpty(overrides.OperatingModes);
            Assert.IsFalse(overrides.IsOverridden("Bess1"));
        }


        /// <summary>
        /// Confirms removing an inactive override throws.
        /// </summary>
        [TestMethod]
        public void OperatingModeOverridesRemoveUnknownTest()
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();

            Assert.ThrowsExactly<KeyNotFoundException>(() => overrides.Remove(new TestOverrideMode("MaintenanceUnit1", "Bess1")));
        }


        /// <summary>
        /// Confirms null arguments are rejected.
        /// </summary>
        [TestMethod]
        public void OperatingModeOverridesNullAddArgumentsTest()
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();

            Assert.ThrowsExactly<ArgumentNullException>(() => overrides.Add(null!));
        }


        /// <summary>
        /// Confirms null arguments are rejected.
        /// </summary>
        [TestMethod]
        public void OperatingModeOverridesNullRemoveArgumentsTest()
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();

            Assert.ThrowsExactly<ArgumentNullException>(() => overrides.Remove(null!));
        }


        /// <summary>
        /// Confirms null arguments are rejected.
        /// </summary>
        [TestMethod]
        public void OperatingModeOverridesNullIsOverriddenArgumentsTest()
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();

            Assert.ThrowsExactly<ArgumentNullException>(() => overrides.IsOverridden(null!));
        }


        /// <summary>
        /// Confirms taking a cycle returns the committed changes once without activating the override modes.
        /// </summary>
        [TestMethod]
        public void OperatingModeOverridesTakeCycleReturnsChangesOnceTest()
        {
            OperatingModeOverrides overrides = new OperatingModeOverrides();
            TestOverrideMode maintenance = new TestOverrideMode("MaintenanceUnit1", "Bess1");
            overrides.Add(maintenance);
            overrides.Remove(maintenance);

            OperatingModeCycle<IOverrideMode> firstCycle = overrides.TakeCycle();
            OperatingModeCycle<IOverrideMode> secondCycle = overrides.TakeCycle();

            Assert.HasCount(2, firstCycle.Changes);
            Assert.AreEqual(OperatingModeChangeKind.Added, firstCycle.Changes[0].Kind);
            Assert.AreEqual(OperatingModeChangeKind.Removed, firstCycle.Changes[1].Kind);
            Assert.IsEmpty(secondCycle.Changes);
            Assert.IsFalse(maintenance.IsActivated);
        }
    }
}

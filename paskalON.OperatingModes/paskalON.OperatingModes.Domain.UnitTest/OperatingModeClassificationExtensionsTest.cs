// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.OperatingModes.Domain.Abstractions;

namespace paskalON.OperatingModes.Domain.UnitTest
{
    /// <summary>
    /// Tests the classification resolution of <see cref="OperatingModeClassificationExtensions"/>.
    /// </summary>
    [TestClass]
    public class OperatingModeClassificationExtensionsTest
    {
        /// <summary>
        /// Confirms the classification is resolved from the implemented classification interface.
        /// </summary>
        /// <param name="interfaceType">The implemented classification interface.</param>
        /// <param name="expected">The expected classification.</param>
        [TestMethod]
        [DataRow(typeof(IOverrideMode), OperatingModeClassification.Override)]
        [DataRow(typeof(IExclusiveActiveMode), OperatingModeClassification.ExclusiveActive)]
        [DataRow(typeof(IExclusiveReactiveMode), OperatingModeClassification.ExclusiveReactive)]
        [DataRow(typeof(IBaseActiveMode), OperatingModeClassification.BaseActive)]
        [DataRow(typeof(IBaseReactiveMode), OperatingModeClassification.BaseReactive)]
        [DataRow(typeof(IAdditiveActiveMode), OperatingModeClassification.AdditiveActive)]
        [DataRow(typeof(IAdditiveReactiveMode), OperatingModeClassification.AdditiveReactive)]
        [DataRow(typeof(ILimitingActiveMode), OperatingModeClassification.LimitingActive)]
        [DataRow(typeof(ILimitingReactiveMode), OperatingModeClassification.LimitingReactive)]
        [DataRow(typeof(IStackableMode), OperatingModeClassification.None)]
        public void OperatingModeClassificationExtensionsResolveTest(Type interfaceType, OperatingModeClassification expected)
        {
            Mock mock = (Mock)Activator.CreateInstance(typeof(Mock<>).MakeGenericType(interfaceType))!;
            IOperatingMode operatingMode = (IOperatingMode)mock.Object;

            OperatingModeClassification classification = OperatingModeClassificationExtensions.Resolve(operatingMode);

            Assert.AreEqual(expected, classification);
        }


        /// <summary>
        /// Confirms the power axes of the classifications.
        /// </summary>
        /// <param name="classification">The classification.</param>
        /// <param name="expectedActive">Whether active power is expected.</param>
        /// <param name="expectedReactive">Whether reactive power is expected.</param>
        [TestMethod]
        [DataRow(OperatingModeClassification.None, false, false)]
        [DataRow(OperatingModeClassification.Override, true, true)]
        [DataRow(OperatingModeClassification.ExclusiveActive, true, false)]
        [DataRow(OperatingModeClassification.BaseReactive, false, true)]
        [DataRow(OperatingModeClassification.AdditiveActive, true, false)]
        [DataRow(OperatingModeClassification.LimitingReactive, false, true)]
        [DataRow(OperatingModeClassification.BaseActive | OperatingModeClassification.LimitingReactive, true, true)]
        public void OperatingModeClassificationExtensionsHasAxisTest(OperatingModeClassification classification, bool expectedActive, bool expectedReactive)
        {
            bool hasActive = classification.HasActive();
            bool hasReactive = classification.HasReactive();

            Assert.AreEqual(expectedActive, hasActive);
            Assert.AreEqual(expectedReactive, hasReactive);
        }


        /// <summary>
        /// Confirms the classification is resolved from an operating mode that implements two classification interfaces.
        /// </summary>
        [TestMethod]
        public void OperatingModeClassificationExtensionsTwoClassificationsTest()
        {
            TestBaseActiveReactiveMode mode = new TestBaseActiveReactiveMode("TestMode");
            OperatingModeClassification classification = OperatingModeClassificationExtensions.Resolve(mode);
            OperatingModeClassification expectedClassification = OperatingModeClassification.BaseActive | OperatingModeClassification.BaseReactive;

            Assert.AreEqual(expectedClassification, classification);
        }


        /// <summary>
        /// Confirms a null operating mode throws.
        /// </summary>
        [TestMethod]
        public void OperatingModeClassificationExtensionsResolveNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => OperatingModeClassificationExtensions.Resolve(null!));
        }
    }
}

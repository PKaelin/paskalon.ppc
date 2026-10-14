// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Stacks;

namespace paskalON.OperatingModes.Domain.UnitTest.Stacks
{
    /// <summary>
    /// Tests the classification rules of <see cref="OperatingModeStackValidator"/>.
    /// </summary>
    [TestClass]
    public class OperatingModeStackValidatorTest
    {
        /// <summary>
        /// Confirms valid stacks are accepted.
        /// </summary>
        /// <param name="description">Description of the stack.</param>
        /// <param name="classifications">The classification interfaces in ascending priority.</param>
        [TestMethod]
        [DataRow("Empty", new Type[] { })]
        [DataRow("BaseOnly", new Type[] { typeof(IBaseActiveMode) })]
        [DataRow("BaseAdditiveLimiting", new Type[] { typeof(IBaseActiveMode), typeof(IAdditiveActiveMode), typeof(IAdditiveActiveMode), typeof(ILimitingActiveMode), typeof(ILimitingActiveMode) })]
        [DataRow("MixedAxes", new Type[] { typeof(IBaseReactiveMode), typeof(IBaseActiveMode), typeof(IAdditiveActiveMode), typeof(ILimitingReactiveMode) })]
        [DataRow("ExclusiveBoth", new Type[] { typeof(IExclusiveActiveMode), typeof(IExclusiveReactiveMode) })]
        [DataRow("ExclusiveActiveBaseReactive", new Type[] { typeof(IExclusiveActiveMode), typeof(IBaseReactiveMode), typeof(ILimitingReactiveMode) })]
        public void OperatingModeStackValidatorValidStackTest(string description, Type[] classifications)
        {
            IReadOnlyList<IStackableMode> modes = CreateModes(classifications);
            Exception? exception = null;

            try
            {
                OperatingModeStackValidator.Validate(modes);
            }
            catch (InvalidOperationException ex)
            {
                exception = ex;
            }

            Assert.IsNull(exception, description);
        }


        /// <summary>
        /// Confirms invalid stacks are rejected.
        /// </summary>
        /// <param name="description">Description of the violated rule.</param>
        /// <param name="classifications">The classification interfaces in ascending priority.</param>
        [TestMethod]
        [DataRow("TwoBaseActive", new Type[] { typeof(IBaseActiveMode), typeof(IBaseActiveMode) })]
        [DataRow("TwoBaseReactive", new Type[] { typeof(IBaseReactiveMode), typeof(IBaseReactiveMode) })]
        [DataRow("AdditiveWithoutBase", new Type[] { typeof(IAdditiveActiveMode) })]
        [DataRow("LimitingWithoutBase", new Type[] { typeof(ILimitingReactiveMode) })]
        [DataRow("AdditiveBeforeBase", new Type[] { typeof(IAdditiveActiveMode), typeof(IBaseActiveMode) })]
        [DataRow("AdditiveAfterLimiting", new Type[] { typeof(IBaseActiveMode), typeof(ILimitingActiveMode), typeof(IAdditiveActiveMode) })]
        [DataRow("ReactiveAdditiveAfterLimiting", new Type[] { typeof(IBaseReactiveMode), typeof(ILimitingReactiveMode), typeof(IAdditiveReactiveMode) })]
        [DataRow("TwoExclusiveActive", new Type[] { typeof(IExclusiveActiveMode), typeof(IExclusiveActiveMode) })]
        [DataRow("ExclusiveWithBase", new Type[] { typeof(IExclusiveActiveMode), typeof(IBaseActiveMode) })]
        [DataRow("ExclusiveWithLimiting", new Type[] { typeof(IExclusiveReactiveMode), typeof(ILimitingReactiveMode) })]
        public void OperatingModeStackValidatorInvalidStackTest(string description, Type[] classifications)
        {
            IReadOnlyList<IStackableMode> modes = CreateModes(classifications);

            Assert.ThrowsExactly<InvalidOperationException>(() => OperatingModeStackValidator.Validate(modes), description);
        }


        /// <summary>
        /// Confirms duplicate names are rejected.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackValidatorDuplicateNamesTest()
        {
            IReadOnlyList<IStackableMode> modes = new IStackableMode[] { CreateMode(typeof(IBaseActiveMode), "Mode"), CreateMode(typeof(IBaseReactiveMode), "Mode") };

            Assert.ThrowsExactly<InvalidOperationException>(() => OperatingModeStackValidator.Validate(modes));
        }


        /// <summary>
        /// Confirms a null list is rejected.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackValidatorNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => OperatingModeStackValidator.Validate(null!));
        }


        /// <summary>
        /// Confirms one mode cannot have multiple active-power classifications.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackValidatorMultipleActiveClassificationsTest()
        {
            Mock<IBaseActiveMode> mock = new Mock<IBaseActiveMode>();
            mock.As<IAdditiveActiveMode>();
            mock.As<IOperatingMode>().Setup(mode => mode.Name).Returns("Mode");

            IReadOnlyList<IStackableMode> modes = new IStackableMode[] { mock.Object };

            Assert.ThrowsExactly<InvalidOperationException>(() => OperatingModeStackValidator.Validate(modes));
        }


        /// <summary>
        /// Confirms one mode cannot have multiple reactive-power classifications.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackValidatorMultipleReactiveClassificationsTest()
        {
            Mock<IBaseReactiveMode> mock = new Mock<IBaseReactiveMode>();
            mock.As<IAdditiveReactiveMode>();
            mock.As<IOperatingMode>().Setup(mode => mode.Name).Returns("Mode");

            IReadOnlyList<IStackableMode> modes = new IStackableMode[] { mock.Object };

            Assert.ThrowsExactly<InvalidOperationException>(() => OperatingModeStackValidator.Validate(modes));
        }


        /// <summary>
        /// Creates mocked operating modes with unique names.
        /// </summary>
        /// <param name="classifications">The classification interfaces.</param>
        /// <returns>The mocked operating modes.</returns>
        private IReadOnlyList<IStackableMode> CreateModes(Type[] classifications)
        {
            return classifications.Select((classification, index) => CreateMode(classification, $"Mode{index}")).ToList();
        }


        /// <summary>
        /// Creates a mocked operating mode implementing the classification interface.
        /// </summary>
        /// <param name="classification">The classification interface.</param>
        /// <param name="name">The name of the operating mode.</param>
        /// <returns>The mocked operating mode.</returns>
        private IStackableMode CreateMode(Type classification, string name)
        {
            Mock mock = (Mock)Activator.CreateInstance(typeof(Mock<>).MakeGenericType(classification))!;
            mock.As<IOperatingMode>().Setup(mode => mode.Name).Returns(name);

            return (IStackableMode)mock.Object;
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Application.Stacks;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Dto.Stacks;
using System.Text.Json;

namespace paskalON.OperatingModes.Application.UnitTest.Stacks
{
    /// <summary>
    /// Tests <see cref="OperatingModeStackDtoFactory"/>.
    /// </summary>
    [TestClass]
    public sealed class OperatingModeStackDtoFactoryTest
    {
        /// <summary>
        /// The factory under test.
        /// </summary>
        private readonly OperatingModeStackDtoFactory _factory = new OperatingModeStackDtoFactory();


        /// <summary>
        /// Confirms operating modes without a specialized DTO get the default stack DTO with their stack type.
        /// </summary>
        /// <param name="modeKind">The kind of operating mode.</param>
        /// <param name="expectedName">The expected operating mode name.</param>
        /// <param name="expectedType">The expected stack type.</param>
        [TestMethod]
        [DataRow("ActivePowerFixed", "ActivePowerFixedMode", OperatingModeStackTypeDto.ExclusiveActive)]
        [DataRow("ReactivePowerFixed", "ReactivePowerFixedMode", OperatingModeStackTypeDto.ExclusiveReactive)]
        [DataRow("MaximumPowerPointTracking", "MaximumPowerPointTrackingMode", OperatingModeStackTypeDto.BaseActive)]
        public void OperatingModeStackDtoFactoryCreateDefaultDtoTest(string modeKind, string expectedName, OperatingModeStackTypeDto expectedType)
        {
            IStackableMode operatingMode = CreateMode(modeKind);

            OperatingModeStackDto dto = _factory.Create(operatingMode);

            Assert.AreEqual(typeof(OperatingModeStackDto), dto.GetType());
            Assert.AreEqual(expectedName, dto.OperatingModeName);
            Assert.AreEqual(expectedType, dto.Type);
        }


        /// <summary>
        /// Confirms the active power limit mode gets the limiting stack DTO with its configured limits.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackDtoFactoryCreateLimitingDtoTest()
        {
            IStackableMode operatingMode = ApplicationTestData.CreateActivePowerLimitMode();

            OperatingModeStackDto dto = _factory.Create(operatingMode);

            OperatingModeLimitingStackDto limitingDto = Assert.IsInstanceOfType<OperatingModeLimitingStackDto>(dto);
            Assert.AreEqual("ActivePowerLimitMode", limitingDto.OperatingModeName);
            Assert.AreEqual(OperatingModeStackTypeDto.LimitingActive, limitingDto.Type);
            Assert.AreEqual(6000000, limitingDto.MaximumLimit);
            Assert.AreEqual(-4000000, limitingDto.MinimumLimit);
        }


        /// <summary>
        /// Confirms the reactive power limit mode gets the limiting stack DTO with its configured limits.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackDtoFactoryCreateReactiveLimitingDtoTest()
        {
            IStackableMode operatingMode = ApplicationTestData.CreateReactivePowerLimitMode();

            OperatingModeStackDto dto = _factory.Create(operatingMode, 3);

            OperatingModeLimitingStackDto limitingDto = Assert.IsInstanceOfType<OperatingModeLimitingStackDto>(dto);
            Assert.AreEqual("ReactivePowerLimitMode", limitingDto.OperatingModeName);
            Assert.AreEqual(OperatingModeStackTypeDto.LimitingReactive, limitingDto.Type);
            Assert.AreEqual(3, limitingDto.Priority);
            Assert.AreEqual(3000000, limitingDto.MaximumLimit);
            Assert.AreEqual(-2000000, limitingDto.MinimumLimit);
        }


        /// <summary>
        /// Confirms the DTOs are created in the order of the operating modes.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackDtoFactoryCreateListKeepsOrderTest()
        {
            IStackableMode[] operatingModes = new IStackableMode[]
            {
                ApplicationTestData.CreateMaximumPowerPointTrackingMode(),
                ApplicationTestData.CreateActivePowerLimitMode()
            };

            IReadOnlyList<OperatingModeStackDto> dtos = _factory.Create(operatingModes);

            Assert.HasCount(2, dtos);
            Assert.AreEqual("MaximumPowerPointTrackingMode", dtos[0].OperatingModeName);
            Assert.IsInstanceOfType<OperatingModeLimitingStackDto>(dtos[1]);
            // Available operating modes are not in the stack
            Assert.IsNull(dtos[0].Priority);
            Assert.IsNull(dtos[1].Priority);
        }


        /// <summary>
        /// Confirms the DTOs of the active operating modes have the priorities of the stack entries.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackDtoFactoryCreateEntriesSetsPriorityTest()
        {
            OperatingModeStackEntry[] entries = new OperatingModeStackEntry[]
            {
                new OperatingModeStackEntry(1, ApplicationTestData.CreateMaximumPowerPointTrackingMode()),
                new OperatingModeStackEntry(2, ApplicationTestData.CreateActivePowerLimitMode())
            };

            IReadOnlyList<OperatingModeStackDto> dtos = _factory.Create(entries);

            Assert.HasCount(2, dtos);
            Assert.AreEqual("MaximumPowerPointTrackingMode", dtos[0].OperatingModeName);
            Assert.AreEqual(1, dtos[0].Priority);
            OperatingModeLimitingStackDto limitingDto = Assert.IsInstanceOfType<OperatingModeLimitingStackDto>(dtos[1]);
            Assert.AreEqual(2, limitingDto.Priority);
            Assert.AreEqual(6000000, limitingDto.MaximumLimit);
        }


        /// <summary>
        /// Confirms a list of the base DTO is serialized with the type discriminator and the properties of the derived DTOs.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackDtoFactoryCreateListSerializesDerivedDtosTest()
        {
            IStackableMode[] operatingModes = new IStackableMode[]
            {
                ApplicationTestData.CreateMaximumPowerPointTrackingMode(),
                ApplicationTestData.CreateActivePowerLimitMode()
            };
            IReadOnlyList<OperatingModeStackDto> dtos = _factory.Create(operatingModes);

            string json = JsonSerializer.Serialize(dtos);

            List<OperatingModeStackDto>? deserialized = JsonSerializer.Deserialize<List<OperatingModeStackDto>>(json);
            StringAssert.Contains(json, "\"$type\":\"limiting\"");
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(typeof(OperatingModeStackDto), deserialized[0].GetType());
            OperatingModeLimitingStackDto limitingDto = Assert.IsInstanceOfType<OperatingModeLimitingStackDto>(deserialized[1]);
            Assert.AreEqual(6000000, limitingDto.MaximumLimit);
            Assert.AreEqual(-4000000, limitingDto.MinimumLimit);
        }


        /// <summary>
        /// Confirms null arguments are rejected.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackDtoFactoryCreateNullArgumentsThrowTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _factory.Create((IStackableMode)null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => _factory.Create((IEnumerable<IStackableMode>)null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => _factory.Create((IEnumerable<OperatingModeStackEntry>)null!));
        }


        /// <summary>
        /// Creates the operating mode of the specified kind.
        /// </summary>
        /// <param name="modeKind">The kind of operating mode.</param>
        /// <returns>The stackable operating mode.</returns>
        private static IStackableMode CreateMode(string modeKind)
        {
            return modeKind switch
            {
                "ActivePowerFixed" => ApplicationTestData.CreateActivePowerFixedMode(),
                "ReactivePowerFixed" => ApplicationTestData.CreateReactivePowerFixedMode(),
                _ => ApplicationTestData.CreateMaximumPowerPointTrackingMode()
            };
        }
    }
}

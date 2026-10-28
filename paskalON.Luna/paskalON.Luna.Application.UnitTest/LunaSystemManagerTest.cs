// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Devices.Client;
using paskalON.OperatingModes.Client;
using paskalON.OperatingModes.Dto.Overrides;
using paskalON.OperatingModes.Service.Dto.V1.Requests;
using System.Net;

namespace paskalON.Luna.Application.UnitTest
{
    /// <summary>
    /// Tests <see cref="LunaSystemManager"/> with mocked operating mode and device servers.
    /// </summary>
    [TestClass]
    public sealed class LunaSystemManagerTest
    {
        /// <summary>
        /// Name of the shutdown override operating mode.
        /// </summary>
        private const string ShutdownOperatingModeName = "ShutdownMode";


        /// <summary>
        /// The mocked operating mode server.
        /// </summary>
        private Mock<IOperatingModeServer> _operatingModeServer = null!;


        /// <summary>
        /// The mocked device server.
        /// </summary>
        private Mock<IDeviceServer> _deviceServer = null!;


        /// <summary>
        /// The system manager under test.
        /// </summary>
        private LunaSystemManager _manager = null!;


        /// <summary>
        /// Creates the system manager with mocked servers.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _operatingModeServer = new Mock<IOperatingModeServer>();
            _deviceServer = new Mock<IDeviceServer>();
            _manager = new LunaSystemManager(NullLogger<LunaSystemManager>.Instance, _operatingModeServer.Object, _deviceServer.Object, ShutdownOperatingModeName);
        }


        /// <summary>
        /// Confirms the constructor validates the shutdown operating mode name.
        /// </summary>
        /// <param name="shutdownOperatingModeName">The invalid shutdown operating mode name.</param>
        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void LunaSystemManagerConstructorInvalidShutdownNameThrowsTest(string shutdownOperatingModeName)
        {
            Assert.ThrowsExactly<ArgumentException>(
                () => new LunaSystemManager(NullLogger<LunaSystemManager>.Instance, _operatingModeServer.Object, _deviceServer.Object, shutdownOperatingModeName));
        }


        /// <summary>
        /// Confirms the constructor validates the dependencies.
        /// </summary>
        [TestMethod]
        public void LunaSystemManagerConstructorNullArgumentsThrowTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new LunaSystemManager(null!, _operatingModeServer.Object, _deviceServer.Object, ShutdownOperatingModeName));
            Assert.ThrowsExactly<ArgumentNullException>(() => new LunaSystemManager(NullLogger<LunaSystemManager>.Instance, null!, _deviceServer.Object, ShutdownOperatingModeName));
            Assert.ThrowsExactly<ArgumentNullException>(() => new LunaSystemManager(NullLogger<LunaSystemManager>.Instance, _operatingModeServer.Object, null!, ShutdownOperatingModeName));
            Assert.ThrowsExactly<ArgumentNullException>(() => new LunaSystemManager(NullLogger<LunaSystemManager>.Instance, _operatingModeServer.Object, _deviceServer.Object, null!));
        }


        /// <summary>
        /// Confirms the PCS are only started when no shutdown is active. Other active overrides do not prevent the start.
        /// </summary>
        /// <param name="isShutdownActive">True if the shutdown override operating mode is active.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="expectedStartCalls">The expected number of start all PCS calls.</param>
        [TestMethod]
        [DataRow(true, SystemCommandResult.ShutdownActive, 0)]
        [DataRow(false, SystemCommandResult.Executed, 1)]
        public async Task StartAsyncStartsAllPcsOnlyWithoutShutdownTest(bool isShutdownActive, SystemCommandResult expectedResult, int expectedStartCalls)
        {
            SetupActiveOverrides(isShutdownActive);

            SystemCommandResult result = await _manager.StartAsync();

            Assert.AreEqual(expectedResult, result);
            _deviceServer.Verify(server => server.ConnectAllMetersAsync(It.IsAny<CancellationToken>()), Times.Exactly(expectedStartCalls));
            _deviceServer.Verify(server => server.StartAllPcs(It.IsAny<CancellationToken>()), Times.Exactly(expectedStartCalls));
        }


        /// <summary>
        /// Confirms the shutdown is only activated when it is not already active.
        /// </summary>
        /// <param name="isShutdownActive">True if the shutdown override operating mode is active.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="expectedActivateCalls">The expected number of activate calls of the shutdown.</param>
        [TestMethod]
        [DataRow(true, SystemCommandResult.ShutdownActive, 0)]
        [DataRow(false, SystemCommandResult.Executed, 1)]
        public async Task StopAsyncActivatesShutdownOnlyWhenNotActiveTest(bool isShutdownActive, SystemCommandResult expectedResult, int expectedActivateCalls)
        {
            SetupActiveOverrides(isShutdownActive);

            SystemCommandResult result = await _manager.StopAsync();

            Assert.AreEqual(expectedResult, result);
            _operatingModeServer.Verify(
                server => server.ActivateOverrideOperatingMode(It.Is<ActivateOverrideRequest>(request => request.OperatingModeName == ShutdownOperatingModeName), It.IsAny<CancellationToken>()),
                Times.Exactly(expectedActivateCalls));
            _deviceServer.Verify(server => server.StopAllPcs(It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// Confirms the shutdown is only deactivated when it is active.
        /// </summary>
        /// <param name="isShutdownActive">True if the shutdown override operating mode is active.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="expectedDeactivateCalls">The expected number of deactivate calls of the shutdown.</param>
        [TestMethod]
        [DataRow(true, SystemCommandResult.Executed, 1)]
        [DataRow(false, SystemCommandResult.ShutdownNotActive, 0)]
        public async Task ClearShutdownAsyncDeactivatesShutdownOnlyWhenActiveTest(bool isShutdownActive, SystemCommandResult expectedResult, int expectedDeactivateCalls)
        {
            SetupActiveOverrides(isShutdownActive);

            SystemCommandResult result = await _manager.ClearShutdownAsync();

            Assert.AreEqual(expectedResult, result);
            _operatingModeServer.Verify(
                server => server.DeactivateOverrideOperatingMode(It.Is<DeactivateOverrideRequest>(request => request.OperatingModeName == ShutdownOperatingModeName), It.IsAny<CancellationToken>()),
                Times.Exactly(expectedDeactivateCalls));
        }


        /// <summary>
        /// Confirms a request rejected by the operating mode service is passed on to the caller with its status code.
        /// </summary>
        [TestMethod]
        public async Task StopAsyncRejectedRequestThrowsWithStatusCodeTest()
        {
            SetupActiveOverrides(false);
            _operatingModeServer
                .Setup(server => server.ActivateOverrideOperatingMode(It.IsAny<ActivateOverrideRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Rejected", null, HttpStatusCode.InternalServerError));

            HttpRequestException exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(() => _manager.StopAsync());

            Assert.AreEqual(HttpStatusCode.InternalServerError, exception.StatusCode);
        }


        /// <summary>
        /// Sets up the active override operating modes. A maintenance mode of a DER unit is always active.
        /// </summary>
        /// <param name="isShutdownActive">True if the shutdown override operating mode is active as well.</param>
        private void SetupActiveOverrides(bool isShutdownActive)
        {
            List<OperatingModeOverrideDto> overrides = new List<OperatingModeOverrideDto>
            {
                new OperatingModeOverrideDto { OperatingModeName = "MaintenanceModeUnit1", TargetDerUnit = "Unit1" }
            };

            if (isShutdownActive)
            {
                overrides.Add(new OperatingModeOverrideDto { OperatingModeName = ShutdownOperatingModeName, TargetDerUnit = null });
            }

            _operatingModeServer
                .Setup(server => server.GetActiveOverrideOperatingModes(It.IsAny<CancellationToken>()))
                .ReturnsAsync(overrides);
        }
    }
}

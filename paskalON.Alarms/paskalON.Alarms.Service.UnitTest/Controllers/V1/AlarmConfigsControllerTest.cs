// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Alarms.Application.Caches;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Alarms.Service.Controllers.V1;
using paskalON.Alarms.Service.Dto.V1.Responses;

namespace paskalON.Alarms.Service.UnitTest.Controllers.V1
{
    /// <summary>
    /// Tests <see cref="AlarmConfigsController"/> with the real cache and a mocked configuration repository.
    /// </summary>
    [TestClass]
    public class AlarmConfigsControllerTest
    {
        /// <summary>
        /// Time when the configurations were changed.
        /// </summary>
        private static readonly DateTimeOffset ChangedOn = new DateTimeOffset(2026, 11, 6, 8, 0, 0, TimeSpan.Zero);


        /// <summary>
        /// Mocked configuration repository.
        /// </summary>
        private readonly Mock<IPpcAlarmConfigRepository> _repository = new Mock<IPpcAlarmConfigRepository>();


        /// <summary>
        /// Cache of the configurations.
        /// </summary>
        private readonly PpcAlarmConfigCache _cache = new PpcAlarmConfigCache(NullLogger<PpcAlarmConfigCache>.Instance);


        /// <summary>
        /// The reload loads the enabled configurations into the cache and returns their number.
        /// </summary>
        [TestMethod]
        public async Task ReloadReturnsConfigCountTest()
        {
            SetupConfigs(CreateConfig(1, "PCS-FAULT", PpcAlarmType.PcsAlarm), CreateConfig(2, "SYS-RTU", PpcAlarmType.SystemAlarm));
            AlarmConfigsController controller = CreateController();

            ActionResult<ReloadAlarmConfigsResponse> result = await controller.Reload(CancellationToken.None);

            OkObjectResult okResult = (OkObjectResult)result.Result!;
            ReloadAlarmConfigsResponse response = (ReloadAlarmConfigsResponse)okResult.Value!;
            Assert.AreEqual(2, response.ConfigCount);
            Assert.AreEqual("PCS-FAULT", _cache.GetEnabled(PpcAlarmType.PcsAlarm).Single().AlarmId);
        }


        /// <summary>
        /// A reload replaces the configurations loaded at startup.
        /// </summary>
        [TestMethod]
        public async Task ReloadReplacesLoadedConfigsTest()
        {
            SetupConfigs(CreateConfig(1, "PCS-FAULT", PpcAlarmType.PcsAlarm));
            await _cache.LoadAsync(_repository.Object);
            SetupConfigs(CreateConfig(2, "PCS-WARNING", PpcAlarmType.PcsAlarm));
            AlarmConfigsController controller = CreateController();

            await controller.Reload(CancellationToken.None);

            Assert.AreEqual("PCS-WARNING", _cache.GetEnabled(PpcAlarmType.PcsAlarm).Single().AlarmId);
        }


        /// <summary>
        /// When the database is not available the reload returns 503 and the loaded configurations stay in use.
        /// </summary>
        [TestMethod]
        public async Task ReloadDatabaseNotAvailableReturnsServiceUnavailableTest()
        {
            SetupConfigs(CreateConfig(1, "PCS-FAULT", PpcAlarmType.PcsAlarm));
            await _cache.LoadAsync(_repository.Object);
            _repository.Setup(repository => repository.GetAllEnabledAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));
            AlarmConfigsController controller = CreateController();

            ActionResult<ReloadAlarmConfigsResponse> result = await controller.Reload(CancellationToken.None);

            ObjectResult problemResult = (ObjectResult)result.Result!;
            Assert.AreEqual(StatusCodes.Status503ServiceUnavailable, problemResult.StatusCode);
            Assert.AreEqual("PCS-FAULT", _cache.GetEnabled(PpcAlarmType.PcsAlarm).Single().AlarmId);
        }


        /// <summary>
        /// An unexpected error is not hidden as unavailable database, it results in an internal server error.
        /// </summary>
        [TestMethod]
        public async Task ReloadUnexpectedErrorThrowsTest()
        {
            _repository.Setup(repository => repository.GetAllEnabledAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Unexpected mapping error."));
            AlarmConfigsController controller = CreateController();

            Func<Task> reload = () => controller.Reload(CancellationToken.None);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(reload);
        }


        /// <summary>
        /// Sets up the repository to return the enabled configurations.
        /// </summary>
        private void SetupConfigs(params PpcAlarmConfig[] configs)
        {
            _repository.Setup(repository => repository.GetAllEnabledAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(configs.ToList());
        }


        /// <summary>
        /// Creates the controller with an HTTP context, like in a request.
        /// </summary>
        private AlarmConfigsController CreateController()
        {
            return new AlarmConfigsController(NullLogger<AlarmConfigsController>.Instance, _cache, _repository.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }


        /// <summary>
        /// Creates an enabled configuration with a fault state dictionary.
        /// </summary>
        private PpcAlarmConfig CreateConfig(int id, string alarmId, PpcAlarmType ppcAlarmType)
        {
            return new PpcAlarmConfig
            {
                Id = id,
                ChangedBy = "Homer",
                ChangedDate = ChangedOn,
                IsEnabled = true,
                AlarmId = alarmId,
                Name = $"{{{{{alarmId}.Name}}}}",
                SeverityLevel = AlarmSeverity.Error,
                Description = $"{{{{{alarmId}.Desc}}}}",
                PpcAlarmType = ppcAlarmType,
                PropertyName = "FaultStates",
                PropertyOperator = PropertyOperator.Equal,
                PropertyValue = 1
            };
        }
    }
}

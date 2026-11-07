// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Alarms.Application.Caches;
using paskalON.Alarms.Application.UnitTest.SampleData;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.UnitTest.Caches
{
    /// <summary>
    /// Tests <see cref="PpcAlarmConfigCache"/> with a mocked configuration repository.
    /// </summary>
    [TestClass]
    public class PpcAlarmConfigCacheTest
    {
        /// <summary>
        /// Mocked configuration repository.
        /// </summary>
        private readonly Mock<IPpcAlarmConfigRepository> _repository = new Mock<IPpcAlarmConfigRepository>();


        /// <summary>
        /// The loaded configurations are returned per alarm type.
        /// </summary>
        [TestMethod]
        public async Task LoadAsyncGroupsConfigsByAlarmTypeTest()
        {
            SetupConfigs(PcsAlarmConfigSampleData.CreatePcsFaultConfig(), CreateSystemRtuConfig());
            PpcAlarmConfigCache cache = CreateCache();

            int count = await cache.LoadAsync(_repository.Object);

            Assert.AreEqual(2, count);
            Assert.IsTrue(cache.IsLoaded);
            Assert.AreEqual("PCS-FAULT", cache.GetEnabled(PpcAlarmType.PcsAlarm).Single().AlarmId);
            Assert.AreEqual("SYS-RTU", cache.GetEnabled(PpcAlarmType.SystemAlarm).Single().AlarmId);
        }


        /// <summary>
        /// An alarm type without configurations returns no configurations.
        /// </summary>
        [TestMethod]
        public async Task GetEnabledTypeWithoutConfigsReturnsEmptyTest()
        {
            SetupConfigs(PcsAlarmConfigSampleData.CreatePcsFaultConfig());
            PpcAlarmConfigCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);

            IReadOnlyList<PpcAlarmConfig> configs = cache.GetEnabled(PpcAlarmType.BatteryAlarm);

            Assert.IsEmpty(configs);
        }


        /// <summary>
        /// Not loaded configurations throw, an evaluation without configurations would normalize all active alarms.
        /// </summary>
        [TestMethod]
        public void GetEnabledNotLoadedThrowsTest()
        {
            PpcAlarmConfigCache cache = CreateCache();

            Action getEnabled = () => cache.GetEnabled(PpcAlarmType.PcsAlarm);

            Assert.ThrowsExactly<InvalidOperationException>(getEnabled);
            Assert.IsFalse(cache.IsLoaded);
        }


        /// <summary>
        /// A reload replaces the cached configurations, e.g. a removed configuration is no longer returned.
        /// </summary>
        [TestMethod]
        public async Task LoadAsyncReloadReplacesConfigsTest()
        {
            SetupConfigs(PcsAlarmConfigSampleData.CreatePcsFaultConfig(), CreateSystemRtuConfig());
            PpcAlarmConfigCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);
            SetupConfigs(CreateSystemRtuConfig());

            await cache.LoadAsync(_repository.Object);

            Assert.IsEmpty(cache.GetEnabled(PpcAlarmType.PcsAlarm));
            Assert.HasCount(1, cache.GetEnabled(PpcAlarmType.SystemAlarm));
        }


        /// <summary>
        /// A failed reload (e.g. database not available) keeps the previously cached configurations.
        /// </summary>
        [TestMethod]
        public async Task LoadAsyncReloadFailsKeepsConfigsTest()
        {
            SetupConfigs(PcsAlarmConfigSampleData.CreatePcsFaultConfig());
            PpcAlarmConfigCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);
            _repository.Setup(repository => repository.GetAllEnabledAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));

            Func<Task> reload = () => cache.LoadAsync(_repository.Object);

            await Assert.ThrowsExactlyAsync<TimeoutException>(reload);
            Assert.AreEqual("PCS-FAULT", cache.GetEnabled(PpcAlarmType.PcsAlarm).Single().AlarmId);
        }


        /// <summary>
        /// Overlapping reloads run one after another: the later started reload replaces the configurations last,
        /// although the earlier reload finishes loading later.
        /// </summary>
        [TestMethod]
        public async Task LoadAsyncOverlappingReloadsKeepLaterConfigsTest()
        {
            TaskCompletionSource<List<PpcAlarmConfig>> slowOldLoad = new TaskCompletionSource<List<PpcAlarmConfig>>(TaskCreationOptions.RunContinuationsAsynchronously);
            _repository.SetupSequence(repository => repository.GetAllEnabledAsync(It.IsAny<CancellationToken>()))
                .Returns(slowOldLoad.Task)
                .ReturnsAsync(new List<PpcAlarmConfig> { CreateSystemRtuConfig() });
            PpcAlarmConfigCache cache = CreateCache();
            Task<int> oldReload = cache.LoadAsync(_repository.Object);
            Task<int> newReload = cache.LoadAsync(_repository.Object);
            bool isNewReloadWaiting = newReload.IsCompleted is false;

            slowOldLoad.SetResult(new List<PpcAlarmConfig> { PcsAlarmConfigSampleData.CreatePcsFaultConfig() });

            await Task.WhenAll(oldReload, newReload).WaitAsync(TimeSpan.FromSeconds(10));
            Assert.IsTrue(isNewReloadWaiting);
            Assert.IsEmpty(cache.GetEnabled(PpcAlarmType.PcsAlarm));
            Assert.HasCount(1, cache.GetEnabled(PpcAlarmType.SystemAlarm));
            _repository.Verify(repository => repository.GetAllEnabledAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
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
        /// Creates the cache.
        /// </summary>
        private PpcAlarmConfigCache CreateCache()
        {
            return new PpcAlarmConfigCache(NullLogger<PpcAlarmConfigCache>.Instance);
        }


        /// <summary>
        /// Creates the system configuration of a lost RTU communication.
        /// </summary>
        private PpcAlarmConfig CreateSystemRtuConfig()
        {
            return new PpcAlarmConfig
            {
                Id = 3,
                ChangedBy = "Homer",
                ChangedDate = PcsAlarmConfigSampleData.ChangedOn,
                IsEnabled = true,
                AlarmId = "SYS-RTU",
                Name = "{{SYS.Rtu.Name}}",
                SeverityLevel = AlarmSeverity.Critical,
                Description = "{{SYS.Rtu.Desc}}",
                PpcAlarmType = PpcAlarmType.SystemAlarm,
                PropertyName = "IsRtuCommunicationLost",
                PropertyOperator = PropertyOperator.Equal,
                PropertyValue = 1
            };
        }
    }
}

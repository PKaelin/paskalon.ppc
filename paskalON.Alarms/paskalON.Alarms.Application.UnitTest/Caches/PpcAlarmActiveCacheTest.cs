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
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Summaries;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.UnitTest.Caches
{
    /// <summary>
    /// Tests <see cref="PpcAlarmActiveCache"/> with a mocked alarm repository.
    /// </summary>
    [TestClass]
    public class PpcAlarmActiveCacheTest
    {
        /// <summary>
        /// Key of PCS 7.
        /// </summary>
        private static readonly PpcAlarmKey Pcs7Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 7 };


        /// <summary>
        /// Key of PCS 8.
        /// </summary>
        private static readonly PpcAlarmKey Pcs8Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = 8 };


        /// <summary>
        /// Time of the summary calculation.
        /// </summary>
        private static readonly DateTimeOffset CalculatedOn = new DateTimeOffset(2026, 11, 7, 10, 30, 0, TimeSpan.Zero);


        /// <summary>
        /// Mocked alarm repository.
        /// </summary>
        private readonly Mock<IPpcAlarmRepository> _repository = new Mock<IPpcAlarmRepository>();


        /// <summary>
        /// PCS fault configuration of the alarms.
        /// </summary>
        private readonly PpcAlarmConfig _faultConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();


        /// <summary>
        /// The loaded active alarms are returned per publisher, a publisher without active alarms returns none.
        /// </summary>
        [TestMethod]
        public async Task LoadAsyncGroupsAlarmsByPublisherTest()
        {
            PpcAlarm pcs7Alarm = CreateAlarm(7, "FaultStates.OverTemperature");
            PpcAlarm pcs8Alarm = CreateAlarm(8, "FaultStates.GridLoss");
            SetupAllActive(pcs7Alarm, pcs8Alarm);
            PpcAlarmActiveCache cache = CreateCache();

            int count = await cache.LoadAsync(_repository.Object);

            Assert.AreEqual(2, count);
            Assert.IsTrue(cache.IsLoaded);
            Assert.AreSame(pcs7Alarm, (await cache.GetActiveAsync(Pcs7Key, _repository.Object)).Single());
            Assert.AreSame(pcs8Alarm, (await cache.GetActiveAsync(Pcs8Key, _repository.Object)).Single());
            Assert.IsEmpty(await cache.GetActiveAsync(new PpcAlarmKey { PpcAlarmType = PpcAlarmType.BatteryAlarm, DeviceId = 7 }, _repository.Object));
            _repository.Verify(repository => repository.GetActiveAsync(It.IsAny<PpcAlarmKey>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// Not loaded active alarms throw, an evaluation without active alarms would raise them again.
        /// </summary>
        [TestMethod]
        public async Task GetActiveAsyncNotLoadedThrowsTest()
        {
            PpcAlarmActiveCache cache = CreateCache();

            Func<Task> getActive = () => cache.GetActiveAsync(Pcs7Key, _repository.Object);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(getActive);
        }


        /// <summary>
        /// A saved evaluation adds the raised and removes the normalized alarms of the publisher.
        /// </summary>
        [TestMethod]
        public async Task ApplyAddsRaisedAndRemovesNormalizedAlarmsTest()
        {
            PpcAlarm overTemperatureAlarm = CreateAlarm(7, "FaultStates.OverTemperature");
            SetupAllActive(overTemperatureAlarm, CreateAlarm(8, "FaultStates.GridLoss"));
            PpcAlarmActiveCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);
            PpcAlarm gridLossAlarm = CreateAlarm(7, "FaultStates.GridLoss");
            PpcAlarmEvaluation evaluation = new PpcAlarmEvaluation();
            evaluation.Normalized.Add(overTemperatureAlarm);
            evaluation.Raised.Add(gridLossAlarm);

            cache.Apply(Pcs7Key, evaluation);

            Assert.AreSame(gridLossAlarm, (await cache.GetActiveAsync(Pcs7Key, _repository.Object)).Single());
            Assert.HasCount(1, await cache.GetActiveAsync(Pcs8Key, _repository.Object));
        }


        /// <summary>
        /// The returned list is a copy, changing it does not change the cached alarms.
        /// </summary>
        [TestMethod]
        public async Task GetActiveAsyncReturnsCopyOfCachedAlarmsTest()
        {
            SetupAllActive(CreateAlarm(7, "FaultStates.OverTemperature"));
            PpcAlarmActiveCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);
            List<PpcAlarm> alarms = await cache.GetActiveAsync(Pcs7Key, _repository.Object);

            alarms.Clear();

            Assert.HasCount(1, await cache.GetActiveAsync(Pcs7Key, _repository.Object));
        }


        /// <summary>
        /// An invalidated publisher is loaded again from the database once, other publishers stay cached.
        /// </summary>
        [TestMethod]
        public async Task GetActiveAsyncInvalidatedPublisherReloadsOnceTest()
        {
            PpcAlarm cachedAlarm = CreateAlarm(7, "FaultStates.OverTemperature");
            SetupAllActive(cachedAlarm, CreateAlarm(8, "FaultStates.GridLoss"));
            PpcAlarmActiveCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);
            PpcAlarm savedAlarm = CreateAlarm(7, "FaultStates.OverTemperature");
            _repository.Setup(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PpcAlarm> { savedAlarm });
            cache.Invalidate(Pcs7Key);

            List<PpcAlarm> reloadedAlarms = await cache.GetActiveAsync(Pcs7Key, _repository.Object);

            Assert.AreSame(savedAlarm, reloadedAlarms.Single());
            Assert.AreSame(savedAlarm, (await cache.GetActiveAsync(Pcs7Key, _repository.Object)).Single());
            await cache.GetActiveAsync(Pcs8Key, _repository.Object);
            _repository.Verify(repository => repository.GetActiveAsync(It.IsAny<PpcAlarmKey>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// When reloading an invalidated publisher fails (e.g. database still not available) it stays invalidated.
        /// </summary>
        [TestMethod]
        public async Task GetActiveAsyncReloadFailsStaysInvalidatedTest()
        {
            SetupAllActive(CreateAlarm(7, "FaultStates.OverTemperature"));
            PpcAlarmActiveCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);
            _repository.SetupSequence(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."))
                .ReturnsAsync(new List<PpcAlarm>());
            cache.Invalidate(Pcs7Key);
            await Assert.ThrowsExactlyAsync<TimeoutException>(() => cache.GetActiveAsync(Pcs7Key, _repository.Object));

            List<PpcAlarm> alarms = await cache.GetActiveAsync(Pcs7Key, _repository.Object);

            Assert.IsEmpty(alarms);
            _repository.Verify(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        /// <summary>
        /// The keys are the publishers with active alarms and the invalidated publishers.
        /// </summary>
        [TestMethod]
        public async Task GetKeysReturnsPublishersWithAlarmsAndInvalidatedTest()
        {
            SetupAllActive(CreateAlarm(7, "FaultStates.OverTemperature"));
            PpcAlarmActiveCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);
            PpcAlarmKey batteryKey = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.BatteryAlarm, DeviceId = 3 };
            cache.Invalidate(batteryKey);

            IReadOnlyList<PpcAlarmKey> keys = cache.GetKeys();

            CollectionAssert.AreEquivalent(new List<PpcAlarmKey> { Pcs7Key, batteryKey }, keys.ToList());
        }


        /// <summary>
        /// The summary counts the alarms in the sequence state Alarm per severity level of all publishers,
        /// acknowledged alarms are counted separately.
        /// </summary>
        [TestMethod]
        public async Task GetSummaryAsyncCountsUnacknowledgedAndAcknowledgedPerSeverityTest()
        {
            PpcAlarmConfig warningConfig = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            warningConfig.Id = 2;
            warningConfig.SeverityLevel = AlarmSeverity.Warning;
            PpcAlarm acknowledgedAlarm = CreateAlarm(8, "FaultStates.GridLoss");
            acknowledgedAlarm.Acknowledge("Homer", CalculatedOn);
            SetupAllActive(CreateAlarm(7, "FaultStates.OverTemperature"), CreateAlarm(8, "FaultStates.OverTemperature"), acknowledgedAlarm,
                PcsAlarmConfigSampleData.CreateActiveAlarm(warningConfig, 7, "FaultStates.OverCurrent"));
            PpcAlarmActiveCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);

            AlarmSummaryDto summary = await cache.GetSummaryAsync(_repository.Object, CalculatedOn);

            Assert.AreEqual(CalculatedOn, summary.CalculatedOn);
            Assert.AreEqual(0, summary.InfoCount);
            Assert.AreEqual(1, summary.WarningCount);
            Assert.AreEqual(2, summary.ErrorCount);
            Assert.AreEqual(0, summary.CriticalCount);
            Assert.AreEqual(0, summary.AcknowledgedWarningCount);
            Assert.AreEqual(1, summary.AcknowledgedErrorCount);
        }


        /// <summary>
        /// The invalidated publishers are loaded from the database before the alarms are counted.
        /// </summary>
        [TestMethod]
        public async Task GetSummaryAsyncReloadsInvalidatedPublishersTest()
        {
            SetupAllActive(CreateAlarm(7, "FaultStates.OverTemperature"));
            PpcAlarmActiveCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);
            _repository.Setup(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PpcAlarm> { CreateAlarm(7, "FaultStates.OverTemperature"), CreateAlarm(7, "FaultStates.GridLoss") });
            cache.Invalidate(Pcs7Key);

            AlarmSummaryDto summary = await cache.GetSummaryAsync(_repository.Object, CalculatedOn);

            Assert.AreEqual(2, summary.ErrorCount);
            Assert.HasCount(2, await cache.GetActiveAsync(Pcs7Key, _repository.Object));
            _repository.Verify(repository => repository.GetActiveAsync(Pcs7Key, It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// A load replaces the cached alarms and clears the invalidated publishers.
        /// </summary>
        [TestMethod]
        public async Task LoadAsyncReplacesAlarmsAndClearsInvalidatedTest()
        {
            SetupAllActive(CreateAlarm(7, "FaultStates.OverTemperature"));
            PpcAlarmActiveCache cache = CreateCache();
            await cache.LoadAsync(_repository.Object);
            cache.Invalidate(Pcs7Key);
            PpcAlarm pcs8Alarm = CreateAlarm(8, "FaultStates.GridLoss");
            SetupAllActive(pcs8Alarm);

            await cache.LoadAsync(_repository.Object);

            Assert.IsEmpty(await cache.GetActiveAsync(Pcs7Key, _repository.Object));
            Assert.AreSame(pcs8Alarm, (await cache.GetActiveAsync(Pcs8Key, _repository.Object)).Single());
            _repository.Verify(repository => repository.GetActiveAsync(It.IsAny<PpcAlarmKey>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// Sets up the repository to return the active alarms of all publishers.
        /// </summary>
        private void SetupAllActive(params PpcAlarm[] alarms)
        {
            _repository.Setup(repository => repository.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(alarms.ToList());
        }


        /// <summary>
        /// Creates the cache.
        /// </summary>
        private static PpcAlarmActiveCache CreateCache()
        {
            return new PpcAlarmActiveCache(NullLogger<PpcAlarmActiveCache>.Instance);
        }


        /// <summary>
        /// Creates an active PCS fault alarm.
        /// </summary>
        private PpcAlarm CreateAlarm(int deviceId, string propertyName)
        {
            return PcsAlarmConfigSampleData.CreateActiveAlarm(_faultConfig, deviceId, propertyName);
        }
    }
}

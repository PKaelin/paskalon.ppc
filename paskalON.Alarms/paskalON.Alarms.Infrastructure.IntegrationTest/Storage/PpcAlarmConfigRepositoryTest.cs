// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Infrastructure.IntegrationTest.Storage.SampleData;
using paskalON.Alarms.Infrastructure.Storage;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Infrastructure.IntegrationTest.Storage
{
    /// <summary>
    /// Tests <see cref="PpcAlarmConfigRepository"/> against a migrated PostgreSQL database.
    /// </summary>
    [TestClass]
    public class PpcAlarmConfigRepositoryTest : DatabaseTestBase
    {
        /// <summary>
        /// Seeds the PCS fault, warning, grid loss, a disabled PCS and the system configurations.
        /// The grid loss configuration shares the actions of the fault configuration.
        /// </summary>
        protected override async Task SeedAsync(AlarmContext db)
        {
            // override DatabaseTestBase -> PostgresTestBase
            PpcAlarmConfig disabledConfig = PcsAlarmSampleData.CreatePcsWarningConfig();
            disabledConfig.AlarmId = "PCS-DISABLED";
            disabledConfig.PropertyName = "VendorEvents";
            disabledConfig.IsEnabled = false;
            PpcAlarmConfig faultConfig = PcsAlarmSampleData.CreatePcsFaultConfig();
            PpcAlarmConfig gridLossConfig = PcsAlarmSampleData.CreatePcsGridLossConfig();
            gridLossConfig.AlarmActions.AddRange(faultConfig.AlarmActions);
            db.PpcAlarmConfigs.Add(faultConfig);
            db.PpcAlarmConfigs.Add(PcsAlarmSampleData.CreatePcsWarningConfig());
            db.PpcAlarmConfigs.Add(gridLossConfig);
            db.PpcAlarmConfigs.Add(PcsAlarmSampleData.CreateSystemRtuConfig());
            db.PpcAlarmConfigs.Add(disabledConfig);
            await db.SaveChangesAsync();
        }


        /// <summary>
        /// Only the enabled configurations of the alarm type are returned.
        /// </summary>
        [TestMethod]
        [DataRow(PpcAlarmType.PcsAlarm, new string[] { "PCS-FAULT", "PCS-WARNING", "PCS-GRIDLOSS" })]
        [DataRow(PpcAlarmType.SystemAlarm, new string[] { "SYS-RTU" })]
        [DataRow(PpcAlarmType.BatteryAlarm, new string[] { })]
        public async Task GetEnabledAsyncReturnsEnabledConfigsOfTypeTest(PpcAlarmType type, string[] expectedAlarmIds)
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfigRepository repository = new PpcAlarmConfigRepository(NullLogger<PpcAlarmConfigRepository>.Instance, context);

            List<PpcAlarmConfig> configs = await repository.GetEnabledAsync(type);

            CollectionAssert.AreEquivalent(expectedAlarmIds, configs.Select(config => config.AlarmId).ToArray());
        }


        /// <summary>
        /// The enabled configurations of all alarm types are returned with their actions, not tracked so that they can be cached.
        /// </summary>
        [TestMethod]
        public async Task GetAllEnabledAsyncReturnsUntrackedEnabledConfigsOfAllTypesTest()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfigRepository repository = new PpcAlarmConfigRepository(NullLogger<PpcAlarmConfigRepository>.Instance, context);

            List<PpcAlarmConfig> configs = await repository.GetAllEnabledAsync();

            CollectionAssert.AreEquivalent(new string[] { "PCS-FAULT", "PCS-WARNING", "PCS-GRIDLOSS", "SYS-RTU" },
                configs.Select(config => config.AlarmId).ToArray());
            Assert.HasCount(2, configs.Single(config => config.AlarmId == "PCS-FAULT").AlarmActions);
            Assert.IsEmpty(context.ChangeTracker.Entries());
        }


        /// <summary>
        /// An action shared by several configurations is stored once and returned as one instance,
        /// so that the cache holds one instance per action.
        /// </summary>
        [TestMethod]
        public async Task GetAllEnabledAsyncSharedActionsAreOneInstanceTest()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfigRepository repository = new PpcAlarmConfigRepository(NullLogger<PpcAlarmConfigRepository>.Instance, context);

            List<PpcAlarmConfig> configs = await repository.GetAllEnabledAsync();

            PpcAlarmConfig faultConfig = configs.Single(config => config.AlarmId == "PCS-FAULT");
            PpcAlarmConfig gridLossConfig = configs.Single(config => config.AlarmId == "PCS-GRIDLOSS");
            Assert.AreSame(faultConfig.AlarmActions.Single(action => action.Name == "ReducePower"),
                gridLossConfig.AlarmActions.Single(action => action.Name == "ReducePower"));
            Assert.HasCount(2, gridLossConfig.AlarmActions.Single(action => action.Name == "ReducePower").AlarmActionParameters);
            Assert.AreEqual(2, await context.AlarmActionConfigs.CountAsync());
        }


        /// <summary>
        /// The configurations are returned with their actions and action parameters.
        /// </summary>
        [TestMethod]
        public async Task GetEnabledAsyncIncludesActionsAndParametersTest()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfigRepository repository = new PpcAlarmConfigRepository(NullLogger<PpcAlarmConfigRepository>.Instance, context);

            List<PpcAlarmConfig> configs = await repository.GetEnabledAsync(PpcAlarmType.PcsAlarm);

            PpcAlarmConfig faultConfig = configs.Single(config => config.AlarmId == "PCS-FAULT");
            Assert.HasCount(2, faultConfig.AlarmActions);
            AlarmActionConfig reducePower = faultConfig.AlarmActions.Single(action => action.Name == "ReducePower");
            Assert.AreEqual(AlarmActionTrigger.Raise, reducePower.Trigger);
            Assert.AreEqual("{{DeviceId}}", reducePower.AlarmActionParameters.Single(parameter => parameter.Key == "DeviceId").Value);
            Assert.AreEqual("50", reducePower.AlarmActionParameters.Single(parameter => parameter.Key == "Percentage").Value);
            AlarmActionConfig resetPower = faultConfig.AlarmActions.Single(action => action.Name == "ResetPower");
            Assert.AreEqual(AlarmActionTrigger.Normalize, resetPower.Trigger);
            Assert.IsEmpty(configs.Single(config => config.AlarmId == "PCS-WARNING").AlarmActions);
        }


        /// <summary>
        /// All fields of a configuration are persisted, the time stamps in UTC.
        /// </summary>
        [TestMethod]
        public async Task GetEnabledAsyncReturnsPersistedFieldsTest()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfigRepository repository = new PpcAlarmConfigRepository(NullLogger<PpcAlarmConfigRepository>.Instance, context);

            List<PpcAlarmConfig> configs = await repository.GetEnabledAsync(PpcAlarmType.PcsAlarm);

            PpcAlarmConfig gridLossConfig = configs.Single(config => config.AlarmId == "PCS-GRIDLOSS");
            Assert.AreEqual("{{PCS.GridLoss.Name}}", gridLossConfig.Name);
            Assert.AreEqual("{{PCS.GridLoss.Desc}}", gridLossConfig.Description);
            Assert.AreEqual("{{PCS.GridLoss.Causes}}", gridLossConfig.PossibleCauses);
            Assert.AreEqual(Domain.Configs.AlarmSeverity.Critical, gridLossConfig.SeverityLevel);
            Assert.IsNull(gridLossConfig.DeviceId);
            Assert.AreEqual("FaultStates.GridLoss", gridLossConfig.PropertyName);
            Assert.AreEqual(PropertyOperator.Equal, gridLossConfig.PropertyOperator);
            Assert.AreEqual(1, gridLossConfig.PropertyValue);
            Assert.AreEqual(PcsAlarmSampleData.ChangedOn, gridLossConfig.ChangedDate);
        }
    }
}

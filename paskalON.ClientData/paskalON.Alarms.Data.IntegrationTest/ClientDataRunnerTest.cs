// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Infrastructure.Storage;
using paskalON.ClientData.Common;
using paskalON.Infrastructure.PostgreSql.Tests;

namespace paskalON.Alarms.Data.IntegrationTest
{
    /// <summary>
    /// Tests of the class <see cref="ClientDataRunner"/> with the alarm service database.
    /// </summary>
    [TestClass]
    public class ClientDataRunnerTest : PostgresTestBase<AlarmContext>
    {
        /// <summary>
        /// Client project used by the tests of a single project.
        /// </summary>
        private const string ClientProjectName = "DemoSuperSimpleBattery";


        /// <summary>
        /// Number of device types with a communication error configuration.
        /// </summary>
        private const int CommunicationErrorConfigCount = 7;


        /// <summary>
        /// Folder of the demo client project files.
        /// </summary>
        private static readonly string _demoDirectory = Path.Combine(AppContext.BaseDirectory, "ClientData", "Demo");


        /// <summary>
        /// Client project file used by the tests of a single project.
        /// </summary>
        private static readonly string _clientProjectFile = Path.Combine(_demoDirectory, $"{ClientProjectName}.csv");


        /// <summary>
        /// Gets the client project names of all demo CSV files in the folder ClientData/Demo.
        /// </summary>
        public static IEnumerable<object[]> DemoClientProjectNames => Directory
            .EnumerateFiles(_demoDirectory, "*.csv")
            .Select(file => new object[] { Path.GetFileNameWithoutExtension(file) })
            .OrderBy(x => (string)x[0], StringComparer.Ordinal);


        /// <summary>
        /// Every demo client project is imported with the system configuration and the communication error configurations.
        /// </summary>
        /// <param name="clientProjectName">Name of the demo client project.</param>
        [TestMethod]
        [DynamicData(nameof(DemoClientProjectNames))]
        public async Task RunAsyncImportsDemoClientDataTest(string clientProjectName)
        {
            string[] args = new string[] { clientProjectName, Path.Combine(_demoDirectory, $"{clientProjectName}.csv") };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual(ClientDataLoader.ChangedBy, systemConfig.ChangedBy);
            Assert.AreEqual(CommunicationErrorConfigCount, await context.PpcAlarmConfigs.CountAsync());
        }


        /// <summary>
        /// The configuration and the system configuration with the alarm action exchange, intervals and subscriber topics are imported.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncImportsSystemConfigTest()
        {
            string[] args = new string[] { ClientProjectName, _clientProjectFile };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            Configuration configuration = await context.Configurations.SingleAsync();
            Assert.AreEqual("AuditCleanUpInMonths", configuration.Key);
            Assert.AreEqual("60", configuration.Value);
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual("ppc:alarms:actions", systemConfig.PublisherExchangeAlarmActions);
            Assert.AreEqual(1000, systemConfig.AlarmActionSendIntervalMilliseconds);
            Assert.AreEqual(5000, systemConfig.AlarmActionRetryIntervalMilliseconds);
            Assert.AreEqual("ppc:device:pcs:core", systemConfig.SubscriberTopicPcsCore);
            Assert.AreEqual("ppc:device:pcs:detail", systemConfig.SubscriberTopicPcsDetail);
            Assert.AreEqual("ppc:device:bb:core", systemConfig.SubscriberTopicBatteryBankCore);
            Assert.AreEqual("ppc:device:bb:detail", systemConfig.SubscriberTopicBatteryBankDetail);
            Assert.AreEqual("ppc:device:pv:core", systemConfig.SubscriberTopicSolarPanelCore);
            Assert.AreEqual("ppc:device:pv:detail", systemConfig.SubscriberTopicSolarPanelDetail);
            Assert.AreEqual("ppc:device:pm:external:core", systemConfig.SubscriberTopicExternalPowerMeterCore);
            Assert.AreEqual("ppc:device:pm:external:detail", systemConfig.SubscriberTopicExternalPowerMeterDetail);
            Assert.AreEqual("ppc:device:pm:auxiliary:core", systemConfig.SubscriberTopicAuxiliaryPowerMeterCore);
            Assert.AreEqual("ppc:device:pm:auxiliary:detail", systemConfig.SubscriberTopicAuxiliaryPowerMeterDetail);
            Assert.AreEqual("ppc:device:pm:circuit:core", systemConfig.SubscriberTopicCircuitPowerMeterCore);
            Assert.AreEqual("ppc:device:pm:circuit:detail", systemConfig.SubscriberTopicCircuitPowerMeterDetail);
            Assert.AreEqual("ppc:device:pm:system:core", systemConfig.SubscriberTopicSystemPowerMeterCore);
            Assert.AreEqual("ppc:device:pm:system:detail", systemConfig.SubscriberTopicSystemPowerMeterDetail);
            Assert.AreEqual("ppc:pc:targets", systemConfig.SubscriberTopicPowerTarget);
            Assert.AreEqual("ppc:om:setpoints", systemConfig.SubscriberTopicCurrentSetpoint);
        }


        /// <summary>
        /// Every device type has an enabled configuration for all its devices that raises an error alarm on a communication error.
        /// </summary>
        /// <param name="ppcAlarmType">Alarm type of the device type.</param>
        /// <param name="alarmId">Expected alarm id.</param>
        [TestMethod]
        [DataRow(PpcAlarmType.PcsAlarm, "PCS-COMM")]
        [DataRow(PpcAlarmType.BatteryAlarm, "BB-COMM")]
        [DataRow(PpcAlarmType.SolarPanelAlarm, "PV-COMM")]
        [DataRow(PpcAlarmType.ExternalPowerMeterAlarm, "PM-EXT-COMM")]
        [DataRow(PpcAlarmType.AuxiliaryPowerMeterAlarm, "PM-AUX-COMM")]
        [DataRow(PpcAlarmType.CircuitPowerMeterAlarm, "PM-CIR-COMM")]
        [DataRow(PpcAlarmType.SystemPowerMeterAlarm, "PM-SYS-COMM")]
        public async Task RunAsyncImportsCommunicationErrorConfigOfDeviceTypeTest(PpcAlarmType ppcAlarmType, string alarmId)
        {
            string[] args = new string[] { ClientProjectName, _clientProjectFile };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfig config = await context.PpcAlarmConfigs.Include(x => x.AlarmActions).SingleAsync(x => x.PpcAlarmType == ppcAlarmType);
            Assert.AreEqual(alarmId, config.AlarmId);
            Assert.IsTrue(config.IsEnabled);
            Assert.AreEqual(AlarmSeverity.Error, config.SeverityLevel);
            Assert.IsNull(config.DeviceId);
            Assert.AreEqual("CommunicationError", config.PropertyName);
            Assert.AreEqual(PropertyOperator.Equal, config.PropertyOperator);
            Assert.AreEqual(1, config.PropertyValue);
            Assert.AreEqual(0, config.TimeBeforeRaisingAlarmMilliseconds);
            Assert.AreEqual(ClientDataLoader.ChangedBy, config.ChangedBy);
            AlarmActionConfig action = config.AlarmActions.Single();
            Assert.AreEqual("ShowAlarm", action.Name);
            Assert.AreEqual(AlarmActionTrigger.Raise, action.Trigger);
        }


        /// <summary>
        /// The show alarm action is stored once and shared by all communication error configurations,
        /// its parameters pass the values of the raised alarm with placeholders.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncImportsSharedShowAlarmActionTest()
        {
            string[] args = new string[] { ClientProjectName, _clientProjectFile };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            AlarmActionConfig action = await context.AlarmActionConfigs.Include(x => x.AlarmActionParameters).SingleAsync();
            Dictionary<string, string> parameters = action.AlarmActionParameters.ToDictionary(x => x.Key, x => x.Value);
            Assert.HasCount(5, parameters);
            Assert.AreEqual("{{AlarmId}}", parameters["AlarmId"]);
            Assert.AreEqual("{{PpcAlarmType}}", parameters["PpcAlarmType"]);
            Assert.AreEqual("{{SeverityLevel}}", parameters["SeverityLevel"]);
            Assert.AreEqual("{{DeviceId}}", parameters["DeviceId"]);
            Assert.AreEqual("{{PropertyName}}", parameters["PropertyName"]);
            List<PpcAlarmConfig> configs = await context.PpcAlarmConfigs.Include(x => x.AlarmActions).ToListAsync();
            Assert.IsTrue(configs.All(config => config.AlarmActions.Single().Id == action.Id));
        }


        /// <summary>
        /// The database is migrated to the selected schema version before the import.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncMigratesToSelectedSchemaVersionTest()
        {
            string[] args = new string[] { ClientProjectName, _clientProjectFile, "0.1" };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            Assert.AreEqual(CommunicationErrorConfigCount, await context.PpcAlarmConfigs.CountAsync());
        }


        /// <summary>
        /// An unknown schema version is rejected before the database is recreated.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncKeepsDatabaseWhenSchemaVersionIsUnknownTest()
        {
            await AddExistingActionAsync();
            string[] args = new string[] { ClientProjectName, _clientProjectFile, "9.9" };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>);

            Assert.AreEqual((int)ClientDataExitCode.InvalidArguments, exitCode);
            await AssertExistingActionIsKeptAsync();
        }


        /// <summary>
        /// A file with an alarm configuration that lists an action not defined earlier is rejected before the database is recreated.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncKeepsDatabaseWhenFileIsInvalidTest()
        {
            await AddExistingActionAsync();
            string file = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");
            await File.WriteAllLinesAsync(file, new string[]
            {
                "Type=PpcAlarmConfig,Name=PCS communication error,IsEnabled=true,AlarmId=PCS-COMM,SeverityLevel=Error,Description=PCS communication error," +
                "PpcAlarmType=PcsAlarm,PropertyName=CommunicationError,PropertyOperator=Equal,PropertyValue=1,AlarmActions=Missing"
            });
            string[] args = new string[] { ClientProjectName, file };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>);
            File.Delete(file);

            Assert.AreEqual((int)ClientDataExitCode.CsvValidationError, exitCode);
            await AssertExistingActionIsKeptAsync();
        }


        /// <summary>
        /// A client project file that does not exist is rejected as invalid argument.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncReturnsInvalidArgumentsWhenFileDoesNotExistTest()
        {
            string[] args = new string[] { ClientProjectName, "DoesNotExist.csv" };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>);

            Assert.AreEqual((int)ClientDataExitCode.InvalidArguments, exitCode);
        }


        /// <summary>
        /// Adds an entity that must still exist if the import did not start.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task AddExistingActionAsync()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            context.AlarmActionConfigs.Add(new AlarmActionConfig { ChangedBy = "Test", Name = "Existing", Trigger = AlarmActionTrigger.Raise });
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Asserts that the database has not been recreated.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task AssertExistingActionIsKeptAsync()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            AlarmActionConfig action = await context.AlarmActionConfigs.SingleAsync();
            Assert.AreEqual("Existing", action.Name);
        }
    }
}

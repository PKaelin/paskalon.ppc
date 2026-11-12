// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Localizations;
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
        /// Number of PCS fault, warning and vendor event configurations of the PcsSimpleV1 (including Unknown).
        /// </summary>
        private const int PcsEventConfigCount = 13;


        /// <summary>
        /// Number of BB fault, warning and vendor event configurations of the BbSimpleV1 (including Unknown).
        /// </summary>
        private const int BbEventConfigCount = 19;


        /// <summary>
        /// Number of BB state of charge configurations.
        /// </summary>
        private const int BbStateOfChargeConfigCount = 1;


        /// <summary>
        /// Number of all alarm configurations of a demo client project.
        /// </summary>
        private const int PpcAlarmConfigCount = CommunicationErrorConfigCount + PcsEventConfigCount + BbEventConfigCount + BbStateOfChargeConfigCount;


        /// <summary>
        /// Folder of the demo client project files.
        /// </summary>
        private static readonly string _demoDirectory = Path.Combine(AppContext.BaseDirectory, "ClientData", "Demo");


        /// <summary>
        /// Client project file used by the tests of a single project.
        /// </summary>
        private static readonly string _clientProjectFile = Path.Combine(_demoDirectory, $"{ClientProjectName}.csv");


        /// <summary>
        /// Number of English texts of a demo client project: name and description of every alarm configuration.
        /// </summary>
        private const int EnglishTextCount = 2 * PpcAlarmConfigCount;


        /// <summary>
        /// Gets the client project names of all demo client project files in the folder ClientData/Demo, without the text files.
        /// </summary>
        public static IEnumerable<object[]> DemoClientProjectNames => Directory
            .EnumerateFiles(_demoDirectory, "*.csv")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => name!.Contains(LocalizedTextSupplement<LocalizedText>.TextFileInfix, StringComparison.Ordinal) is false)
            .Select(name => new object[] { name! })
            .OrderBy(x => (string)x[0], StringComparer.Ordinal);


        /// <summary>
        /// Every demo client project is imported with the system configuration, the communication error and the PCS event configurations.
        /// </summary>
        /// <param name="clientProjectName">Name of the demo client project.</param>
        [TestMethod]
        [DynamicData(nameof(DemoClientProjectNames))]
        public async Task RunAsyncImportsDemoClientDataTest(string clientProjectName)
        {
            string[] args = new string[] { clientProjectName, Path.Combine(_demoDirectory, $"{clientProjectName}.csv") };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual(ClientDataLoader.ChangedBy, systemConfig.ChangedBy);
            Assert.AreEqual(PpcAlarmConfigCount, await context.PpcAlarmConfigs.CountAsync());
            Assert.AreEqual(EnglishTextCount, await context.LocalizedTexts.CountAsync(text => text.Language == LocalizedText.DefaultLanguage));
        }


        /// <summary>
        /// The English texts of the text file are imported, every localization key of the alarm configurations has an English text.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncImportsEnglishTextOfEveryLocalizationKeyTest()
        {
            string[] args = new string[] { ClientProjectName, _clientProjectFile };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            Dictionary<string, string> texts = await context.LocalizedTexts.Where(text => text.Language == LocalizedText.DefaultLanguage)
                .ToDictionaryAsync(text => text.Name, text => text.Text);
            Assert.HasCount(EnglishTextCount, texts);
            Assert.AreEqual("PCS communication error", texts["PCS.CommunicationError.Name"]);
            Assert.AreEqual("The PPC cannot communicate with the power conversion system.", texts["PCS.CommunicationError.Desc"]);
            List<PpcAlarmConfig> configs = await context.PpcAlarmConfigs.ToListAsync();
            List<string> keys = configs.SelectMany(config => new string[] { config.Name, config.Description }).ToList();
            Assert.IsTrue(keys.All(key => LocalizationKey.TryParse(key, out string textKey) && texts.ContainsKey(textKey)));
        }


        /// <summary>
        /// A localization key without an English text is rejected before the database is recreated.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncKeepsDatabaseWhenEnglishTextIsMissingTest()
        {
            await AddExistingActionAsync();
            string directory = Directory.CreateTempSubdirectory().FullName;
            string file = Path.Combine(directory, "MissingText.csv");
            await File.WriteAllLinesAsync(file, new string[]
            {
                "Type=AlarmActionConfig,Name=RaiseAlarm,Trigger=Raise",
                "Type=PpcAlarmConfig,Name={{PCS.CommunicationError.Name}},IsEnabled=true,AlarmId=PCS-COMM,SeverityLevel=Error,Description={{PCS.CommunicationError.Desc}}," +
                "PpcAlarmType=PcsAlarm,PropertyName=CommunicationError,PropertyOperator=Equal,PropertyValue=1,AlarmActions=RaiseAlarm"
            });
            await File.WriteAllLinesAsync(Path.Combine(directory, "MissingText.Texts.en.csv"), new string[]
            {
                "Type=LocalizedText,Name=PCS.CommunicationError.Name,Language=en,Text=PCS communication error"
            });
            string[] args = new string[] { ClientProjectName, file };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());
            Directory.Delete(directory, true);

            Assert.AreEqual((int)ClientDataExitCode.CsvValidationError, exitCode);
            await AssertExistingActionIsKeptAsync();
        }


        /// <summary>
        /// A text in the client project file instead of a text file is rejected before the database is recreated.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncKeepsDatabaseWhenTextIsInClientProjectFileTest()
        {
            await AddExistingActionAsync();
            string directory = Directory.CreateTempSubdirectory().FullName;
            string file = Path.Combine(directory, "TextInProject.csv");
            await File.WriteAllLinesAsync(file, new string[]
            {
                "Type=LocalizedText,Name=PCS.CommunicationError.Name,Language=en,Text=PCS communication error"
            });
            string[] args = new string[] { ClientProjectName, file };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());
            Directory.Delete(directory, true);

            Assert.AreEqual((int)ClientDataExitCode.CsvValidationError, exitCode);
            await AssertExistingActionIsKeptAsync();
        }


        /// <summary>
        /// The configuration and the system configuration with the alarm action exchange, intervals and subscriber topics are imported.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncImportsSystemConfigTest()
        {
            string[] args = new string[] { ClientProjectName, _clientProjectFile };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            Configuration configuration = await context.Configurations.SingleAsync();
            Assert.AreEqual("AuditCleanUpInMonths", configuration.Key);
            Assert.AreEqual("60", configuration.Value);
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual("ppc:alarms:actions", systemConfig.PublisherExchangeAlarmActions);
            Assert.AreEqual(1000, systemConfig.AlarmActionSendIntervalMilliseconds);
            Assert.AreEqual(5000, systemConfig.AlarmActionRetryIntervalMilliseconds);
            Assert.AreEqual(500, systemConfig.AlarmDelayCheckIntervalMilliseconds);
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
        /// Every device type has an enabled configuration for all its devices that raises a critical alarm on a communication error.
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

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfig config = await context.PpcAlarmConfigs.Include(x => x.AlarmActions)
                .SingleAsync(x => x.PpcAlarmType == ppcAlarmType && x.PropertyName == "CommunicationError");
            Assert.AreEqual(alarmId, config.AlarmId);
            Assert.IsTrue(config.IsEnabled);
            Assert.AreEqual(AlarmSeverity.Critical, config.SeverityLevel);
            Assert.IsNull(config.DeviceId);
            Assert.AreEqual("CommunicationError", config.PropertyName);
            Assert.AreEqual(PropertyOperator.Equal, config.PropertyOperator);
            Assert.AreEqual(1, config.PropertyValue);
            Assert.AreEqual(0, config.TimeBeforeRaisingAlarmMilliseconds);
            Assert.AreEqual(0, config.TimeBeforeNormalizingAlarmMilliseconds);
            Assert.IsTrue(config.IsAcknowledgeable);
            Assert.AreEqual(ClientDataLoader.ChangedBy, config.ChangedBy);
            CollectionAssert.AreEquivalent(GetExpectedActions(config),
                config.AlarmActions.Select(action => (action.Name, action.Trigger)).ToList());
        }


        /// <summary>
        /// Every PcsSimpleV1 fault, warning and vendor event (including Unknown) has an enabled configuration for all PCS
        /// that raises an alarm of its severity when the entry of the detail data dictionary is true.
        /// </summary>
        /// <param name="alarmId">Alarm id of the configuration.</param>
        /// <param name="propertyName">Dictionary entry of the PCS detail data.</param>
        /// <param name="severity">Expected severity level.</param>
        [TestMethod]
        [DataRow("PCS-FAULT-HIGH-INPUT-VOLTAGE", "FaultStates.HighInputVoltage", AlarmSeverity.Error)]
        [DataRow("PCS-FAULT-LOW-INPUT-VOLTAGE", "FaultStates.LowInputVoltage", AlarmSeverity.Error)]
        [DataRow("PCS-FAULT-HIGH-FREQUENCY", "FaultStates.HighFrequency", AlarmSeverity.Error)]
        [DataRow("PCS-FAULT-LOW-FREQUENCY", "FaultStates.LowFrequency", AlarmSeverity.Error)]
        [DataRow("PCS-FAULT-UNKNOWN", "FaultStates.Unknown", AlarmSeverity.Error)]
        [DataRow("PCS-WARN-HIGH-INPUT-VOLTAGE", "WarningStates.HighInputVoltage", AlarmSeverity.Warning)]
        [DataRow("PCS-WARN-LOW-INPUT-VOLTAGE", "WarningStates.LowInputVoltage", AlarmSeverity.Warning)]
        [DataRow("PCS-WARN-HIGH-FREQUENCY", "WarningStates.HighFrequency", AlarmSeverity.Warning)]
        [DataRow("PCS-WARN-LOW-FREQUENCY", "WarningStates.LowFrequency", AlarmSeverity.Warning)]
        [DataRow("PCS-WARN-UNKNOWN", "WarningStates.Unknown", AlarmSeverity.Warning)]
        [DataRow("PCS-EVENT-MAINTENANCE-DUE", "VendorEvents.MaintenanceDue", AlarmSeverity.Info)]
        [DataRow("PCS-EVENT-END-OF-LIFE-DUE", "VendorEvents.EndOfLifeDue", AlarmSeverity.Info)]
        [DataRow("PCS-EVENT-UNKNOWN", "VendorEvents.Unknown", AlarmSeverity.Info)]
        public async Task RunAsyncImportsPcsEventConfigTest(string alarmId, string propertyName, AlarmSeverity severity)
        {
            string[] args = new string[] { ClientProjectName, _clientProjectFile };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfig config = await context.PpcAlarmConfigs.Include(x => x.AlarmActions).SingleAsync(x => x.AlarmId == alarmId);
            Assert.AreEqual(PpcAlarmType.PcsAlarm, config.PpcAlarmType);
            Assert.IsTrue(config.IsEnabled);
            Assert.AreEqual(severity, config.SeverityLevel);
            Assert.IsNull(config.DeviceId);
            Assert.AreEqual(propertyName, config.PropertyName);
            Assert.AreEqual(PropertyOperator.Equal, config.PropertyOperator);
            Assert.AreEqual(1, config.PropertyValue);
            Assert.AreEqual($"{{{{PCS.{propertyName}.Name}}}}", config.Name);
            CollectionAssert.AreEquivalent(GetExpectedActions(config),
                config.AlarmActions.Select(action => (action.Name, action.Trigger)).ToList());
        }


        /// <summary>
        /// Every demo client project has an enabled configuration for all battery banks that raises a warning
        /// when the usable state of charge reaches the usable minimum state of charge (usable state of charge 0).
        /// </summary>
        /// <param name="clientProjectName">Name of the demo client project.</param>
        [TestMethod]
        [DynamicData(nameof(DemoClientProjectNames))]
        public async Task RunAsyncImportsUsableStateOfChargeMinimumConfigTest(string clientProjectName)
        {
            string[] args = new string[] { clientProjectName, Path.Combine(_demoDirectory, $"{clientProjectName}.csv") };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            PpcAlarmConfig config = await context.PpcAlarmConfigs.Include(x => x.AlarmActions).SingleAsync(x => x.AlarmId == "BB-USABLE-SOC-MIN");
            Assert.AreEqual(PpcAlarmType.BatteryAlarm, config.PpcAlarmType);
            Assert.IsTrue(config.IsEnabled);
            Assert.AreEqual(AlarmSeverity.Warning, config.SeverityLevel);
            Assert.IsNull(config.DeviceId);
            Assert.AreEqual("UsableStateOfCharge", config.PropertyName);
            Assert.AreEqual(PropertyOperator.SmallerThanOrEqual, config.PropertyOperator);
            Assert.AreEqual(0, config.PropertyValue);
            Assert.AreEqual("{{BB.UsableStateOfChargeMinimum.Name}}", config.Name);
            Assert.AreEqual("{{BB.UsableStateOfChargeMinimum.Desc}}", config.Description);
            Assert.IsTrue(config.IsAcknowledgeable);
            CollectionAssert.AreEquivalent(GetExpectedActions(config),
                config.AlarmActions.Select(action => (action.Name, action.Trigger)).ToList());
        }


        /// <summary>
        /// The raise, clear and acknowledge alarm actions are stored once and shared by the alarm configurations (acknowledge only by the acknowledgeable ones),
        /// their parameters pass the values of the alarm with placeholders.
        /// </summary>
        /// <param name="actionName">Name of the shared action.</param>
        [TestMethod]
        [DataRow("RaiseAlarm")]
        [DataRow("ClearAlarm")]
        [DataRow("AcknowledgeAlarm")]
        public async Task RunAsyncImportsSharedAlarmActionTest(string actionName)
        {
            string[] args = new string[] { ClientProjectName, _clientProjectFile };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            AlarmActionConfig action = await context.AlarmActionConfigs.Include(x => x.AlarmActionParameters).SingleAsync(x => x.Name == actionName);
            Dictionary<string, string> parameters = action.AlarmActionParameters.ToDictionary(x => x.Key, x => x.Value);
            Assert.HasCount(5, parameters);
            Assert.AreEqual("{{AlarmId}}", parameters["AlarmId"]);
            Assert.AreEqual("{{PpcAlarmType}}", parameters["PpcAlarmType"]);
            Assert.AreEqual("{{SeverityLevel}}", parameters["SeverityLevel"]);
            Assert.AreEqual("{{DeviceId}}", parameters["DeviceId"]);
            Assert.AreEqual("{{PropertyName}}", parameters["PropertyName"]);
            List<PpcAlarmConfig> configs = await context.PpcAlarmConfigs.Include(x => x.AlarmActions).ToListAsync();
            Assert.IsTrue(configs.All(config => config.AlarmActions.Count(configAction => configAction.Id == action.Id)
                == (action.Trigger != AlarmActionTrigger.Acknowledge || config.IsAcknowledgeable ? 1 : 0)));
            Assert.AreEqual(3, await context.AlarmActionConfigs.CountAsync());
        }


        /// <summary>
        /// The database is migrated to the selected schema version before the import.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncMigratesToSelectedSchemaVersionTest()
        {
            string[] args = new string[] { ClientProjectName, _clientProjectFile, "0.1" };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            Assert.AreEqual(PpcAlarmConfigCount, await context.PpcAlarmConfigs.CountAsync());
        }


        /// <summary>
        /// An unknown schema version is rejected before the database is recreated.
        /// </summary>
        [TestMethod]
        public async Task RunAsyncKeepsDatabaseWhenSchemaVersionIsUnknownTest()
        {
            await AddExistingActionAsync();
            string[] args = new string[] { ClientProjectName, _clientProjectFile, "9.9" };

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

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

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());
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

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<AlarmContext>, CreateTextSupplement());

            Assert.AreEqual((int)ClientDataExitCode.InvalidArguments, exitCode);
        }


        /// <summary>
        /// Gets the expected shared actions of a configuration: raise and clear, and acknowledge if it is acknowledgeable.
        /// </summary>
        /// <param name="config">The imported configuration.</param>
        /// <returns>Names and triggers of the expected actions.</returns>
        private List<(string, AlarmActionTrigger)> GetExpectedActions(PpcAlarmConfig config)
        {
            List<(string, AlarmActionTrigger)> actions = new List<(string, AlarmActionTrigger)>
            {
                ("RaiseAlarm", AlarmActionTrigger.Raise),
                ("ClearAlarm", AlarmActionTrigger.Normalize)
            };

            if (config.IsAcknowledgeable)
            {
                actions.Add(("AcknowledgeAlarm", AlarmActionTrigger.Acknowledge));
            }

            return actions;
        }


        /// <summary>
        /// Creates the supplement of the localized texts as the alarm client data program does.
        /// </summary>
        /// <returns>The supplement that imports the text files of the client project file.</returns>
        private LocalizedTextSupplement<LocalizedText> CreateTextSupplement()
        {
            return new LocalizedTextSupplement<LocalizedText>(text => text.Name, text => text.Language,
                entity => entity is AlarmConfigBase config ? config.GetLocalizableValues() : [], LocalizedText.DefaultLanguage);
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

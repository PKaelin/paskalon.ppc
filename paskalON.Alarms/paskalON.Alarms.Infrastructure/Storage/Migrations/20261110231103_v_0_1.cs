using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace paskalON.Alarms.Infrastructure.Storage.Migrations
{
    /// <inheritdoc />
    public partial class v_0_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:case_insensitive_collation", "und-u-ks-level2,und-u-ks-level2,icu,False");

            migrationBuilder.CreateSequence(
                name: "DomainBaseSequence");

            migrationBuilder.CreateTable(
                name: "AlarmActionConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Trigger = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmActionConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Key = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizedText",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedText", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PpcAlarmConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AlarmId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SeverityLevel = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PossibleCauses = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Response = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Consequences = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsAcknowledgeable = table.Column<bool>(type: "boolean", nullable: false),
                    TimeBeforeRaisingAlarmMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    TimeBeforeNormalizingAlarmMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    PpcAlarmType = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: true),
                    PropertyName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PropertyOperator = table.Column<int>(type: "integer", nullable: false),
                    PropertyValue = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PpcAlarmConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AlarmActionSendIntervalMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    AlarmActionRetryIntervalMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    AlarmDelayCheckIntervalMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    PublisherExchangeAlarmActions = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicPcsDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicPcsCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicBatteryBankCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicBatteryBankDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicSolarPanelCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicSolarPanelDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicExternalPowerMeterCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicExternalPowerMeterDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicAuxiliaryPowerMeterCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicAuxiliaryPowerMeterDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicCircuitPowerMeterCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicCircuitPowerMeterDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicSystemPowerMeterCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicSystemPowerMeterDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicPowerTarget = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicCurrentSetpoint = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlarmActionParameterConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AlarmActionConfigId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmActionParameterConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlarmActionParameterConfig_AlarmActionConfig_AlarmActionCon~",
                        column: x => x.AlarmActionConfigId,
                        principalTable: "AlarmActionConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlarmConfigAction",
                columns: table => new
                {
                    AlarmActionConfigId = table.Column<int>(type: "integer", nullable: false),
                    AlarmConfigBaseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmConfigAction", x => new { x.AlarmActionConfigId, x.AlarmConfigBaseId });
                    table.ForeignKey(
                        name: "FK_AlarmConfigAction_AlarmActionConfig_AlarmActionConfigId",
                        column: x => x.AlarmActionConfigId,
                        principalTable: "AlarmActionConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PpcAlarm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AlarmSequenceState = table.Column<int>(type: "integer", nullable: false),
                    AcknowledgedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AcknowledgedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeviceId = table.Column<int>(type: "integer", nullable: true),
                    DeviceName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PropertyName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PpcAlarmConfigId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PpcAlarm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PpcAlarm_PpcAlarmConfig_PpcAlarmConfigId",
                        column: x => x.PpcAlarmConfigId,
                        principalTable: "PpcAlarmConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlarmActionOutboxMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    PpcAlarmId = table.Column<int>(type: "integer", nullable: false),
                    ActionName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SentOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailedAttempts = table.Column<int>(type: "integer", nullable: false),
                    LastError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmActionOutboxMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlarmActionOutboxMessage_PpcAlarm_PpcAlarmId",
                        column: x => x.PpcAlarmId,
                        principalTable: "PpcAlarm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlarmActionOutboxMessage_MessageId",
                table: "AlarmActionOutboxMessage",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlarmActionOutboxMessage_Pending",
                table: "AlarmActionOutboxMessage",
                column: "CreatedOn",
                filter: "\"SentOn\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AlarmActionOutboxMessage_PpcAlarmId",
                table: "AlarmActionOutboxMessage",
                column: "PpcAlarmId");

            migrationBuilder.CreateIndex(
                name: "IX_AlarmActionParameterConfig_AlarmActionConfigId_Key",
                table: "AlarmActionParameterConfig",
                columns: new[] { "AlarmActionConfigId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlarmConfigAction_AlarmConfigBaseId",
                table: "AlarmConfigAction",
                column: "AlarmConfigBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_Key",
                table: "Configuration",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedText_Name_Language",
                table: "LocalizedText",
                columns: new[] { "Name", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PpcAlarm_Active",
                table: "PpcAlarm",
                columns: new[] { "DeviceId", "PropertyName", "PpcAlarmConfigId" },
                unique: true,
                filter: "\"AlarmSequenceState\" <> 2")
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "IX_PpcAlarm_AlarmSequenceState",
                table: "PpcAlarm",
                column: "AlarmSequenceState");

            migrationBuilder.CreateIndex(
                name: "IX_PpcAlarm_PpcAlarmConfigId",
                table: "PpcAlarm",
                column: "PpcAlarmConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PpcAlarmConfig_PpcAlarmType_DeviceId_PropertyName",
                table: "PpcAlarmConfig",
                columns: new[] { "PpcAlarmType", "DeviceId", "PropertyName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlarmActionOutboxMessage");

            migrationBuilder.DropTable(
                name: "AlarmActionParameterConfig");

            migrationBuilder.DropTable(
                name: "AlarmConfigAction");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "LocalizedText");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "PpcAlarm");

            migrationBuilder.DropTable(
                name: "AlarmActionConfig");

            migrationBuilder.DropTable(
                name: "PpcAlarmConfig");

            migrationBuilder.DropSequence(
                name: "DomainBaseSequence");
        }
    }
}

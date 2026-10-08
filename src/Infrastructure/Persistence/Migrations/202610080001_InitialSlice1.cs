using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameNet.Infrastructure.Persistence.Migrations;

public partial class InitialSlice1 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "gamenet");

        migrationBuilder.CreateTable(
            name: "Operators",
            schema: "gamenet",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Operators", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Stations",
            schema: "gamenet",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Number = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Lifecycle = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Stations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AgentDevices",
            schema: "gamenet",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DeviceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                CredentialHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                PairingCodeHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                PairingExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                State = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                StationId = table.Column<Guid>(type: "uuid", nullable: true),
                ConnectionId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                LeaseVersion = table.Column<long>(type: "bigint", nullable: false),
                LastSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AgentDevices", x => x.Id);
                table.ForeignKey(
                    name: "FK_AgentDevices_Stations_StationId",
                    column: x => x.StationId,
                    principalSchema: "gamenet",
                    principalTable: "Stations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "OperatorSessions",
            schema: "gamenet",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OperatorSessions", x => x.Id);
                table.ForeignKey(
                    name: "FK_OperatorSessions_Operators_OperatorId",
                    column: x => x.OperatorId,
                    principalSchema: "gamenet",
                    principalTable: "Operators",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AgentDevices_CredentialHash",
            schema: "gamenet",
            table: "AgentDevices",
            column: "CredentialHash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AgentDevices_DeviceId",
            schema: "gamenet",
            table: "AgentDevices",
            column: "DeviceId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AgentDevices_StationId",
            schema: "gamenet",
            table: "AgentDevices",
            column: "StationId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Operators_UserName",
            schema: "gamenet",
            table: "Operators",
            column: "UserName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OperatorSessions_OperatorId",
            schema: "gamenet",
            table: "OperatorSessions",
            column: "OperatorId");

        migrationBuilder.CreateIndex(
            name: "IX_OperatorSessions_TokenHash",
            schema: "gamenet",
            table: "OperatorSessions",
            column: "TokenHash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Stations_AgentDeviceId",
            schema: "gamenet",
            table: "Stations",
            column: "AgentDeviceId",
            unique: true);
        
        migrationBuilder.CreateIndex(
            name: "IX_Stations_Number",
            schema: "gamenet",
            table: "Stations",
            column: "Number",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OperatorSessions",
            schema: "gamenet");

        migrationBuilder.DropTable(
            name: "AgentDevices",
            schema: "gamenet");

        migrationBuilder.DropTable(
            name: "Operators",
            schema: "gamenet");

        migrationBuilder.DropTable(
            name: "Stations",
            schema: "gamenet");
    }
}

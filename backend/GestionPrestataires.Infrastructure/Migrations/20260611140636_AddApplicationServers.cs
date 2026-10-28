using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationServers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_servers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    identifiant = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    libelle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ordre = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_servers", x => x.id);
                    table.ForeignKey(
                        name: "FK_application_servers_applications_planning_application_id",
                        column: x => x.application_id,
                        principalTable: "applications_planning",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6441), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6443) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6446), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6447) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6450), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6450) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6455), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6455) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6458), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6459) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6453), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6453) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6461), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6461) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6464), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6465) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$Wc8gkO0L3qDlzAzs1vF.kOdsxA7FPpqXqdOmF49N7enKpw6XML8aK");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$89pGC8XJBt7kGE3y371IcehjQOEt5no6kfHS5X9W2rek2A5FzF/Ja");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$IaUYuE48/nkEKrTyx8r.AOnaVNZ8T.rrz872m8qd/mJ2J54LRyNze");

            migrationBuilder.CreateIndex(
                name: "ix_application_servers_application_id",
                table: "application_servers",
                column: "application_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_servers");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1349), new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1354) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1358), new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1359) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1362), new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1363) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1370), new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1370) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1373), new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1374) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1366), new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1366) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1377), new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1377) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1380), new DateTime(2026, 6, 11, 13, 26, 17, 748, DateTimeKind.Utc).AddTicks(1381) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$LxMO/2cM380eSx/HJz4e0e86QVxCkruSiTpOzE0yjPUGm5cCrLjoq");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$eA8jOMaIjpv3OC/1PEgex.noo3vT2ijJJpyXEma6I5Z0Yzg9QNwHO");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$R4k1A3NPksiF4R.CkXazkOXOvzj4InG5XyaVjS.9EHNrIRA.P2g.W");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeactivationFieldsToPrestataire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deactivated_at",
                table: "prestataires",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deactivation_reason",
                table: "prestataires",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deactivated_at",
                table: "prestataires");

            migrationBuilder.DropColumn(
                name: "deactivation_reason",
                table: "prestataires");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1804), new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1811) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1818), new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1818) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1823), new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1824) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1833), new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1834) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1838), new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1839) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1828), new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1829) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1842), new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1843) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1846), new DateTime(2026, 6, 11, 13, 4, 0, 292, DateTimeKind.Utc).AddTicks(1847) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$4dMPQ0OsjYWLGPvF5RsQfOXQTguK1yzcZqbcHonkQTpOxqlrXpdTS");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$iHZY4UqV.MW08rKtpeSv7.EMvCFF9oBuOAxOSUmNAIO4lXQ97lVvC");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$1laGO4/Ok.8Ul9.x1pmR8OSqWXof/HnHjhfvY8OeOnlMJuEgL.RAO");
        }
    }
}

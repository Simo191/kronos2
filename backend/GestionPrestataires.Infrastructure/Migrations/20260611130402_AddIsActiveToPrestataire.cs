using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToPrestataire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "prestataires",
                type: "boolean",
                nullable: false,
                defaultValue: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "prestataires");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(588), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(592) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(598), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(599) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(603), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(603) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(613), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(614) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(618), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(619) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(608), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(609) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(623), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(624) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(627), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(628) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$bouQXMMYsBguxND5G4brY.OoPW1Y8ryJMRlSHxgyu/k3j3dJ6hyTG");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$Igebjfxygke3zzyjDH2aveWS60rwHFtRlLmHOgc3.XQzmeKXiXmR6");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$dqkFlthEvjKf0hhAwXiqPuLrAHSk.LrS4eo7PKKttrCh7Dq8MB9jK");
        }
    }
}

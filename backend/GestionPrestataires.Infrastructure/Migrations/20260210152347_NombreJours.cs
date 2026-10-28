using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NombreJours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NombreJours",
                table: "shifts",
                type: "numeric",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7186), new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7189) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7194), new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7194) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7198), new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7199) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7208), new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7208) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7212), new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7213) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7203), new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7204) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7216), new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7217) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7220), new DateTime(2026, 2, 10, 15, 23, 45, 723, DateTimeKind.Utc).AddTicks(7221) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$9vPKrrLtFe8AQwzGmeV13u6mEQqW7gcnX02cUxDLBUiSVrTiau5uO");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$6YfECdecSzXkCriyMVPMoOclI5/v8f9wXeC8OAyJi9EeNttkbcbvC");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$0eXn60wLgaxYbjk3v8yvi.eWen9VZBSAb0YBJyOo78u6O9VfV4VFu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreJours",
                table: "shifts");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4918), new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4920) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4925), new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4926) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4930), new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4931) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4939), new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4939) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4944), new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4944) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4934), new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4935) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4948), new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4949) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4952), new DateTime(2026, 2, 10, 13, 43, 38, 358, DateTimeKind.Utc).AddTicks(4953) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$8fxrRpkH16WCNVwcZWL6Qe4bkPgmZLelNvEeCEH2ivbJxpqxTVsf.");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$I8dpQCiBW8aOk.nlSsyN1.9bVuIytd4WvShrC90PQAAMCPVOkTvnq");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$kIjADBRWgdLyfuvBpLK3LuqcgufN/naX4SC1L7iKHeww3Zy7GsCOG");
        }
    }
}

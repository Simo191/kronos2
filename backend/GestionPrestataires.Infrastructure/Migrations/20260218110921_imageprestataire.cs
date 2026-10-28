using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class imageprestataire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "prestataires",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9665), new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9671) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9679), new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9679) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9685), new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9686) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9696), new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9696) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9702), new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9702) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9690), new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9691) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9706), new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9707) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9711), new DateTime(2026, 2, 18, 11, 9, 19, 763, DateTimeKind.Utc).AddTicks(9712) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$skJuwD3hWLiik0S.h.YRbeSnwAj58gUOqMilFD4W2K0YBWesWNOzK");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$Jq2eTfYpXj.0yiTFhu8bKutveT4eSTrq6E9YbHpC05eMph4iZwlY.");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$3vp5o.9mupnBCMu408kDhuF//6VyBYfqvY6LuE6mzb.O4io7.zEeK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "prestataires");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7057), new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7060) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7064), new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7065) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7069), new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7069) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7077), new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7078) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7082), new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7082) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7073), new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7074) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7086), new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7086) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7090), new DateTime(2026, 2, 18, 9, 51, 15, 874, DateTimeKind.Utc).AddTicks(7091) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$.E71OSqz7YgrKx8CJVbeCeGnB4N/bL6qNB88BfiMEtyYKwjc0zhku");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$GHPfU5djRlM2PTAhn7ekx..piSTkw0OOY/gwmiiNKIxwhp5y/XXLy");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$GBkWcYtTeKDApZ1fPBai1OueVWOWrbfyrsCi8f3RtoqVs8rlgpT0G");
        }
    }
}

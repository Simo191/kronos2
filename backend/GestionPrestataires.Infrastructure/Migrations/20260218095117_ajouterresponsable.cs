using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ajouterresponsable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "prestataires",
                type: "integer",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "responsable",
                table: "prestataires",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "active",
                table: "contrats",
                type: "boolean",
                nullable: false,
                defaultValue: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order",
                table: "prestataires");

            migrationBuilder.DropColumn(
                name: "responsable",
                table: "prestataires");

            migrationBuilder.DropColumn(
                name: "active",
                table: "contrats");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3951), new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3954) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3959), new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3959) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3963), new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3963) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3970), new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3971) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3974), new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3974) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3967), new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3967) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3977), new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3978) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3981), new DateTime(2026, 2, 12, 9, 54, 26, 814, DateTimeKind.Utc).AddTicks(3981) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$yAUqtKSULH4CSgkJbk1PC.j9JneZJGaPaldyFw.UmewpcWbv9gUQS");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$QFoVFzwcc8QIbashoOgsAukxakzot3gNcLfbRL.vvL7BK9P1vksN.");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$vXOFNtZHLbL0dzQqM/J7JuDU7lQrJ09AWsRRgp0f8vjs4.VeSeTo.");
        }
    }
}

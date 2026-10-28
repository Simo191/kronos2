using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class uploaddoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentUrl",
                table: "contrats",
                type: "text",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentUrl",
                table: "contrats");

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
    }
}

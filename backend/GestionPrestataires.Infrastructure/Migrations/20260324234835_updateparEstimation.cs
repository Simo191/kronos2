using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateparEstimation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "modifie_par",
                table: "estimations",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8612), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8615) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8620), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8621) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8624), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8625) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8633), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8633) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8636), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8637) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8629), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8629) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8640), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8641) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8644), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8645) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$Tc38d54EQkGPxXldb2Cp3e.WsVKx8gmh6JFyn1daxn8vNi/yfCfm.");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$3CZBBGypEGtzDnHNvfMyFebad3HiSNAMlThof6YldLVUT8KBrEFBu");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$JBIHaE3ygLTU3XBHhfgbheJx0oyrYw3LQG6n9t8yr678OkRrvecVK");

            migrationBuilder.CreateIndex(
                name: "IX_estimations_modifie_par",
                table: "estimations",
                column: "modifie_par");

            migrationBuilder.AddForeignKey(
                name: "FK_estimations_users_modifie_par",
                table: "estimations",
                column: "modifie_par",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_estimations_users_modifie_par",
                table: "estimations");

            migrationBuilder.DropIndex(
                name: "IX_estimations_modifie_par",
                table: "estimations");

            migrationBuilder.DropColumn(
                name: "modifie_par",
                table: "estimations");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3331), new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3334) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3339), new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3340) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3344), new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3344) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3355), new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3355) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3359), new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3360) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3348), new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3348) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3364), new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3364) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3368), new DateTime(2026, 3, 9, 15, 27, 56, 647, DateTimeKind.Utc).AddTicks(3369) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$5R0yShv64vd2DdGnF1yQBO2KP5r6J2Zx8KACMQssOqB9VR6EHaEKW");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$IEt6G02p.sWljqyvPE4xueYckZOANHhRZCZRcRpuARpKYQ7y4bjN2");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$/9UYxBFeMnrbPwSrv5s1CuRrTlBcIY5qiTUdr75K2X9O/3uPKdTFe");
        }
    }
}

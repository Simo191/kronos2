using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeutc01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "datetravail",
                table: "jours_travail",
                newName: "DateTravail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTravail",
                table: "jours_travail",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$Piak7iuDlc8Y3LqMrT/v9Okvni7iLNkiPZKKRK.OgwUMBnxEEp.yS");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$nEFphzUC4hwLKTmS51v.1uwOry/0kYG7k8sPPwDfWeVYC0AXMtGDu");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$AkXGkeYw4EHz3BG9zcxtFeg9.4m.V1z.fV1qOFKThqJ1K4O59HMR.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTravail",
                table: "jours_travail",
                newName: "datetravail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "datetravail",
                table: "jours_travail",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$k81eTOJoPZA4NOccQBGUpuLtQkZbcxv2REXHK3D9Ubqi9PWaQwLzm");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$a0ry7JJUeuAIToNyBk3A9.buZyP4UnXz.R.hVuif7FsrB95GQRkuC");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$nS0ChkUqXqGT8d5j6vdmaeQ02y5xVY5Vfdksbp5ICt0nl5m2c8cUe");
        }
    }
}

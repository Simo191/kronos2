using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changementheursparmois : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "commentaire",
                table: "jours_travail");

            migrationBuilder.DropColumn(
                name: "duree",
                table: "jours_travail");

            migrationBuilder.RenameColumn(
                name: "date_travail",
                table: "jours_travail",
                newName: "DateTravail");

            migrationBuilder.RenameColumn(
                name: "projet",
                table: "jours_travail",
                newName: "Description");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTravail",
                table: "jours_travail",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Mois",
                table: "jours_travail",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "NombreJours",
                table: "jours_travail",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$QHM.ubsX7pNcGI/sWzUgXe23f/7UjSvXoVGsG9AnVKOu.7EJOtCK.");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$JDKc1xLXCBpBtZdEXmOftu7y8l3zAx/jaNcjXhSWoA6bAM2ePk1mK");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$e0lut0ME86N/Z51hmd502utY.lMO5UBGbADXZ7baGmo3TwI7Noiq.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mois",
                table: "jours_travail");

            migrationBuilder.DropColumn(
                name: "NombreJours",
                table: "jours_travail");

            migrationBuilder.RenameColumn(
                name: "DateTravail",
                table: "jours_travail",
                newName: "date_travail");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "jours_travail",
                newName: "projet");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_travail",
                table: "jours_travail",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "commentaire",
                table: "jours_travail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "duree",
                table: "jours_travail",
                type: "numeric(4,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$Xoktx588UIWIOgtSpXPtzOh9XKJsxNDTiygeQYShtmFOyNswExhEe");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$ME8JfYBtZEesn51bGIGmfOkOzSlwcTEhGicuzB17CD6IM4Xscb5Di");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$koFpsr.weKVwbNSGoHXivex/S2mDu8RBgqEuV5jnLqjN/QAnj6UQe");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTimeUTC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTravail",
                table: "jours_travail",
                newName: "datetravail");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "datetravail",
                table: "jours_travail",
                newName: "DateTravail");

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
    }
}

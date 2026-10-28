using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class data_inital2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$NwSjSND04./RMfBwLXR8NuOtqwJpImte16gZZiyUoX5O.0W8hx4kW");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$y3NHqSYOFlRL4A6dBiq5mO4txR.Ng55cb7.Mt2Ve5NCCZKwVa7YF6");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$H.7E2ZAF1e8DFujCMIRmXuF4iXIyer3lw/0BDF1TnQB26YNe0GEJy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$nXNiQGizQpjQnQeT9.49iuxskWsSNClmWukCckyVjKkhoaGzo5hAG");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$CV.M3DnGKVJmpPjKFJE2ZeI86DKlHm3f.k06D1R0XVXJqQvWeyK5q");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$G2lpWx91HPYlbnXWFhLFAuCULjQI8A93.ygGLHnwkQgi2QRqsIgIm");
        }
    }
}

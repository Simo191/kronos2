using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class data_inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "created_at", "email", "first_name", "is_active", "last_login_at", "last_name", "password_hash", "role_id" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@LeaStudio.com", "System", true, null, "Administrateur", "$2a$11$nXNiQGizQpjQnQeT9.49iuxskWsSNClmWukCckyVjKkhoaGzo5hAG", new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "razqani@gmail.com", "System", true, null, "Manager", "$2a$11$CV.M3DnGKVJmpPjKFJE2ZeI86DKlHm3f.k06D1R0XVXJqQvWeyK5q", new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "rs.razqani@gmail.com", "System", true, null, "User", "$2a$11$G2lpWx91HPYlbnXWFhLFAuCULjQI8A93.ygGLHnwkQgi2QRqsIgIm", new Guid("11111111-1111-1111-1111-111111111113") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
        }
    }
}

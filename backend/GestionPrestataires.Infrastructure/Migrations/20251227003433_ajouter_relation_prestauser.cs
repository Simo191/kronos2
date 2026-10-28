using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ajouter_relation_prestauser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "prestataires",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(5995), new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(5998) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6001), new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6002) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6004), new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6005) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6010), new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6011) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6014), new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6014) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6007), new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6008) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6017), new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6017) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6020), new DateTime(2025, 12, 27, 0, 34, 31, 995, DateTimeKind.Utc).AddTicks(6020) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$xl1qYOQD1C4U0wBDU5QAyOgMxL8p2Ez/l81gJH1oFzQS6HZ1Monr6");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$G5mA5I.PmBWU50S7V1WhvODST09hFKBapGFs8fB59xmFgyGxPh15m");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$WXG0DOPo6xGPSBZnLDI9suCWbGCjpOf.kxMpv1gbAQe4zDEAIwN0O");

            migrationBuilder.CreateIndex(
                name: "IX_prestataires_user_id",
                table: "prestataires",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_prestataires_users_user_id",
                table: "prestataires",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prestataires_users_user_id",
                table: "prestataires");

            migrationBuilder.DropIndex(
                name: "IX_prestataires_user_id",
                table: "prestataires");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "prestataires");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5155), new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5158) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5164), new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5164) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5168), new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5169) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5178), new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5178) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5182), new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5182) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5173), new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5173) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5186), new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5187) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5190), new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5191) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$ylv07E4RtodcXBdFrD5T/OP.guw5sg2PCxcUBCNv1aMSBXNigzUAu");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$gGkwSvfxt/z6aXaevfZtK.JgApl4acpNZQ8XkIwAhfWnZ2MtMxBgG");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$SNTUoh1uPXJhl33cQ5lJ7Os/JCmFjfBny9TI7.F.7N3X0mPpXgZ9y");
        }
    }
}

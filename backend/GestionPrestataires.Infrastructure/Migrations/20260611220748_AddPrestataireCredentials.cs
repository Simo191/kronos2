using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrestataireCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prestataire_credentials",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    prestataire_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    account_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    encrypted_password = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prestataire_credentials", x => x.id);
                    table.ForeignKey(
                        name: "FK_prestataire_credentials_prestataires_prestataire_id",
                        column: x => x.prestataire_id,
                        principalTable: "prestataires",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7177), new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7180) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7185), new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7185) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7188), new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7189) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7195), new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7196) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7198), new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7199) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7192), new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7192) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7205), new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7206) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7208), new DateTime(2026, 6, 11, 22, 7, 47, 495, DateTimeKind.Utc).AddTicks(7208) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$IK.mwE21swYPHVhMKvySGOz8lcbECdbyfSr91wHJ94T02bu7lw1/i");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$ARZMMIdtuKjFintyZONEyeSjwkDo2yAwFh.rtDikxDB0FqrHL8RGa");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$Vm8AbpGLckw1U2PEIOPfoe7UbuTvrnSIt7n2S71WH0D0mh15rJvgC");

            migrationBuilder.CreateIndex(
                name: "ix_prestataire_credentials_prestataire_id",
                table: "prestataire_credentials",
                column: "prestataire_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prestataire_credentials");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6441), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6443) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6446), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6447) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6450), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6450) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6455), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6455) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6458), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6459) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6453), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6453) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6461), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6461) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6464), new DateTime(2026, 6, 11, 14, 6, 34, 971, DateTimeKind.Utc).AddTicks(6465) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$Wc8gkO0L3qDlzAzs1vF.kOdsxA7FPpqXqdOmF49N7enKpw6XML8aK");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$89pGC8XJBt7kGE3y371IcehjQOEt5no6kfHS5X9W2rek2A5FzF/Ja");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$IaUYuE48/nkEKrTyx8r.AOnaVNZ8T.rrz872m8qd/mJ2J54LRyNze");
        }
    }
}

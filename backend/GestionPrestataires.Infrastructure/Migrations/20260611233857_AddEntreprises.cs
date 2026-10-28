using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEntreprises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "entreprise_id",
                table: "prestataires",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "entreprises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entreprises", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4187), new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4192) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4197), new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4197) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4200), new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4200) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4206), new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4206) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4208), new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4209) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4203), new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4203) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4211), new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4212) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4214), new DateTime(2026, 6, 11, 23, 38, 56, 286, DateTimeKind.Utc).AddTicks(4215) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$zlCuv6SirZO4sbnhR.Flj.CP5roSE1AnZLWLkHEYn7Mz6Exl4y.d2");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$yS1.Umx6X3qcDSrq88ZrcunUDqrYrm0Xfz1yVOU1mCbqeGldGZX12");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$ycU.HN6hhr41vHC5hCFFBuF8h7KKLxiC9FVDvlcMdunMXp4oTUI6.");

            migrationBuilder.CreateIndex(
                name: "IX_prestataires_entreprise_id",
                table: "prestataires",
                column: "entreprise_id");

            migrationBuilder.CreateIndex(
                name: "ix_entreprises_nom",
                table: "entreprises",
                column: "nom");

            migrationBuilder.AddForeignKey(
                name: "FK_prestataires_entreprises_entreprise_id",
                table: "prestataires",
                column: "entreprise_id",
                principalTable: "entreprises",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prestataires_entreprises_entreprise_id",
                table: "prestataires");

            migrationBuilder.DropTable(
                name: "entreprises");

            migrationBuilder.DropIndex(
                name: "IX_prestataires_entreprise_id",
                table: "prestataires");

            migrationBuilder.DropColumn(
                name: "entreprise_id",
                table: "prestataires");

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
        }
    }
}

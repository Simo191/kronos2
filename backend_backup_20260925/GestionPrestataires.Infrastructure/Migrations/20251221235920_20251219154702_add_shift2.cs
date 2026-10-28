using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20251219154702_add_shift2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "type_shifts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    short_label = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    bg_color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    border_color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    text_color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    order = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "0=Travail, 1=Absence, 2=Congé"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_shifts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shifts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    prestataire_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    type_shift_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shifts", x => x.id);
                    table.ForeignKey(
                        name: "fk_shifts_prestataire_id",
                        column: x => x.prestataire_id,
                        principalTable: "prestataires",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shifts_type_shift_id",
                        column: x => x.type_shift_id,
                        principalTable: "type_shifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "type_shifts",
                columns: new[] { "id", "bg_color", "border_color", "code", "color", "created_at", "is_active", "label", "order", "short_label", "text_color", "updated_at" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "bg-green-200", "border-green-400", "SUR_SITE", "#4ade80", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5155), true, "Sur Site", 1, "Sur Site", "text-green-900", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5158) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "bg-yellow-200", "border-yellow-400", "TELETRAVAIL", "#facc15", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5164), true, "Télétravail", 2, "Télétrav.", "text-yellow-900", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5164) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "bg-blue-200", "border-blue-400", "FORMATION", "#60a5fa", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5168), true, "Formation", 3, "Formatio.", "text-blue-900", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5169) }
                });

            migrationBuilder.InsertData(
                table: "type_shifts",
                columns: new[] { "id", "bg_color", "border_color", "category", "code", "color", "created_at", "is_active", "label", "order", "short_label", "text_color", "updated_at" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), "bg-gray-200", "border-gray-400", 1, "ABSENCE", "#9ca3af", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5178), true, "Absence", 5, "Absence", "text-gray-900", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5178) },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "bg-red-200", "border-red-400", 2, "CONGE", "#f87171", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5182), true, "Congé", 6, "Congé", "text-red-900", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5182) }
                });

            migrationBuilder.InsertData(
                table: "type_shifts",
                columns: new[] { "id", "bg_color", "border_color", "code", "color", "created_at", "is_active", "label", "order", "short_label", "text_color", "updated_at" },
                values: new object[] { new Guid("77777777-7777-7777-7777-777777777777"), "bg-purple-200", "border-purple-400", "DEPLACEMENT", "#c084fc", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5173), true, "Déplacement", 4, "Déplacmt.", "text-purple-900", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5173) });

            migrationBuilder.InsertData(
                table: "type_shifts",
                columns: new[] { "id", "bg_color", "border_color", "category", "code", "color", "created_at", "is_active", "label", "order", "short_label", "text_color", "updated_at" },
                values: new object[] { new Guid("88888888-8888-8888-8888-888888888888"), "bg-orange-200", "border-orange-400", 2, "ECOLE", "#fb923c", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5186), true, "École", 7, "École", "text-orange-900", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5187) });

            migrationBuilder.InsertData(
                table: "type_shifts",
                columns: new[] { "id", "bg_color", "border_color", "code", "color", "created_at", "is_active", "label", "order", "short_label", "text_color", "updated_at" },
                values: new object[] { new Guid("99999999-9999-9999-9999-999999999999"), "bg-emerald-700", "border-emerald-800", "SEMINAIRE", "#047857", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5190), true, "Séminaire", 8, "Séminair.", "text-white", new DateTime(2025, 12, 21, 23, 59, 18, 755, DateTimeKind.Utc).AddTicks(5191) });

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

            migrationBuilder.CreateIndex(
                name: "ix_shifts_date",
                table: "shifts",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "ix_shifts_prestataire_id_date",
                table: "shifts",
                columns: new[] { "prestataire_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_shifts_type_shift_id",
                table: "shifts",
                column: "type_shift_id");

            migrationBuilder.CreateIndex(
                name: "ix_type_shifts_category",
                table: "type_shifts",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_type_shifts_code",
                table: "type_shifts",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shifts");

            migrationBuilder.DropTable(
                name: "type_shifts");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$GU.lrABZVMZtOdVYHpxxi.20zffycsFmJW.Moc/zJSd2LfquaiK.a");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$5FIHRQqgIg/2c4Dwg2X5reNfUn8twAeplQd9gVJYv9yH8AjkpT2c6");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$SRx1DEIdoyFrsTjiBlqL4uDeUcUQ.BKxltR.ORtVXk8Aw7tN2IaSe");
        }
    }
}

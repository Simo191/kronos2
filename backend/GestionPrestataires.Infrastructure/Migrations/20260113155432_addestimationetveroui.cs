using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addestimationetveroui : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "estimations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    prestataire_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mois = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    nombre_jours = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    tjm = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    date_estimation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrestataireId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimations", x => x.id);
                    table.ForeignKey(
                        name: "FK_estimations_prestataires_PrestataireId1",
                        column: x => x.PrestataireId1,
                        principalTable: "prestataires",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_estimations_prestataires_prestataire_id",
                        column: x => x.prestataire_id,
                        principalTable: "prestataires",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mois_verrouilles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    mois = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    verrouille_par_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_verrouillage = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    commentaire = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mois_verrouilles", x => x.id);
                    table.ForeignKey(
                        name: "FK_mois_verrouilles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_mois_verrouilles_users_verrouille_par_user_id",
                        column: x => x.verrouille_par_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4938), new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4943) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4949), new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4950) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4955), new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4956) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4967), new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4968) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4973), new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4973) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4961), new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4962) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4978), new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4979) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4983), new DateTime(2026, 1, 13, 15, 54, 30, 322, DateTimeKind.Utc).AddTicks(4984) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$YeGvKDznpmwfo03Cx.QQUOmGa4JPKgvi6yAWTjF9cFZRjHrrfe8Um");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$94TYgcS5YjP4ddU9fobOc.7ocPnmJt3qJ6zLoMPMWIvin9iWhG/cS");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$jhZxrOe7IqXfTfIEylh7xeSptKQHWjUEzciQL3WaLUs4W1HOEqa4q");

            migrationBuilder.CreateIndex(
                name: "IX_estimations_prestataire_id_mois",
                table: "estimations",
                columns: new[] { "prestataire_id", "mois" });

            migrationBuilder.CreateIndex(
                name: "IX_estimations_PrestataireId1",
                table: "estimations",
                column: "PrestataireId1");

            migrationBuilder.CreateIndex(
                name: "IX_mois_verrouilles_mois",
                table: "mois_verrouilles",
                column: "mois",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mois_verrouilles_UserId",
                table: "mois_verrouilles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_mois_verrouilles_verrouille_par_user_id",
                table: "mois_verrouilles",
                column: "verrouille_par_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "estimations");

            migrationBuilder.DropTable(
                name: "mois_verrouilles");

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
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addgetionplanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "applications_planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    date_debut = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    statut = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    priorite = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    couleur = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    avancement = table.Column<int>(type: "integer", nullable: false),
                    date_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modification = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications_planning", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "application_prestataire_assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prestataire_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_prestataire_assignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_application_prestataire_assignments_applications_planning_a~",
                        column: x => x.application_id,
                        principalTable: "applications_planning",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_application_prestataire_assignments_prestataires_prestatair~",
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
                values: new object[] { new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8918), new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8922) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8928), new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8929) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8933), new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8934) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8942), new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8942) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8946), new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8947) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8937), new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8938) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8951), new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8952) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8955), new DateTime(2026, 3, 25, 0, 11, 33, 480, DateTimeKind.Utc).AddTicks(8956) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$/lnveOgUw/Iji3EyCwGIPuw/C7vt/RJXYp.RaWTrr2AXwkhUYtyFO");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$xgKG51q4k.QqfRLe2Fwa5O.ktAk44c9iXJzJmRgh8izs30RXisdWC");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$.s.ZjOvZ3EaH1okH4U.BnOk9FPdyNAlHw7n6Ts44f2BRZBpgkI1ZC");

            migrationBuilder.CreateIndex(
                name: "IX_application_prestataire_assignments_application_id",
                table: "application_prestataire_assignments",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_prestataire_assignments_prestataire_id",
                table: "application_prestataire_assignments",
                column: "prestataire_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_prestataire_assignments");

            migrationBuilder.DropTable(
                name: "applications_planning");

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8612), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8615) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8620), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8621) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8624), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8625) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8633), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8633) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8636), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8637) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8629), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8629) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8640), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8641) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8644), new DateTime(2026, 3, 24, 23, 48, 34, 487, DateTimeKind.Utc).AddTicks(8645) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$Tc38d54EQkGPxXldb2Cp3e.WsVKx8gmh6JFyn1daxn8vNi/yfCfm.");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$3CZBBGypEGtzDnHNvfMyFebad3HiSNAMlThof6YldLVUT8KBrEFBu");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$JBIHaE3ygLTU3XBHhfgbheJx0oyrYw3LQG6n9t8yr678OkRrvecVK");
        }
    }
}

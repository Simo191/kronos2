using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class creatdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cagnottes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    annee = table.Column<int>(type: "integer", nullable: false),
                    budget_annuel = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    budget_consomme = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    date_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modification = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cagnottes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prestataires",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "text", nullable: false),
                    prenom = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    telephone = table.Column<string>(type: "text", nullable: true),
                    tjm = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prestataires", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    displayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "types_contrat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    libelle = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_types_contrat", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "jours_travail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    prestataire_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_travail = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tjm = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    duree = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    commentaire = table.Column<string>(type: "text", nullable: true),
                    projet = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jours_travail", x => x.id);
                    table.ForeignKey(
                        name: "FK_jours_travail_prestataires_prestataire_id",
                        column: x => x.prestataire_id,
                        principalTable: "prestataires",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contrats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    prestataire_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_contrat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_debut = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    nombre_jours = table.Column<int>(type: "integer", nullable: false),
                    tjm = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    montant_total = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contrats", x => x.id);
                    table.ForeignKey(
                        name: "FK_contrats_prestataires_prestataire_id",
                        column: x => x.prestataire_id,
                        principalTable: "prestataires",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contrats_types_contrat_type_contrat_id",
                        column: x => x.type_contrat_id,
                        principalTable: "types_contrat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "description", "displayName", "name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Full system access", "Administrateur", "Admin" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Management access", "Manager", "Manager" },
                    { new Guid("11111111-1111-1111-1111-111111111113"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Basic user access", "User", "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cagnottes_annee",
                table: "cagnottes",
                column: "annee",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contrats_prestataire_id",
                table: "contrats",
                column: "prestataire_id");

            migrationBuilder.CreateIndex(
                name: "IX_contrats_type_contrat_id",
                table: "contrats",
                column: "type_contrat_id");

            migrationBuilder.CreateIndex(
                name: "IX_jours_travail_prestataire_id",
                table: "jours_travail",
                column: "prestataire_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cagnottes");

            migrationBuilder.DropTable(
                name: "contrats");

            migrationBuilder.DropTable(
                name: "jours_travail");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "types_contrat");

            migrationBuilder.DropTable(
                name: "prestataires");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}

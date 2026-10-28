using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addestimation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_estimations_prestataires_PrestataireId1",
                table: "estimations");

            migrationBuilder.DropForeignKey(
                name: "FK_estimations_prestataires_prestataire_id",
                table: "estimations");

            migrationBuilder.DropIndex(
                name: "IX_estimations_prestataire_id_mois",
                table: "estimations");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "estimations");

            migrationBuilder.DropColumn(
                name: "description",
                table: "estimations");

            migrationBuilder.DropColumn(
                name: "mois",
                table: "estimations");

            migrationBuilder.DropColumn(
                name: "nombre_jours",
                table: "estimations");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "estimations",
                newName: "date_modification");

            migrationBuilder.RenameColumn(
                name: "tjm",
                table: "estimations",
                newName: "budget_consomme");

            migrationBuilder.RenameColumn(
                name: "prestataire_id",
                table: "estimations",
                newName: "cree_par");

            migrationBuilder.RenameColumn(
                name: "date_estimation",
                table: "estimations",
                newName: "date_creation");

            migrationBuilder.RenameColumn(
                name: "PrestataireId1",
                table: "estimations",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_estimations_PrestataireId1",
                table: "estimations",
                newName: "IX_estimations_UserId");

            migrationBuilder.AddColumn<int>(
                name: "annee",
                table: "estimations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "budget_cagnotte",
                table: "estimations",
                type: "numeric(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "lignes_estimation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prestataire_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nom_futur_prestataire = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    mois = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    nombre_jours = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    tjm = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    revenu = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    PrestataireId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lignes_estimation", x => x.id);
                    table.ForeignKey(
                        name: "FK_lignes_estimation_estimations_estimation_id",
                        column: x => x.estimation_id,
                        principalTable: "estimations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lignes_estimation_prestataires_PrestataireId1",
                        column: x => x.PrestataireId1,
                        principalTable: "prestataires",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_lignes_estimation_prestataires_prestataire_id",
                        column: x => x.prestataire_id,
                        principalTable: "prestataires",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2771), new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2774) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2779), new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2780) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2784), new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2784) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2793), new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2794) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2798), new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2799) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2787), new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2788) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2803), new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2803) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2807), new DateTime(2026, 1, 14, 16, 23, 39, 798, DateTimeKind.Utc).AddTicks(2808) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$WGQ4.HNHEZKKZjnvCQRYZOtj7BG5k9w0G0Kn/7IiOfnAyIQv55B8K");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$8JGxy/yPTCC9dKqdeeWvxer2taBiB60nOPyV7E/TTKLgaChc2nRLG");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$DEEtwaZKM2GU/1V/6CAE4e1dySVKRLb1z1QBAWHPPfDEinjBGh5sy");

            migrationBuilder.CreateIndex(
                name: "IX_estimations_annee",
                table: "estimations",
                column: "annee",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimations_cree_par",
                table: "estimations",
                column: "cree_par");

            migrationBuilder.CreateIndex(
                name: "IX_lignes_estimation_estimation_id_mois",
                table: "lignes_estimation",
                columns: new[] { "estimation_id", "mois" });

            migrationBuilder.CreateIndex(
                name: "IX_lignes_estimation_prestataire_id",
                table: "lignes_estimation",
                column: "prestataire_id");

            migrationBuilder.CreateIndex(
                name: "IX_lignes_estimation_PrestataireId1",
                table: "lignes_estimation",
                column: "PrestataireId1");

            migrationBuilder.AddForeignKey(
                name: "FK_estimations_users_UserId",
                table: "estimations",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_estimations_users_cree_par",
                table: "estimations",
                column: "cree_par",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_estimations_users_UserId",
                table: "estimations");

            migrationBuilder.DropForeignKey(
                name: "FK_estimations_users_cree_par",
                table: "estimations");

            migrationBuilder.DropTable(
                name: "lignes_estimation");

            migrationBuilder.DropIndex(
                name: "IX_estimations_annee",
                table: "estimations");

            migrationBuilder.DropIndex(
                name: "IX_estimations_cree_par",
                table: "estimations");

            migrationBuilder.DropColumn(
                name: "annee",
                table: "estimations");

            migrationBuilder.DropColumn(
                name: "budget_cagnotte",
                table: "estimations");

            migrationBuilder.RenameColumn(
                name: "date_modification",
                table: "estimations",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "date_creation",
                table: "estimations",
                newName: "date_estimation");

            migrationBuilder.RenameColumn(
                name: "cree_par",
                table: "estimations",
                newName: "prestataire_id");

            migrationBuilder.RenameColumn(
                name: "budget_consomme",
                table: "estimations",
                newName: "tjm");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "estimations",
                newName: "PrestataireId1");

            migrationBuilder.RenameIndex(
                name: "IX_estimations_UserId",
                table: "estimations",
                newName: "IX_estimations_PrestataireId1");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "estimations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "estimations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mois",
                table: "estimations",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "nombre_jours",
                table: "estimations",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.AddForeignKey(
                name: "FK_estimations_prestataires_PrestataireId1",
                table: "estimations",
                column: "PrestataireId1",
                principalTable: "prestataires",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_estimations_prestataires_prestataire_id",
                table: "estimations",
                column: "prestataire_id",
                principalTable: "prestataires",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

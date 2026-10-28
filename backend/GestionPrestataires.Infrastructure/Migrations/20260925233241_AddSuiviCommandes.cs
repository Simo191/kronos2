using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    //   Suivi des commandes : champs commande sur les contrats + répartition annuelle.
    //   ⚠️ Les UpdateData générés par EF sur type_shifts / users (dates et password_hash
    //   des comptes seedés, non déterministes) ont été retirés volontairement :
    //   ils auraient écrasé les mots de passe existants.
    public partial class AddSuiviCommandes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "application_id",
                table: "contrats",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "commentaire",
                table: "contrats",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "numero_commande",
                table: "contrats",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reference_ctr",
                table: "contrats",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tranche",
                table: "contrats",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Ferme");

            migrationBuilder.CreateTable(
                name: "contrat_repartitions_annuelles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contrat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    annee = table.Column<int>(type: "integer", nullable: false),
                    nombre_jours = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contrat_repartitions_annuelles", x => x.id);
                    table.ForeignKey(
                        name: "FK_contrat_repartitions_annuelles_contrats_contrat_id",
                        column: x => x.contrat_id,
                        principalTable: "contrats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contrats_application_id",
                table: "contrats",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ux_contrat_repartitions_contrat_annee",
                table: "contrat_repartitions_annuelles",
                columns: new[] { "contrat_id", "annee" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_contrats_applications_planning_application_id",
                table: "contrats",
                column: "application_id",
                principalTable: "applications_planning",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contrats_applications_planning_application_id",
                table: "contrats");

            migrationBuilder.DropTable(
                name: "contrat_repartitions_annuelles");

            migrationBuilder.DropIndex(
                name: "ix_contrats_application_id",
                table: "contrats");

            migrationBuilder.DropColumn(
                name: "application_id",
                table: "contrats");

            migrationBuilder.DropColumn(
                name: "commentaire",
                table: "contrats");

            migrationBuilder.DropColumn(
                name: "numero_commande",
                table: "contrats");

            migrationBuilder.DropColumn(
                name: "reference_ctr",
                table: "contrats");

            migrationBuilder.DropColumn(
                name: "tranche",
                table: "contrats");
        }
    }
}

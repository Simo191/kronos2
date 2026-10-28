using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_tablesourcebudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "budget_annuel",
                table: "cagnottes");

            migrationBuilder.CreateTable(
                name: "sources_budget",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cagnotte_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    montant = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    date_creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modification = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sources_budget", x => x.id);
                    table.ForeignKey(
                        name: "FK_sources_budget_cagnottes_cagnotte_id",
                        column: x => x.cagnotte_id,
                        principalTable: "cagnottes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "ix_sources_budget_cagnotte_id",
                table: "sources_budget",
                column: "cagnotte_id");

            migrationBuilder.CreateIndex(
                name: "ix_sources_budget_code",
                table: "sources_budget",
                column: "code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sources_budget");

            migrationBuilder.AddColumn<decimal>(
                name: "budget_annuel",
                table: "cagnottes",
                type: "numeric(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$.WlSU528.WIEgom09tKog.m5GFQowq2H93DfBEeFWNsKzXWjUm.KS");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$uAdl2oWh0.egJ.WB18DfaexSqyBHln2Q2.iZMA7U07AUTO6spnUTa");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$aBiRFqeq7kyIXF713uFV9uBFF7VywCTlbL7XU6PQJIoRDDvggZByK");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPrestataires.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdreToApplicationPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ordre",
                table: "applications_planning",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(588), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(592) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(598), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(599) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(603), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(603) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(613), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(614) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(618), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(619) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(608), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(609) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(623), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(624) });

            migrationBuilder.UpdateData(
                table: "type_shifts",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(627), new DateTime(2026, 3, 25, 2, 7, 16, 84, DateTimeKind.Utc).AddTicks(628) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$11$bouQXMMYsBguxND5G4brY.OoPW1Y8ryJMRlSHxgyu/k3j3dJ6hyTG");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$11$Igebjfxygke3zzyjDH2aveWS60rwHFtRlLmHOgc3.XQzmeKXiXmR6");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$11$dqkFlthEvjKf0hhAwXiqPuLrAHSk.LrS4eo7PKKttrCh7Dq8MB9jK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ordre",
                table: "applications_planning");

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
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CS8981
#pragma warning disable IDE1006

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class naigaiclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "naigai_class",
                table: "kaikei_jisseki",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "naigai_class_master",
                columns: table => new
                {
                    naigai_class = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    naigai_class_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_naigai_class_master", x => x.naigai_class);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kaikei_jisseki_naigai_class",
                table: "kaikei_jisseki",
                column: "naigai_class");

            migrationBuilder.AddForeignKey(
                name: "FK_kaikei_jisseki_naigai_class_master_naigai_class",
                table: "kaikei_jisseki",
                column: "naigai_class",
                principalTable: "naigai_class_master",
                principalColumn: "naigai_class",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_kaikei_jisseki_tento_zaiko_shohin_code",
                table: "kaikei_jisseki",
                column: "shohin_code",
                principalTable: "tento_zaiko",
                principalColumn: "shohin_code",
                onDelete: ReferentialAction.Cascade);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_kaikei_jisseki_naigai_class_master_naigai_class",
                table: "kaikei_jisseki");

            migrationBuilder.DropForeignKey(
                name: "FK_kaikei_jisseki_tento_zaiko_shohin_code",
                table: "kaikei_jisseki");

            migrationBuilder.DropTable(
                name: "naigai_class_master");

            migrationBuilder.DropIndex(
                name: "IX_kaikei_jisseki_naigai_class",
                table: "kaikei_jisseki");

            migrationBuilder.DropColumn(
                name: "naigai_class",
                table: "kaikei_jisseki");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class changeZaiko : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SokoZaikoShiirePrdId",
                table: "soko_zaiko",
                type: "character varying(10)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SokoZaikoShiireSakiId",
                table: "soko_zaiko",
                type: "character varying(10)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SokoZaikoShohinId",
                table: "soko_zaiko",
                type: "character varying(10)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_soko_zaiko_SokoZaikoShiireSakiId_SokoZaikoShiirePrdId_SokoZ~",
                table: "soko_zaiko",
                columns: new[] { "SokoZaikoShiireSakiId", "SokoZaikoShiirePrdId", "SokoZaikoShohinId" });

            migrationBuilder.CreateIndex(
                name: "IX_shiire_jisseki_shiire_saki_code_shiire_prd_code_shohin_code",
                table: "shiire_jisseki",
                columns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" });

            migrationBuilder.AddForeignKey(
                name: "FK_shiire_jisseki_soko_zaiko_shiire_saki_code_shiire_prd_code_~",
                table: "shiire_jisseki",
                columns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" },
                principalTable: "soko_zaiko",
                principalColumns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_soko_zaiko_soko_zaiko_SokoZaikoShiireSakiId_SokoZaikoShiire~",
                table: "soko_zaiko",
                columns: new[] { "SokoZaikoShiireSakiId", "SokoZaikoShiirePrdId", "SokoZaikoShohinId" },
                principalTable: "soko_zaiko",
                principalColumns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shiire_jisseki_soko_zaiko_shiire_saki_code_shiire_prd_code_~",
                table: "shiire_jisseki");

            migrationBuilder.DropForeignKey(
                name: "FK_soko_zaiko_soko_zaiko_SokoZaikoShiireSakiId_SokoZaikoShiire~",
                table: "soko_zaiko");

            migrationBuilder.DropIndex(
                name: "IX_soko_zaiko_SokoZaikoShiireSakiId_SokoZaikoShiirePrdId_SokoZ~",
                table: "soko_zaiko");

            migrationBuilder.DropIndex(
                name: "IX_shiire_jisseki_shiire_saki_code_shiire_prd_code_shohin_code",
                table: "shiire_jisseki");

            migrationBuilder.DropColumn(
                name: "SokoZaikoShiirePrdId",
                table: "soko_zaiko");

            migrationBuilder.DropColumn(
                name: "SokoZaikoShiireSakiId",
                table: "soko_zaiko");

            migrationBuilder.DropColumn(
                name: "SokoZaikoShohinId",
                table: "soko_zaiko");
        }
    }
}

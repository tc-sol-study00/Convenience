using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class NoMapped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shiire_jisseki_soko_zaiko_shiire_saki_code_shiire_prd_code_~",
                table: "shiire_jisseki");

            migrationBuilder.DropIndex(
                name: "IX_shiire_jisseki_shiire_saki_code_shiire_prd_code_shohin_code",
                table: "shiire_jisseki");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}

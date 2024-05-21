using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class foreignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tento_haraidashi_jisseki_shiire_master_ShiireMasterShiireSa~",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.DropIndex(
                name: "IX_tento_haraidashi_jisseki_ShiireMasterShiireSakiId_ShiireMas~",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.DropColumn(
                name: "ShiireMasterShiirePrdId",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.DropColumn(
                name: "ShiireMasterShiireSakiId",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.AddForeignKey(
                name: "FK_tento_haraidashi_jisseki_shiire_master_shiire_saki_code_shi~",
                table: "tento_haraidashi_jisseki",
                columns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" },
                principalTable: "shiire_master",
                principalColumns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tento_haraidashi_jisseki_shiire_master_shiire_saki_code_shi~",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.AddColumn<string>(
                name: "ShiireMasterShiirePrdId",
                table: "tento_haraidashi_jisseki",
                type: "character varying(10)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShiireMasterShiireSakiId",
                table: "tento_haraidashi_jisseki",
                type: "character varying(10)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tento_haraidashi_jisseki_ShiireMasterShiireSakiId_ShiireMas~",
                table: "tento_haraidashi_jisseki",
                columns: new[] { "ShiireMasterShiireSakiId", "ShiireMasterShiirePrdId", "shohin_code" });

            migrationBuilder.AddForeignKey(
                name: "FK_tento_haraidashi_jisseki_shiire_master_ShiireMasterShiireSa~",
                table: "tento_haraidashi_jisseki",
                columns: new[] { "ShiireMasterShiireSakiId", "ShiireMasterShiirePrdId", "shohin_code" },
                principalTable: "shiire_master",
                principalColumns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" });
        }
    }
}

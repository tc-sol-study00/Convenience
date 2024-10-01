using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class haraidashi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tento_haraidashi_jisseki",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.AddColumn<string>(
                name: "tento_haraidashi_code",
                table: "tento_haraidashi_jisseki",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tento_haraidashi_jisseki",
                table: "tento_haraidashi_jisseki",
                columns: new[] { "tento_haraidashi_code", "shiire_saki_code", "shiire_prd_code", "shohin_code" });

            migrationBuilder.CreateTable(
                name: "tento_haraidashi_header",
                columns: table => new
                {
                    tento_haraidashi_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    haraidashi_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tento_haraidashi_header", x => x.tento_haraidashi_code);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tento_haraidashi_jisseki_shiire_saki_code_shiire_prd_code_s~",
                table: "tento_haraidashi_jisseki",
                columns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code" });

            migrationBuilder.AddForeignKey(
                name: "FK_tento_haraidashi_jisseki_tento_haraidashi_header_tento_hara~",
                table: "tento_haraidashi_jisseki",
                column: "tento_haraidashi_code",
                principalTable: "tento_haraidashi_header",
                principalColumn: "tento_haraidashi_code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tento_haraidashi_jisseki_tento_haraidashi_header_tento_hara~",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.DropTable(
                name: "tento_haraidashi_header");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tento_haraidashi_jisseki",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.DropIndex(
                name: "IX_tento_haraidashi_jisseki_shiire_saki_code_shiire_prd_code_s~",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.DropColumn(
                name: "tento_haraidashi_code",
                table: "tento_haraidashi_jisseki");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tento_haraidashi_jisseki",
                table: "tento_haraidashi_jisseki",
                columns: new[] { "shiire_saki_code", "shiire_prd_code", "shohin_code", "haraidashi_date" });
        }
    }
}

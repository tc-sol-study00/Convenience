using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class Kaikei : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropPrimaryKey(
                name: "PK_kaikei_jisseki",
                table: "kaikei_jisseki");

            migrationBuilder.AddColumn<string>(
                name: "uriage_datetimeid",
                table: "kaikei_jisseki",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_kaikei_jisseki",
                table: "kaikei_jisseki",
                columns: new[] { "uriage_datetimeid", "shohin_code", "uriage_datetime" });

            migrationBuilder.CreateTable(
                name: "kaikei_header",
                columns: table => new
                {
                    uriage_datetimeid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    uriage_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kaikei_header", x => x.uriage_datetimeid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kaikei_jisseki_shohin_code",
                table: "kaikei_jisseki",
                column: "shohin_code");

            migrationBuilder.AddForeignKey(
                name: "FK_kaikei_jisseki_kaikei_header_uriage_datetimeid",
                table: "kaikei_jisseki",
                column: "uriage_datetimeid",
                principalTable: "kaikei_header",
                principalColumn: "uriage_datetimeid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_kaikei_jisseki_kaikei_header_uriage_datetimeid",
                table: "kaikei_jisseki");

            migrationBuilder.DropTable(
                name: "kaikei_header");

            migrationBuilder.DropPrimaryKey(
                name: "PK_kaikei_jisseki",
                table: "kaikei_jisseki");

            migrationBuilder.DropIndex(
                name: "IX_kaikei_jisseki_shohin_code",
                table: "kaikei_jisseki");

            migrationBuilder.DropColumn(
                name: "uriage_datetimeid",
                table: "kaikei_jisseki");

            migrationBuilder.AddPrimaryKey(
                name: "PK_kaikei_jisseki",
                table: "kaikei_jisseki",
                columns: new[] { "shohin_code", "uriage_datetime" });
        }
    }
}

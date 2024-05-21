using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class seq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_shiire_jisseki",
                table: "shiire_jisseki");

            migrationBuilder.AddColumn<DateTime>(
                name: "shiire_date",
                table: "shiire_jisseki",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "seq_by_shiiredate",
                table: "shiire_jisseki",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_shiire_jisseki",
                table: "shiire_jisseki",
                columns: new[] { "chumon_code", "shiire_date", "seq_by_shiiredate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_shiire_jisseki",
                table: "shiire_jisseki");

            migrationBuilder.DropColumn(
                name: "shiire_date",
                table: "shiire_jisseki");

            migrationBuilder.DropColumn(
                name: "seq_by_shiiredate",
                table: "shiire_jisseki");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shiire_jisseki",
                table: "shiire_jisseki",
                columns: new[] { "chumon_code", "shiire_datetime", "shiire_saki_code", "shiire_prd_code", "shohin_code" });
        }
    }
}

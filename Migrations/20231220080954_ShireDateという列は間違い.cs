using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class ShireDateという列は間違い : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShireDateTime",
                table: "shiire_jisseki");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ShireDateTime",
                table: "shiire_jisseki",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

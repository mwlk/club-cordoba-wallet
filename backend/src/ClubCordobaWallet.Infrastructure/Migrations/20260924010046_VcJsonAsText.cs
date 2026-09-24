using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubCordobaWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VcJsonAsText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "vc_json",
                table: "credentials",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "vc_json",
                table: "credentials",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}

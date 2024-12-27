using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Supervisório_Banco_Renault.Migrations
{
    /// <inheritdoc />
    public partial class SettingRFIDUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_TagRFID",
                table: "Users",
                column: "TagRFID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_TagRFID",
                table: "Users");
        }
    }
}

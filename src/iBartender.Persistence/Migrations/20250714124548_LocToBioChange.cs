using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iBartender.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LocToBioChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Users",
                newName: "Bio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Bio",
                table: "Users",
                newName: "Location");
        }
    }
}

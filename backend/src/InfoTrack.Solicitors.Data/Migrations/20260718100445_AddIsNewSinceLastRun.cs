using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoTrack.Solicitors.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsNewSinceLastRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNewSinceLastRun",
                table: "SolicitorListings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNewSinceLastRun",
                table: "SolicitorListings");
        }
    }
}

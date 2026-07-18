using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InfoTrack.Solicitors.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchRunLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchRunId = table.Column<int>(type: "int", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchRunLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchRunLocations_SearchRuns_SearchRunId",
                        column: x => x.SearchRunId,
                        principalTable: "SearchRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitorListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchRunLocationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    ProfileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailFormUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBasicListing = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitorListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitorListings_SearchRunLocations_SearchRunLocationId",
                        column: x => x.SearchRunLocationId,
                        principalTable: "SearchRunLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitorQualityMarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitorListingId = table.Column<int>(type: "int", nullable: false),
                    MarkName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitorQualityMarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitorQualityMarks_SolicitorListings_SolicitorListingId",
                        column: x => x.SolicitorListingId,
                        principalTable: "SolicitorListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "London" },
                    { 2, true, "Birmingham" },
                    { 3, true, "Leeds" },
                    { 4, true, "Manchester" },
                    { 5, true, "Sheffield" },
                    { 6, true, "Bradford" },
                    { 7, true, "Liverpool" },
                    { 8, true, "Bristol" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Name",
                table: "Locations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchRunLocations_SearchRunId",
                table: "SearchRunLocations",
                column: "SearchRunId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitorListings_SearchRunLocationId",
                table: "SolicitorListings",
                column: "SearchRunLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitorQualityMarks_SolicitorListingId",
                table: "SolicitorQualityMarks",
                column: "SolicitorListingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "SolicitorQualityMarks");

            migrationBuilder.DropTable(
                name: "SolicitorListings");

            migrationBuilder.DropTable(
                name: "SearchRunLocations");

            migrationBuilder.DropTable(
                name: "SearchRuns");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBIMa.Migrations
{
    /// <inheritdoc />
    public partial class k1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResidentRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResidentId = table.Column<int>(type: "int", nullable: false),
                    RequestDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResidentRequests_Users_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResidentRequests_ResidentId",
                table: "ResidentRequests",
                column: "ResidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResidentRequests");
        }
    }
}

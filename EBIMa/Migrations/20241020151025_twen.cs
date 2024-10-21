using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBIMa.Migrations
{
    /// <inheritdoc />
    public partial class twen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "PaymentForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "PaymentForms");
        }
    }
}

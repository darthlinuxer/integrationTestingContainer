using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace example.api.Migrations
{
    /// <inheritdoc />
    public partial class modelchanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Person",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Person",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SurName",
                table: "Person",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "SurName",
                table: "Person");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Person",
                newName: "Name");
        }
    }
}

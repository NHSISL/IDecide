using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonDataServices.IDecide.Core.Migrations
{
    /// <inheritdoc />
    public partial class ConsumerRemoveAccessTokenAddEntraIdAndContactDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "Consumers");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Consumers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Consumers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Consumers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntraId",
                table: "Consumers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Consumers");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Consumers");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Consumers");

            migrationBuilder.DropColumn(
                name: "EntraId",
                table: "Consumers");

            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "Consumers",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");
        }
    }
}

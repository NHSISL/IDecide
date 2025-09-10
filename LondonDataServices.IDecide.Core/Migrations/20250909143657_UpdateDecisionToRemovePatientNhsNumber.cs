using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonDataServices.IDecide.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDecisionToRemovePatientNhsNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientNhsNumber",
                schema: "Decision",
                table: "Decisions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientNhsNumber",
                schema: "Decision",
                table: "Decisions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}

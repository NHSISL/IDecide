using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonDataServices.IDecide.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientAndLinkToDecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                schema: "Decision",
                table: "Decisions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Patients",
                schema: "Decision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhsNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ValidationCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ValidationCodeExpiresOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_PatientId",
                schema: "Decision",
                table: "Decisions",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decisions_Patients_PatientId",
                schema: "Decision",
                table: "Decisions",
                column: "PatientId",
                principalSchema: "Decision",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decisions_Patients_PatientId",
                schema: "Decision",
                table: "Decisions");

            migrationBuilder.DropTable(
                name: "Patients",
                schema: "Decision");

            migrationBuilder.DropIndex(
                name: "IX_Decisions_PatientId",
                schema: "Decision",
                table: "Decisions");

            migrationBuilder.DropColumn(
                name: "PatientId",
                schema: "Decision",
                table: "Decisions");
        }
    }
}

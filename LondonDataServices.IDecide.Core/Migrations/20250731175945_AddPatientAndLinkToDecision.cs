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
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PostCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ValidationCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ValidationCodeExpiresOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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

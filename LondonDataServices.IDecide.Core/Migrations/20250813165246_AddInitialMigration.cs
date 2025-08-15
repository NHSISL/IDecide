using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonDataServices.IDecide.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Audit");

            migrationBuilder.EnsureSchema(
                name: "Decision");

            migrationBuilder.CreateTable(
                name: "Audits",
                schema: "Audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AuditType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LogLevel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DecisionTypes",
                schema: "Decision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                schema: "Decision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NhsNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    GivenName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    ValidationCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ValidationCodeExpiresOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Decisions",
                schema: "Decision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientNhsNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DecisionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecisionChoice = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ResponsiblePersonGivenName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ResponsiblePersonSurname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ResponsiblePersonRelationship = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Decisions_DecisionTypes_DecisionTypeId",
                        column: x => x.DecisionTypeId,
                        principalSchema: "Decision",
                        principalTable: "DecisionTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Decisions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "Decision",
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audits_AuditType",
                schema: "Audit",
                table: "Audits",
                column: "AuditType");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_CorrelationId",
                schema: "Audit",
                table: "Audits",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_LogLevel",
                schema: "Audit",
                table: "Audits",
                column: "LogLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_DecisionTypeId",
                schema: "Decision",
                table: "Decisions",
                column: "DecisionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_PatientId",
                schema: "Decision",
                table: "Decisions",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "Decisions",
                schema: "Decision");

            migrationBuilder.DropTable(
                name: "DecisionTypes",
                schema: "Decision");

            migrationBuilder.DropTable(
                name: "Patients",
                schema: "Decision");
        }
    }
}

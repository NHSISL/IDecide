using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonDataServices.IDecide.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Audit");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits",
                schema: "Audit");
        }
    }
}

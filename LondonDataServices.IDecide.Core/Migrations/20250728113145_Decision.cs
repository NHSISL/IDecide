using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonDataServices.IDecide.Core.Migrations
{
    /// <inheritdoc />
    public partial class Decision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Decisions",
                schema: "Decision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientNhsNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DecisionTypeId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 450, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_DecisionTypeId",
                schema: "Decision",
                table: "Decisions",
                column: "DecisionTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Decisions",
                schema: "Decision");
        }
    }
}

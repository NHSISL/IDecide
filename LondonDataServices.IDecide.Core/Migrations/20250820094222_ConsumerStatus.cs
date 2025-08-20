using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonDataServices.IDecide.Core.Migrations
{
    /// <inheritdoc />
    public partial class ConsumerStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsumerStatuses",
                schema: "Consumer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumerStatuses_Consumers_ConsumerId",
                        column: x => x.ConsumerId,
                        principalSchema: "Consumer",
                        principalTable: "Consumers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConsumerStatuses_Decisions_DecisionId",
                        column: x => x.DecisionId,
                        principalSchema: "Decision",
                        principalTable: "Decisions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerStatuses_ConsumerId_DecisionId",
                schema: "Consumer",
                table: "ConsumerStatuses",
                columns: new[] { "ConsumerId", "DecisionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerStatuses_DecisionId",
                schema: "Consumer",
                table: "ConsumerStatuses",
                column: "DecisionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumerStatuses",
                schema: "Consumer");
        }
    }
}

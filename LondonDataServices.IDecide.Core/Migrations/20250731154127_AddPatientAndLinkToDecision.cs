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
            migrationBuilder.DropForeignKey(
                name: "FK_Decisions_Patient_PatientId",
                schema: "Decision",
                table: "Decisions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Patient_TempId1",
                table: "Patient");

            migrationBuilder.RenameTable(
                name: "Patient",
                newName: "Patients",
                newSchema: "Decision");

            migrationBuilder.RenameColumn(
                name: "TempId1",
                schema: "Decision",
                table: "Patients",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "Decision",
                table: "Patients",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateOfBirth",
                schema: "Decision",
                table: "Patients",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NHSNumber",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NhsNumber",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                schema: "Decision",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "Decision",
                table: "Patients",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ValidationCode",
                schema: "Decision",
                table: "Patients",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidationCodeExpiresOn",
                schema: "Decision",
                table: "Patients",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                schema: "Decision",
                table: "Patients",
                column: "Id");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "GivenName",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "NHSNumber",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "NhsNumber",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PostCode",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Surname",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ValidationCode",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ValidationCodeExpiresOn",
                schema: "Decision",
                table: "Patients");

            migrationBuilder.RenameTable(
                name: "Patients",
                schema: "Decision",
                newName: "Patient");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Patient",
                newName: "TempId1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Patient_TempId1",
                table: "Patient",
                column: "TempId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Decisions_Patient_PatientId",
                schema: "Decision",
                table: "Decisions",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "TempId1");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMS.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyPocModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_Participants_ParticipantId",
                table: "QuestionResponses");

            migrationBuilder.DropTable(
                name: "QuestionResponseOptions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionResponses_ParticipantId",
                table: "QuestionResponses");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "QuestionResponses");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "QuestionnaireAssignments");

            migrationBuilder.AddColumn<string>(
                name: "SelectedOptionIds",
                table: "QuestionResponses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedOptionIds",
                table: "QuestionResponses");

            migrationBuilder.AddColumn<int>(
                name: "ParticipantId",
                table: "QuestionResponses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "QuestionnaireAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionResponseOptions",
                columns: table => new
                {
                    QuestionResponseOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionOptionId = table.Column<int>(type: "int", nullable: false),
                    QuestionResponseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionResponseOptions", x => x.QuestionResponseOptionId);
                    table.ForeignKey(
                        name: "FK_QuestionResponseOptions_QuestionOptions_QuestionOptionId",
                        column: x => x.QuestionOptionId,
                        principalTable: "QuestionOptions",
                        principalColumn: "QuestionOptionId");
                    table.ForeignKey(
                        name: "FK_QuestionResponseOptions_QuestionResponses_QuestionResponseId",
                        column: x => x.QuestionResponseId,
                        principalTable: "QuestionResponses",
                        principalColumn: "QuestionResponseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_ParticipantId",
                table: "QuestionResponses",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponseOptions_QuestionOptionId",
                table: "QuestionResponseOptions",
                column: "QuestionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponseOptions_QuestionResponseId",
                table: "QuestionResponseOptions",
                column: "QuestionResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_Participants_ParticipantId",
                table: "QuestionResponses",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "ParticipantId");
        }
    }
}

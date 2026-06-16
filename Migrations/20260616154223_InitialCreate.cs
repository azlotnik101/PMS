using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    ParticipantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DateOfBirth = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ParticipantCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.ParticipantId);
                });

            migrationBuilder.CreateTable(
                name: "Questionnaires",
                columns: table => new
                {
                    QuestionnaireId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaires", x => x.QuestionnaireId);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireAssignments",
                columns: table => new
                {
                    QuestionnaireAssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    QuestionnaireId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireAssignments", x => x.QuestionnaireAssignmentId);
                    table.ForeignKey(
                        name: "FK_QuestionnaireAssignments_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "ParticipantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionnaireAssignments_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "QuestionnaireId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionResponses",
                columns: table => new
                {
                    QuestionResponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionnaireAssignmentId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NumericValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SelectedChoiceIds = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AnsweredDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionResponses", x => x.QuestionResponseId);
                    table.ForeignKey(
                        name: "FK_QuestionResponses_QuestionnaireAssignments_QuestionnaireAssignmentId",
                        column: x => x.QuestionnaireAssignmentId,
                        principalTable: "QuestionnaireAssignments",
                        principalColumn: "QuestionnaireAssignmentId");
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionnaireId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ParentQuestionId = table.Column<int>(type: "int", nullable: true),
                    ParentChoiceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "QuestionnaireId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Questions_Questions_ParentQuestionId",
                        column: x => x.ParentQuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId");
                });

            migrationBuilder.CreateTable(
                name: "SelectableQuestionChoices",
                columns: table => new
                {
                    SelectableQuestionChoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    ChoiceText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectableQuestionChoices", x => x.SelectableQuestionChoiceId);
                    table.ForeignKey(
                        name: "FK_SelectableQuestionChoices_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Participants_EmailAddress",
                table: "Participants",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ParticipantCode",
                table: "Participants",
                column: "ParticipantCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAssignments_ParticipantId_QuestionnaireId",
                table: "QuestionnaireAssignments",
                columns: new[] { "ParticipantId", "QuestionnaireId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAssignments_QuestionnaireId",
                table: "QuestionnaireAssignments",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_QuestionId",
                table: "QuestionResponses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_QuestionnaireAssignmentId",
                table: "QuestionResponses",
                column: "QuestionnaireAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ParentChoiceId",
                table: "Questions",
                column: "ParentChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ParentQuestionId",
                table: "Questions",
                column: "ParentQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionnaireId",
                table: "Questions",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectableQuestionChoices_QuestionId",
                table: "SelectableQuestionChoices",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_Questions_QuestionId",
                table: "QuestionResponses",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_SelectableQuestionChoices_ParentChoiceId",
                table: "Questions",
                column: "ParentChoiceId",
                principalTable: "SelectableQuestionChoices",
                principalColumn: "SelectableQuestionChoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Questionnaires_QuestionnaireId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectableQuestionChoices_Questions_QuestionId",
                table: "SelectableQuestionChoices");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "QuestionResponses");

            migrationBuilder.DropTable(
                name: "QuestionnaireAssignments");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Questionnaires");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "SelectableQuestionChoices");
        }
    }
}

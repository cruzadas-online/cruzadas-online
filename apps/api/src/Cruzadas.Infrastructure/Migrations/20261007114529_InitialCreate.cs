using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cruzadas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quiz_attempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false),
                    CorrectAnswersCount = table.Column<int>(type: "integer", nullable: false),
                    ScorePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_attempts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "quizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    QuestionsPerAttempt = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "attempt_answers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizAttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SelectedOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    AnsweredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attempt_answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attempt_answers_quiz_attempts_QuizAttemptId",
                        column: x => x.QuizAttemptId,
                        principalTable: "quiz_attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Explanation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    SourceReference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_questions_quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "answer_options",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answer_options", x => x.Id);
                    table.ForeignKey(
                        name: "FK_answer_options_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attempt_questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizAttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attempt_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attempt_questions_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attempt_questions_quiz_attempts_QuizAttemptId",
                        column: x => x.QuizAttemptId,
                        principalTable: "quiz_attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_answer_options_QuestionId",
                table: "answer_options",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_attempt_answers_QuizAttemptId",
                table: "attempt_answers",
                column: "QuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_attempt_questions_QuestionId",
                table: "attempt_questions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_attempt_questions_QuizAttemptId",
                table: "attempt_questions",
                column: "QuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_QuizId",
                table: "questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_Slug",
                table: "quizzes",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answer_options");

            migrationBuilder.DropTable(
                name: "attempt_answers");

            migrationBuilder.DropTable(
                name: "attempt_questions");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "quiz_attempts");

            migrationBuilder.DropTable(
                name: "quizzes");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cruzadas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizGroupsAndCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DifficultyLevel",
                table: "quizzes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Iniciante");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "quizzes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "quiz_groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_groups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_GroupId",
                table: "quizzes",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_groups_Slug",
                table: "quiz_groups",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_quizzes_quiz_groups_GroupId",
                table: "quizzes",
                column: "GroupId",
                principalTable: "quiz_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quizzes_quiz_groups_GroupId",
                table: "quizzes");

            migrationBuilder.DropTable(
                name: "quiz_groups");

            migrationBuilder.DropIndex(
                name: "IX_quizzes_GroupId",
                table: "quizzes");

            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "quizzes");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "quizzes");
        }
    }
}

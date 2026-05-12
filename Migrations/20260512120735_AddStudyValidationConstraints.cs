using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaJourney.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyValidationConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_PracticeLogs_AccuracyScore",
                table: "PracticeLogs",
                sql: "[AccuracyScore] >= 1 AND [AccuracyScore] <= 100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PracticeLogs_DurationMinutes",
                table: "PracticeLogs",
                sql: "[DurationMinutes] >= 5 AND [DurationMinutes] <= 240");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PracticeLogs_ReviewOrder",
                table: "PracticeLogs",
                sql: "[NextReviewDate] >= [PracticedOn]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Lessons_EstimatedMinutes",
                table: "Lessons",
                sql: "[EstimatedMinutes] >= 10 AND [EstimatedMinutes] <= 240");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LanguageTracks_WeeklyStudyGoalHours",
                table: "LanguageTracks",
                sql: "[WeeklyStudyGoalHours] >= 1 AND [WeeklyStudyGoalHours] <= 40");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PracticeLogs_AccuracyScore",
                table: "PracticeLogs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PracticeLogs_DurationMinutes",
                table: "PracticeLogs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PracticeLogs_ReviewOrder",
                table: "PracticeLogs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Lessons_EstimatedMinutes",
                table: "Lessons");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LanguageTracks_WeeklyStudyGoalHours",
                table: "LanguageTracks");
        }
    }
}

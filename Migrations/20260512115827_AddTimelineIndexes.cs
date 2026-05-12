using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaJourney.Migrations
{
    /// <inheritdoc />
    public partial class AddTimelineIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PracticeLogs_UserId",
                table: "PracticeLogs");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_UserId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_LanguageTracks_UserId",
                table: "LanguageTracks");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeLogs_UserId_NextReviewDate",
                table: "PracticeLogs",
                columns: new[] { "UserId", "NextReviewDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_UserId_IsCompleted_ScheduledDate",
                table: "Lessons",
                columns: new[] { "UserId", "IsCompleted", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LanguageTracks_UserId_MilestoneDate",
                table: "LanguageTracks",
                columns: new[] { "UserId", "MilestoneDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PracticeLogs_UserId_NextReviewDate",
                table: "PracticeLogs");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_UserId_IsCompleted_ScheduledDate",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_LanguageTracks_UserId_MilestoneDate",
                table: "LanguageTracks");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeLogs_UserId",
                table: "PracticeLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_UserId",
                table: "Lessons",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageTracks_UserId",
                table: "LanguageTracks",
                column: "UserId");
        }
    }
}

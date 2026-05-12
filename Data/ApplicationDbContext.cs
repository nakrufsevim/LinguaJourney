using LinguaJourney.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinguaJourney.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LanguageTrack> LanguageTracks => Set<LanguageTrack>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<PracticeLog> PracticeLogs => Set<PracticeLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LanguageTrack>()
                .HasOne(track => track.User)
                .WithMany(user => user.LanguageTracks)
                .HasForeignKey(track => track.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LanguageTrack>()
                .HasIndex(track => new { track.UserId, track.MilestoneDate });

            modelBuilder.Entity<LanguageTrack>()
                .ToTable(table => table.HasCheckConstraint(
                    "CK_LanguageTracks_WeeklyStudyGoalHours",
                    "[WeeklyStudyGoalHours] >= 1 AND [WeeklyStudyGoalHours] <= 40"));

            modelBuilder.Entity<Lesson>()
                .HasOne(lesson => lesson.User)
                .WithMany(user => user.Lessons)
                .HasForeignKey(lesson => lesson.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lesson>()
                .HasIndex(lesson => new { lesson.UserId, lesson.IsCompleted, lesson.ScheduledDate });

            modelBuilder.Entity<Lesson>()
                .ToTable(table => table.HasCheckConstraint(
                    "CK_Lessons_EstimatedMinutes",
                    "[EstimatedMinutes] >= 10 AND [EstimatedMinutes] <= 240"));

            modelBuilder.Entity<Lesson>()
                .HasOne(lesson => lesson.LanguageTrack)
                .WithMany(track => track.Lessons)
                .HasForeignKey(lesson => lesson.LanguageTrackId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PracticeLog>()
                .HasOne(log => log.User)
                .WithMany(user => user.PracticeLogs)
                .HasForeignKey(log => log.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PracticeLog>()
                .HasIndex(log => new { log.UserId, log.NextReviewDate });

            modelBuilder.Entity<PracticeLog>()
                .ToTable(table =>
                {
                    table.HasCheckConstraint(
                        "CK_PracticeLogs_AccuracyScore",
                        "[AccuracyScore] >= 1 AND [AccuracyScore] <= 100");
                    table.HasCheckConstraint(
                        "CK_PracticeLogs_DurationMinutes",
                        "[DurationMinutes] >= 5 AND [DurationMinutes] <= 240");
                    table.HasCheckConstraint(
                        "CK_PracticeLogs_ReviewOrder",
                        "[NextReviewDate] >= [PracticedOn]");
                });

            modelBuilder.Entity<PracticeLog>()
                .HasOne(log => log.Lesson)
                .WithMany(lesson => lesson.PracticeLogs)
                .HasForeignKey(log => log.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 

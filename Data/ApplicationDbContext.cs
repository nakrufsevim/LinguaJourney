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

            modelBuilder.Entity<Lesson>()
                .HasOne(lesson => lesson.User)
                .WithMany(user => user.Lessons)
                .HasForeignKey(lesson => lesson.UserId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .HasOne(log => log.Lesson)
                .WithMany(lesson => lesson.PracticeLogs)
                .HasForeignKey(log => log.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 

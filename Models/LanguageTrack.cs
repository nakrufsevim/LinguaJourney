using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseNotesSharing.Models
{
    public class LanguageTrack
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        public string NativeLanguage { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string CurrentLevel { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string GoalLevel { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FocusArea { get; set; } = string.Empty;

        [Required]
        [StringLength(450)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 40)]
        public int WeeklyStudyGoalHours { get; set; }

        [DataType(DataType.Date)]
        public DateTime MilestoneDate { get; set; } = DateTime.Today.AddMonths(3);

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}

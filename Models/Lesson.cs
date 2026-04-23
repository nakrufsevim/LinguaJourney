using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinguaJourney.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        public string SkillArea { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CefrLevel { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VocabularyTheme { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Summary { get; set; } = string.Empty;

        [Required]
        [StringLength(350)]
        public string PracticeTask { get; set; } = string.Empty;

        [Range(10, 240)]
        public int EstimatedMinutes { get; set; }

        [DataType(DataType.Date)]
        public DateTime ScheduledDate { get; set; } = DateTime.Today.AddDays(3);

        public bool IsCompleted { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int LanguageTrackId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(LanguageTrackId))]
        public virtual LanguageTrack LanguageTrack { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<PracticeLog> PracticeLogs { get; set; } = new List<PracticeLog>();
    }
}

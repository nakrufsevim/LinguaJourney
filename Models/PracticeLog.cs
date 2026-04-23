using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinguaJourney.Models
{
    public class PracticeLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string SessionTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string ActivityType { get; set; } = string.Empty;

        [Required]
        [StringLength(120)]
        public string NewExpression { get; set; } = string.Empty;

        [Required]
        [StringLength(450)]
        public string Reflection { get; set; } = string.Empty;

        [Range(1, 100)]
        public int AccuracyScore { get; set; }

        [Range(5, 240)]
        public int DurationMinutes { get; set; }

        [DataType(DataType.Date)]
        public DateTime PracticedOn { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime NextReviewDate { get; set; } = DateTime.Today.AddDays(7);

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int LessonId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(LessonId))]
        public virtual Lesson Lesson { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}

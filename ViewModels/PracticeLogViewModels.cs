using System.ComponentModel.DataAnnotations;

namespace LinguaJourney.ViewModels
{
    public class PracticeLogFormViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Lesson")]
        public int LessonId { get; set; }

        [Required]
        [Display(Name = "Session Title")]
        [StringLength(120)]
        public string SessionTitle { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Activity Type")]
        [StringLength(40)]
        public string ActivityType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "New Expression")]
        [StringLength(120)]
        public string NewExpression { get; set; } = string.Empty;

        [Required]
        [StringLength(450)]
        public string Reflection { get; set; } = string.Empty;

        [Range(1, 100)]
        [Display(Name = "Accuracy Score")]
        public int AccuracyScore { get; set; } = 80;

        [Range(5, 240)]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; } = 30;

        [DataType(DataType.Date)]
        [Display(Name = "Practiced On")]
        public DateTime PracticedOn { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Next Review Date")]
        public DateTime NextReviewDate { get; set; } = DateTime.Today.AddDays(7);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PracticedOn.Date > DateTime.Today)
            {
                yield return new ValidationResult(
                    "Practice date cannot be in the future.",
                    new[] { nameof(PracticedOn) });
            }

            var minimumNextReviewDate = Id == 0 ? DateTime.Today : PracticedOn.Date;
            var nextReviewMessage = Id == 0
                ? "Next review date cannot be in the past."
                : "Next review date cannot be earlier than the practice date.";

            if (NextReviewDate.Date < minimumNextReviewDate)
            {
                yield return new ValidationResult(
                    nextReviewMessage,
                    new[] { nameof(NextReviewDate) });
            }
        }
    }

    public class PracticeLogListItemViewModel
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string TrackName { get; set; } = string.Empty;
        public string SessionTitle { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public int AccuracyScore { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime PracticedOn { get; set; }
    }

    public class PracticeLogDetailsViewModel
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string TrackName { get; set; } = string.Empty;
        public string SessionTitle { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string NewExpression { get; set; } = string.Empty;
        public string Reflection { get; set; } = string.Empty;
        public int AccuracyScore { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime PracticedOn { get; set; }
        public DateTime NextReviewDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CourseNotesSharing.ViewModels
{
    public class LessonFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Language Track")]
        public int LanguageTrackId { get; set; }

        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Skill Area")]
        [StringLength(60)]
        public string SkillArea { get; set; } = string.Empty;

        [Required]
        [Display(Name = "CEFR Level")]
        [StringLength(20)]
        public string CefrLevel { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vocabulary Theme")]
        [StringLength(100)]
        public string VocabularyTheme { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Summary { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Practice Task")]
        [StringLength(350)]
        public string PracticeTask { get; set; } = string.Empty;

        [Range(10, 240)]
        [Display(Name = "Estimated Minutes")]
        public int EstimatedMinutes { get; set; } = 45;

        [DataType(DataType.Date)]
        [Display(Name = "Scheduled Date")]
        public DateTime ScheduledDate { get; set; } = DateTime.Today.AddDays(3);

        [Display(Name = "Completed")]
        public bool IsCompleted { get; set; }
    }

    public class LessonListItemViewModel
    {
        public int Id { get; set; }
        public int LanguageTrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SkillArea { get; set; } = string.Empty;
        public string CefrLevel { get; set; } = string.Empty;
        public string VocabularyTheme { get; set; } = string.Empty;
        public int EstimatedMinutes { get; set; }
        public DateTime ScheduledDate { get; set; }
        public bool IsCompleted { get; set; }
        public int PracticeLogCount { get; set; }
    }

    public class LessonDetailsViewModel
    {
        public int Id { get; set; }
        public int LanguageTrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SkillArea { get; set; } = string.Empty;
        public string CefrLevel { get; set; } = string.Empty;
        public string VocabularyTheme { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string PracticeTask { get; set; } = string.Empty;
        public int EstimatedMinutes { get; set; }
        public DateTime ScheduledDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public IEnumerable<LessonPracticePreviewViewModel> PracticeLogs { get; set; } = new List<LessonPracticePreviewViewModel>();
    }

    public class LessonPracticePreviewViewModel
    {
        public int Id { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public DateTime PracticedOn { get; set; }
        public int AccuracyScore { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace LinguaJourney.ViewModels
{
    public class LanguageTrackFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Target Language")]
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Native Language")]
        [StringLength(60)]
        public string NativeLanguage { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Current Level")]
        [StringLength(30)]
        public string CurrentLevel { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Goal Level")]
        [StringLength(30)]
        public string GoalLevel { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Focus Area")]
        [StringLength(100)]
        public string FocusArea { get; set; } = string.Empty;

        [Required]
        [StringLength(450)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 40)]
        [Display(Name = "Weekly Study Goal (hours)")]
        public int WeeklyStudyGoalHours { get; set; } = 5;

        [DataType(DataType.Date)]
        [Display(Name = "Milestone Date")]
        public DateTime MilestoneDate { get; set; } = DateTime.Today.AddMonths(3);
    }

    public class LanguageTrackListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CurrentLevel { get; set; } = string.Empty;
        public string GoalLevel { get; set; } = string.Empty;
        public string FocusArea { get; set; } = string.Empty;
        public int WeeklyStudyGoalHours { get; set; }
        public DateTime MilestoneDate { get; set; }
        public int LessonCount { get; set; }
        public int CompletedLessons { get; set; }
        public int PracticeMinutes { get; set; }
    }

    public class LanguageTrackDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NativeLanguage { get; set; } = string.Empty;
        public string CurrentLevel { get; set; } = string.Empty;
        public string GoalLevel { get; set; } = string.Empty;
        public string FocusArea { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int WeeklyStudyGoalHours { get; set; }
        public DateTime MilestoneDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LessonCount { get; set; }
        public int PracticeLogCount { get; set; }
        public IEnumerable<LanguageTrackLessonPreviewViewModel> Lessons { get; set; } = new List<LanguageTrackLessonPreviewViewModel>();
    }

    public class LanguageTrackLessonPreviewViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SkillArea { get; set; } = string.Empty;
        public string CefrLevel { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int PracticeLogCount { get; set; }
    }
}

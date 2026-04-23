namespace LinguaJourney.ViewModels
{
    public class HomeDashboardViewModel
    {
        public bool IsAuthenticated { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public int LanguageTrackCount { get; set; }
        public int LessonCount { get; set; }
        public int PracticeLogCount { get; set; }
        public int CompletedLessonCount { get; set; }
        public int WeeklyMinutes { get; set; }
        public IEnumerable<DashboardTrackViewModel> PriorityTracks { get; set; } = new List<DashboardTrackViewModel>();
        public IEnumerable<DashboardLessonViewModel> UpcomingLessons { get; set; } = new List<DashboardLessonViewModel>();
        public IEnumerable<DashboardPracticeViewModel> RecentPracticeLogs { get; set; } = new List<DashboardPracticeViewModel>();
    }

    public class DashboardTrackViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GoalLevel { get; set; } = string.Empty;
        public string FocusArea { get; set; } = string.Empty;
        public int LessonCount { get; set; }
    }

    public class DashboardLessonViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TrackName { get; set; } = string.Empty;
        public string SkillArea { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class DashboardPracticeViewModel
    {
        public int Id { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string LessonTitle { get; set; } = string.Empty;
        public DateTime PracticedOn { get; set; }
        public int AccuracyScore { get; set; }
    }

    public class SearchResultViewModel
    {
        public string Query { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public IEnumerable<SearchResultCardViewModel> Tracks { get; set; } = new List<SearchResultCardViewModel>();
        public IEnumerable<SearchResultCardViewModel> Lessons { get; set; } = new List<SearchResultCardViewModel>();
        public IEnumerable<SearchResultCardViewModel> PracticeLogs { get; set; } = new List<SearchResultCardViewModel>();
        public int TotalResults => Tracks.Count() + Lessons.Count() + PracticeLogs.Count();
    }

    public class SearchResultCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
    }
}

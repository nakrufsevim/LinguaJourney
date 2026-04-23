using System.Diagnostics;
using LinguaJourney.Data;
using LinguaJourney.Models;
using LinguaJourney.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinguaJourney.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "LinguaJourney";
            ViewData["HeroLabel"] = "Foreign Language Learning App";
            ViewBag.Highlight = "Plan your study path, practice deliberately, and track real progress.";

            var model = new HomeDashboardViewModel
            {
                IsAuthenticated = User.Identity?.IsAuthenticated == true
            };

            if (!model.IsAuthenticated)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return View(model);
            }

            var weekStart = DateTime.Today.AddDays(-6);
            var user = await _userManager.GetUserAsync(User);

            model.FirstName = user?.FirstName ?? "Learner";
            model.LanguageTrackCount = await _context.LanguageTracks.CountAsync(track => track.UserId == userId);
            model.LessonCount = await _context.Lessons.CountAsync(lesson => lesson.UserId == userId);
            model.PracticeLogCount = await _context.PracticeLogs.CountAsync(log => log.UserId == userId);
            model.CompletedLessonCount = await _context.Lessons.CountAsync(lesson => lesson.UserId == userId && lesson.IsCompleted);
            model.WeeklyMinutes = await _context.PracticeLogs
                .Where(log => log.UserId == userId && log.PracticedOn >= weekStart)
                .SumAsync(log => (int?)log.DurationMinutes) ?? 0;

            model.PriorityTracks = await _context.LanguageTracks
                .AsNoTracking()
                .Where(track => track.UserId == userId)
                .OrderBy(track => track.MilestoneDate)
                .Select(track => new DashboardTrackViewModel
                {
                    Id = track.Id,
                    Name = track.Name,
                    GoalLevel = track.GoalLevel,
                    FocusArea = track.FocusArea,
                    LessonCount = track.Lessons.Count()
                })
                .Take(3)
                .ToListAsync();

            model.UpcomingLessons = await _context.Lessons
                .AsNoTracking()
                .Where(lesson => lesson.UserId == userId)
                .OrderBy(lesson => lesson.IsCompleted)
                .ThenBy(lesson => lesson.ScheduledDate)
                .Select(lesson => new DashboardLessonViewModel
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    TrackName = lesson.LanguageTrack.Name,
                    SkillArea = lesson.SkillArea,
                    ScheduledDate = lesson.ScheduledDate,
                    IsCompleted = lesson.IsCompleted
                })
                .Take(4)
                .ToListAsync();

            model.RecentPracticeLogs = await _context.PracticeLogs
                .AsNoTracking()
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.PracticedOn)
                .Select(log => new DashboardPracticeViewModel
                {
                    Id = log.Id,
                    SessionTitle = log.SessionTitle,
                    ActivityType = log.ActivityType,
                    LessonTitle = log.Lesson.Title,
                    PracticedOn = log.PracticedOn,
                    AccuracyScore = log.AccuracyScore
                })
                .Take(4)
                .ToListAsync();

            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Title"] = "Learning Method";
            ViewBag.Framework = "Track -> Lesson -> Practice";
            return View();
        }

        public async Task<IActionResult> Search(string? query)
        {
            ViewData["Title"] = "Search";
            ViewData["SearchTip"] = "Search your language tracks, lessons, and practice reflections from one place.";

            var model = new SearchResultViewModel
            {
                Query = query?.Trim() ?? string.Empty,
                IsAuthenticated = User.Identity?.IsAuthenticated == true
            };

            if (!model.IsAuthenticated || string.IsNullOrWhiteSpace(model.Query))
            {
                ViewBag.SearchMessage = model.IsAuthenticated
                    ? "Start with a word, theme, language, or skill."
                    : "Login to search your study library.";
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return View(model);
            }

            model.Tracks = await _context.LanguageTracks
                .AsNoTracking()
                .Where(track => track.UserId == userId &&
                    (track.Name.Contains(model.Query) ||
                     track.FocusArea.Contains(model.Query) ||
                     track.Description.Contains(model.Query)))
                .OrderBy(track => track.Name)
                .Select(track => new SearchResultCardViewModel
                {
                    Id = track.Id,
                    Title = track.Name,
                    Subtitle = $"{track.CurrentLevel} -> {track.GoalLevel}",
                    Description = track.Description,
                    ControllerName = "LanguageTracks"
                })
                .ToListAsync();

            model.Lessons = await _context.Lessons
                .AsNoTracking()
                .Where(lesson => lesson.UserId == userId &&
                    (lesson.Title.Contains(model.Query) ||
                     lesson.SkillArea.Contains(model.Query) ||
                     lesson.VocabularyTheme.Contains(model.Query) ||
                     lesson.Summary.Contains(model.Query)))
                .OrderBy(lesson => lesson.ScheduledDate)
                .Select(lesson => new SearchResultCardViewModel
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    Subtitle = $"{lesson.LanguageTrack.Name} · {lesson.SkillArea}",
                    Description = lesson.PracticeTask,
                    ControllerName = "Lessons"
                })
                .ToListAsync();

            model.PracticeLogs = await _context.PracticeLogs
                .AsNoTracking()
                .Where(log => log.UserId == userId &&
                    (log.SessionTitle.Contains(model.Query) ||
                     log.ActivityType.Contains(model.Query) ||
                     log.NewExpression.Contains(model.Query) ||
                     log.Reflection.Contains(model.Query)))
                .OrderByDescending(log => log.PracticedOn)
                .Select(log => new SearchResultCardViewModel
                {
                    Id = log.Id,
                    Title = log.SessionTitle,
                    Subtitle = $"{log.Lesson.Title} · {log.ActivityType}",
                    Description = log.Reflection,
                    ControllerName = "PracticeLogs"
                })
                .ToListAsync();

            ViewBag.SearchMessage = model.TotalResults == 0
                ? "No matching study items yet."
                : $"{model.TotalResults} result(s) found.";

            return View(model);
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}

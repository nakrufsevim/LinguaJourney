using System.Security.Claims;
using CourseNotesSharing.Data;
using CourseNotesSharing.Models;
using CourseNotesSharing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseNotesSharing.Controllers
{
    [Authorize]
    public class LanguageTracksController : Controller
    {
        private static readonly string[] LevelOptions =
        {
            "A1 Beginner",
            "A2 Elementary",
            "B1 Intermediate",
            "B2 Upper-Intermediate",
            "C1 Advanced",
            "C2 Fluent"
        };

        private static readonly string[] FocusAreas =
        {
            "Conversation",
            "Travel",
            "Academic Writing",
            "Business Communication",
            "Exam Preparation",
            "Grammar Accuracy",
            "Listening Confidence"
        };

        private readonly ApplicationDbContext _context;

        public LanguageTracksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            ViewData["Title"] = "Language Tracks";
            ViewData["SectionText"] = "Organize each target language with a clear level goal, milestone, and weekly rhythm.";
            ViewBag.SearchTerm = searchTerm;

            var userId = GetUserId();
            var query = BuildTrackQuery(userId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(track =>
                    track.Name.Contains(searchTerm) ||
                    track.FocusArea.Contains(searchTerm) ||
                    track.GoalLevel.Contains(searchTerm));
            }

            var tracks = await query
                .OrderBy(track => track.MilestoneDate)
                .ToListAsync();

            return View(tracks);
        }

        public async Task<IActionResult> MyTracks()
        {
            ViewData["Title"] = "My Focus Tracks";
            ViewData["SectionText"] = "A tighter view of the language plans that deserve attention first.";

            var tracks = await BuildTrackQuery(GetUserId())
                .OrderBy(track => track.CompletedLessons == track.LessonCount)
                .ThenBy(track => track.MilestoneDate)
                .ToListAsync();

            return View(tracks);
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "Track Details";

            var track = await _context.LanguageTracks
                .AsNoTracking()
                .Where(item => item.Id == id && item.UserId == GetUserId())
                .Select(item => new LanguageTrackDetailsViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    NativeLanguage = item.NativeLanguage,
                    CurrentLevel = item.CurrentLevel,
                    GoalLevel = item.GoalLevel,
                    FocusArea = item.FocusArea,
                    Description = item.Description,
                    WeeklyStudyGoalHours = item.WeeklyStudyGoalHours,
                    MilestoneDate = item.MilestoneDate,
                    CreatedDate = item.CreatedDate,
                    LessonCount = item.Lessons.Count(),
                    PracticeLogCount = item.Lessons.SelectMany(lesson => lesson.PracticeLogs).Count(),
                    Lessons = item.Lessons
                        .OrderBy(lesson => lesson.ScheduledDate)
                        .Select(lesson => new LanguageTrackLessonPreviewViewModel
                        {
                            Id = lesson.Id,
                            Title = lesson.Title,
                            SkillArea = lesson.SkillArea,
                            CefrLevel = lesson.CefrLevel,
                            IsCompleted = lesson.IsCompleted,
                            PracticeLogCount = lesson.PracticeLogs.Count()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (track == null)
            {
                return NotFound();
            }

            ViewBag.DaysRemaining = Math.Max((track.MilestoneDate.Date - DateTime.Today).Days, 0);
            return View(track);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Create Language Track";
            PopulateFormOptions();
            return View(new LanguageTrackFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LanguageTrackFormViewModel model)
        {
            ViewData["Title"] = "Create Language Track";

            if (!ModelState.IsValid)
            {
                PopulateFormOptions(model);
                return View(model);
            }

            var track = new LanguageTrack
            {
                Name = model.Name,
                NativeLanguage = model.NativeLanguage,
                CurrentLevel = model.CurrentLevel,
                GoalLevel = model.GoalLevel,
                FocusArea = model.FocusArea,
                Description = model.Description,
                WeeklyStudyGoalHours = model.WeeklyStudyGoalHours,
                MilestoneDate = model.MilestoneDate,
                UserId = GetUserId()
            };

            _context.LanguageTracks.Add(track);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Language track created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Language Track";

            var track = await _context.LanguageTracks
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == GetUserId());

            if (track == null)
            {
                return NotFound();
            }

            var model = new LanguageTrackFormViewModel
            {
                Id = track.Id,
                Name = track.Name,
                NativeLanguage = track.NativeLanguage,
                CurrentLevel = track.CurrentLevel,
                GoalLevel = track.GoalLevel,
                FocusArea = track.FocusArea,
                Description = track.Description,
                WeeklyStudyGoalHours = track.WeeklyStudyGoalHours,
                MilestoneDate = track.MilestoneDate
            };

            PopulateFormOptions(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LanguageTrackFormViewModel model)
        {
            ViewData["Title"] = "Edit Language Track";

            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                PopulateFormOptions(model);
                return View(model);
            }

            var track = await _context.LanguageTracks
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == GetUserId());

            if (track == null)
            {
                return NotFound();
            }

            track.Name = model.Name;
            track.NativeLanguage = model.NativeLanguage;
            track.CurrentLevel = model.CurrentLevel;
            track.GoalLevel = model.GoalLevel;
            track.FocusArea = model.FocusArea;
            track.Description = model.Description;
            track.WeeklyStudyGoalHours = model.WeeklyStudyGoalHours;
            track.MilestoneDate = model.MilestoneDate;

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Language track updated.";
            return RedirectToAction(nameof(Details), new { id = track.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ViewData["Title"] = "Delete Language Track";

            var track = await _context.LanguageTracks
                .AsNoTracking()
                .Where(item => item.Id == id && item.UserId == GetUserId())
                .Select(item => new LanguageTrackDetailsViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    CurrentLevel = item.CurrentLevel,
                    GoalLevel = item.GoalLevel,
                    FocusArea = item.FocusArea,
                    Description = item.Description,
                    WeeklyStudyGoalHours = item.WeeklyStudyGoalHours,
                    MilestoneDate = item.MilestoneDate,
                    LessonCount = item.Lessons.Count(),
                    PracticeLogCount = item.Lessons.SelectMany(lesson => lesson.PracticeLogs).Count()
                })
                .FirstOrDefaultAsync();

            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var track = await _context.LanguageTracks
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == GetUserId());

            if (track == null)
            {
                return NotFound();
            }

            _context.LanguageTracks.Remove(track);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Language track deleted.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<LanguageTrackListItemViewModel> BuildTrackQuery(string userId)
        {
            return _context.LanguageTracks
                .AsNoTracking()
                .Where(track => track.UserId == userId)
                .Select(track => new LanguageTrackListItemViewModel
                {
                    Id = track.Id,
                    Name = track.Name,
                    CurrentLevel = track.CurrentLevel,
                    GoalLevel = track.GoalLevel,
                    FocusArea = track.FocusArea,
                    WeeklyStudyGoalHours = track.WeeklyStudyGoalHours,
                    MilestoneDate = track.MilestoneDate,
                    LessonCount = track.Lessons.Count(),
                    CompletedLessons = track.Lessons.Count(lesson => lesson.IsCompleted),
                    PracticeMinutes = track.Lessons.SelectMany(lesson => lesson.PracticeLogs).Sum(log => (int?)log.DurationMinutes) ?? 0
                });
        }

        private void PopulateFormOptions(LanguageTrackFormViewModel? model = null)
        {
            ViewBag.LevelOptions = new SelectList(LevelOptions, model?.CurrentLevel);
            ViewBag.GoalLevelOptions = new SelectList(LevelOptions, model?.GoalLevel);
            ViewBag.FocusAreaOptions = new SelectList(FocusAreas, model?.FocusArea);
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}

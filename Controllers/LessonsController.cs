using System.Security.Claims;
using LinguaJourney.Data;
using LinguaJourney.Models;
using LinguaJourney.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LinguaJourney.Controllers
{
    [Authorize]
    public class LessonsController : Controller
    {
        private static readonly string[] SkillAreas =
        {
            "Speaking",
            "Listening",
            "Reading",
            "Writing",
            "Grammar",
            "Vocabulary",
            "Pronunciation"
        };

        private static readonly string[] CefrLevels =
        {
            "A1",
            "A2",
            "B1",
            "B2",
            "C1",
            "C2"
        };

        private readonly ApplicationDbContext _context;

        public LessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? languageTrackId, string? skillArea)
        {
            ViewData["Title"] = "Lessons";
            ViewData["SectionText"] = "Break each language goal into lesson-sized units with one clear task.";

            await PopulateFormOptions(languageTrackId: languageTrackId, skillArea: skillArea);

            var query = BuildLessonQuery(GetUserId());

            if (languageTrackId.HasValue)
            {
                query = query.Where(lesson => lesson.LanguageTrackId == languageTrackId.Value);
            }

            if (!string.IsNullOrWhiteSpace(skillArea))
            {
                query = query.Where(lesson => lesson.SkillArea == skillArea);
            }

            return View(await query.OrderBy(lesson => lesson.ScheduledDate).ToListAsync());
        }

        public async Task<IActionResult> MyLessons()
        {
            ViewData["Title"] = "Weekly Study Board";
            ViewData["SectionText"] = "Upcoming lessons that keep your momentum moving.";

            var lessons = await BuildLessonQuery(GetUserId())
                .Where(lesson => !lesson.IsCompleted)
                .OrderBy(lesson => lesson.ScheduledDate)
                .ToListAsync();

            return View(lessons);
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "Lesson Details";

            var lesson = await _context.Lessons
                .AsNoTracking()
                .Where(item => item.Id == id && item.UserId == GetUserId())
                .Select(item => new LessonDetailsViewModel
                {
                    Id = item.Id,
                    LanguageTrackId = item.LanguageTrackId,
                    TrackName = item.LanguageTrack.Name,
                    Title = item.Title,
                    SkillArea = item.SkillArea,
                    CefrLevel = item.CefrLevel,
                    VocabularyTheme = item.VocabularyTheme,
                    Summary = item.Summary,
                    PracticeTask = item.PracticeTask,
                    EstimatedMinutes = item.EstimatedMinutes,
                    ScheduledDate = item.ScheduledDate,
                    IsCompleted = item.IsCompleted,
                    CreatedDate = item.CreatedDate,
                    PracticeLogs = item.PracticeLogs
                        .OrderByDescending(log => log.PracticedOn)
                        .Select(log => new LessonPracticePreviewViewModel
                        {
                            Id = log.Id,
                            SessionTitle = log.SessionTitle,
                            ActivityType = log.ActivityType,
                            PracticedOn = log.PracticedOn,
                            AccuracyScore = log.AccuracyScore
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (lesson == null)
            {
                return NotFound();
            }

            ViewBag.PracticeCount = lesson.PracticeLogs.Count();
            return View(lesson);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? languageTrackId)
        {
            ViewData["Title"] = "Create Lesson";
            var selectedTrackId = languageTrackId.HasValue && await UserOwnsTrackAsync(languageTrackId.Value)
                ? languageTrackId
                : null;

            await PopulateFormOptions(languageTrackId: selectedTrackId);
            return View(new LessonFormViewModel
            {
                LanguageTrackId = selectedTrackId ?? 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonFormViewModel model)
        {
            ViewData["Title"] = "Create Lesson";

            if (!await UserOwnsTrackAsync(model.LanguageTrackId))
            {
                ModelState.AddModelError(nameof(model.LanguageTrackId), "Choose one of your language tracks.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateFormOptions(model.LanguageTrackId, model.SkillArea, model.CefrLevel);
                return View(model);
            }

            var lesson = new Lesson
            {
                LanguageTrackId = model.LanguageTrackId,
                Title = model.Title,
                SkillArea = model.SkillArea,
                CefrLevel = model.CefrLevel,
                VocabularyTheme = model.VocabularyTheme,
                Summary = model.Summary,
                PracticeTask = model.PracticeTask,
                EstimatedMinutes = model.EstimatedMinutes,
                ScheduledDate = model.ScheduledDate,
                IsCompleted = model.IsCompleted,
                UserId = GetUserId()
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Lesson created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Lesson";

            var lesson = await _context.Lessons
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == GetUserId());

            if (lesson == null)
            {
                return NotFound();
            }

            var model = new LessonFormViewModel
            {
                Id = lesson.Id,
                LanguageTrackId = lesson.LanguageTrackId,
                Title = lesson.Title,
                SkillArea = lesson.SkillArea,
                CefrLevel = lesson.CefrLevel,
                VocabularyTheme = lesson.VocabularyTheme,
                Summary = lesson.Summary,
                PracticeTask = lesson.PracticeTask,
                EstimatedMinutes = lesson.EstimatedMinutes,
                ScheduledDate = lesson.ScheduledDate,
                IsCompleted = lesson.IsCompleted
            };

            await PopulateFormOptions(model.LanguageTrackId, model.SkillArea, model.CefrLevel);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LessonFormViewModel model)
        {
            ViewData["Title"] = "Edit Lesson";

            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!await UserOwnsTrackAsync(model.LanguageTrackId))
            {
                ModelState.AddModelError(nameof(model.LanguageTrackId), "Choose one of your language tracks.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateFormOptions(model.LanguageTrackId, model.SkillArea, model.CefrLevel);
                return View(model);
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == GetUserId());

            if (lesson == null)
            {
                return NotFound();
            }

            lesson.LanguageTrackId = model.LanguageTrackId;
            lesson.Title = model.Title;
            lesson.SkillArea = model.SkillArea;
            lesson.CefrLevel = model.CefrLevel;
            lesson.VocabularyTheme = model.VocabularyTheme;
            lesson.Summary = model.Summary;
            lesson.PracticeTask = model.PracticeTask;
            lesson.EstimatedMinutes = model.EstimatedMinutes;
            lesson.ScheduledDate = model.ScheduledDate;
            lesson.IsCompleted = model.IsCompleted;

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Lesson updated.";
            return RedirectToAction(nameof(Details), new { id = lesson.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ViewData["Title"] = "Delete Lesson";

            var lesson = await _context.Lessons
                .AsNoTracking()
                .Where(item => item.Id == id && item.UserId == GetUserId())
                .Select(item => new LessonDetailsViewModel
                {
                    Id = item.Id,
                    LanguageTrackId = item.LanguageTrackId,
                    TrackName = item.LanguageTrack.Name,
                    Title = item.Title,
                    SkillArea = item.SkillArea,
                    CefrLevel = item.CefrLevel,
                    VocabularyTheme = item.VocabularyTheme,
                    Summary = item.Summary,
                    PracticeTask = item.PracticeTask,
                    EstimatedMinutes = item.EstimatedMinutes,
                    ScheduledDate = item.ScheduledDate,
                    IsCompleted = item.IsCompleted
                })
                .FirstOrDefaultAsync();

            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == GetUserId());

            if (lesson == null)
            {
                return NotFound();
            }

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Lesson deleted.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<LessonListItemViewModel> BuildLessonQuery(string userId)
        {
            return _context.Lessons
                .AsNoTracking()
                .Where(lesson => lesson.UserId == userId)
                .Select(lesson => new LessonListItemViewModel
                {
                    Id = lesson.Id,
                    LanguageTrackId = lesson.LanguageTrackId,
                    TrackName = lesson.LanguageTrack.Name,
                    Title = lesson.Title,
                    SkillArea = lesson.SkillArea,
                    CefrLevel = lesson.CefrLevel,
                    VocabularyTheme = lesson.VocabularyTheme,
                    EstimatedMinutes = lesson.EstimatedMinutes,
                    ScheduledDate = lesson.ScheduledDate,
                    IsCompleted = lesson.IsCompleted,
                    PracticeLogCount = lesson.PracticeLogs.Count()
                });
        }

        private async Task PopulateFormOptions(int? languageTrackId = null, string? skillArea = null, string? cefrLevel = null)
        {
            var userId = GetUserId();
            var trackOptions = await _context.LanguageTracks
                .AsNoTracking()
                .Where(track => track.UserId == userId)
                .OrderBy(track => track.Name)
                .Select(track => new
                {
                    track.Id,
                    Label = $"{track.Name} ({track.GoalLevel})"
                })
                .ToListAsync();

            ViewBag.TrackOptions = new SelectList(trackOptions, "Id", "Label", languageTrackId);
            ViewBag.SkillOptions = new SelectList(SkillAreas, skillArea);
            ViewBag.CefrOptions = new SelectList(CefrLevels, cefrLevel);
        }

        private async Task<bool> UserOwnsTrackAsync(int trackId)
        {
            return await _context.LanguageTracks.AnyAsync(track => track.Id == trackId && track.UserId == GetUserId());
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}

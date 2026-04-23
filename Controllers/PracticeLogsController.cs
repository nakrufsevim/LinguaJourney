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
    public class PracticeLogsController : Controller
    {
        private static readonly string[] ActivityTypes =
        {
            "Speaking Drill",
            "Listening Review",
            "Reading Sprint",
            "Writing Practice",
            "Pronunciation Session",
            "Flashcard Recall",
            "Conversation Shadowing"
        };

        private readonly ApplicationDbContext _context;

        public PracticeLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? activityType)
        {
            ViewData["Title"] = "Practice Logs";
            ViewData["SectionText"] = "Capture what you practiced, what you learned, and when to review it next.";
            ViewBag.ActivityType = activityType;
            ViewBag.ActivityOptions = new SelectList(ActivityTypes, activityType);

            var query = BuildPracticeLogQuery(GetUserId());
            if (!string.IsNullOrWhiteSpace(activityType))
            {
                query = query.Where(log => log.ActivityType == activityType);
            }

            return View(await query.OrderByDescending(log => log.PracticedOn).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "Practice Log Details";

            var log = await _context.PracticeLogs
                .AsNoTracking()
                .Where(item => item.Id == id && item.UserId == GetUserId())
                .Select(item => new PracticeLogDetailsViewModel
                {
                    Id = item.Id,
                    LessonId = item.LessonId,
                    LessonTitle = item.Lesson.Title,
                    TrackName = item.Lesson.LanguageTrack.Name,
                    SessionTitle = item.SessionTitle,
                    ActivityType = item.ActivityType,
                    NewExpression = item.NewExpression,
                    Reflection = item.Reflection,
                    AccuracyScore = item.AccuracyScore,
                    DurationMinutes = item.DurationMinutes,
                    PracticedOn = item.PracticedOn,
                    NextReviewDate = item.NextReviewDate,
                    CreatedDate = item.CreatedDate
                })
                .FirstOrDefaultAsync();

            if (log == null)
            {
                return NotFound();
            }

            ViewBag.ReviewWindow = Math.Max((log.NextReviewDate.Date - DateTime.Today).Days, 0);
            return View(log);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? lessonId)
        {
            ViewData["Title"] = "Log Practice Session";
            var selectedLessonId = lessonId.HasValue && await UserOwnsLessonAsync(lessonId.Value)
                ? lessonId
                : null;

            await PopulateFormOptions(selectedLessonId);
            return View(new PracticeLogFormViewModel
            {
                LessonId = selectedLessonId ?? 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PracticeLogFormViewModel model)
        {
            ViewData["Title"] = "Log Practice Session";

            if (!await UserOwnsLessonAsync(model.LessonId))
            {
                ModelState.AddModelError(nameof(model.LessonId), "Choose one of your lessons.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateFormOptions(model.LessonId, model.ActivityType);
                return View(model);
            }

            var log = new PracticeLog
            {
                LessonId = model.LessonId,
                SessionTitle = model.SessionTitle,
                ActivityType = model.ActivityType,
                NewExpression = model.NewExpression,
                Reflection = model.Reflection,
                AccuracyScore = model.AccuracyScore,
                DurationMinutes = model.DurationMinutes,
                PracticedOn = model.PracticedOn,
                NextReviewDate = model.NextReviewDate,
                UserId = GetUserId()
            };

            _context.PracticeLogs.Add(log);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Practice session saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Practice Log";

            var log = await _context.PracticeLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == GetUserId());

            if (log == null)
            {
                return NotFound();
            }

            var model = new PracticeLogFormViewModel
            {
                Id = log.Id,
                LessonId = log.LessonId,
                SessionTitle = log.SessionTitle,
                ActivityType = log.ActivityType,
                NewExpression = log.NewExpression,
                Reflection = log.Reflection,
                AccuracyScore = log.AccuracyScore,
                DurationMinutes = log.DurationMinutes,
                PracticedOn = log.PracticedOn,
                NextReviewDate = log.NextReviewDate
            };

            await PopulateFormOptions(model.LessonId, model.ActivityType);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PracticeLogFormViewModel model)
        {
            ViewData["Title"] = "Edit Practice Log";

            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!await UserOwnsLessonAsync(model.LessonId))
            {
                ModelState.AddModelError(nameof(model.LessonId), "Choose one of your lessons.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateFormOptions(model.LessonId, model.ActivityType);
                return View(model);
            }

            var log = await _context.PracticeLogs
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == GetUserId());

            if (log == null)
            {
                return NotFound();
            }

            log.LessonId = model.LessonId;
            log.SessionTitle = model.SessionTitle;
            log.ActivityType = model.ActivityType;
            log.NewExpression = model.NewExpression;
            log.Reflection = model.Reflection;
            log.AccuracyScore = model.AccuracyScore;
            log.DurationMinutes = model.DurationMinutes;
            log.PracticedOn = model.PracticedOn;
            log.NextReviewDate = model.NextReviewDate;

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Practice log updated.";
            return RedirectToAction(nameof(Details), new { id = log.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ViewData["Title"] = "Delete Practice Log";

            var log = await _context.PracticeLogs
                .AsNoTracking()
                .Where(item => item.Id == id && item.UserId == GetUserId())
                .Select(item => new PracticeLogDetailsViewModel
                {
                    Id = item.Id,
                    LessonId = item.LessonId,
                    LessonTitle = item.Lesson.Title,
                    TrackName = item.Lesson.LanguageTrack.Name,
                    SessionTitle = item.SessionTitle,
                    ActivityType = item.ActivityType,
                    NewExpression = item.NewExpression,
                    Reflection = item.Reflection,
                    AccuracyScore = item.AccuracyScore,
                    DurationMinutes = item.DurationMinutes,
                    PracticedOn = item.PracticedOn,
                    NextReviewDate = item.NextReviewDate
                })
                .FirstOrDefaultAsync();

            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var log = await _context.PracticeLogs
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == GetUserId());

            if (log == null)
            {
                return NotFound();
            }

            _context.PracticeLogs.Remove(log);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Practice log deleted.";
            return RedirectToAction(nameof(Index));
        }

        private IQueryable<PracticeLogListItemViewModel> BuildPracticeLogQuery(string userId)
        {
            return _context.PracticeLogs
                .AsNoTracking()
                .Where(log => log.UserId == userId)
                .Select(log => new PracticeLogListItemViewModel
                {
                    Id = log.Id,
                    LessonId = log.LessonId,
                    LessonTitle = log.Lesson.Title,
                    TrackName = log.Lesson.LanguageTrack.Name,
                    SessionTitle = log.SessionTitle,
                    ActivityType = log.ActivityType,
                    AccuracyScore = log.AccuracyScore,
                    DurationMinutes = log.DurationMinutes,
                    PracticedOn = log.PracticedOn
                });
        }

        private async Task PopulateFormOptions(int? lessonId = null, string? activityType = null)
        {
            var lessonOptions = await _context.Lessons
                .AsNoTracking()
                .Where(lesson => lesson.UserId == GetUserId())
                .OrderBy(lesson => lesson.ScheduledDate)
                .Select(lesson => new
                {
                    lesson.Id,
                    Label = $"{lesson.Title} ({lesson.LanguageTrack.Name})"
                })
                .ToListAsync();

            ViewBag.LessonOptions = new SelectList(lessonOptions, "Id", "Label", lessonId);
            ViewBag.ActivityOptions = new SelectList(ActivityTypes, activityType);
        }

        private async Task<bool> UserOwnsLessonAsync(int lessonId)
        {
            return await _context.Lessons.AnyAsync(lesson => lesson.Id == lessonId && lesson.UserId == GetUserId());
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}

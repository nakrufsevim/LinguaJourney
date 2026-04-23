using CourseNotesSharing.Data;
using CourseNotesSharing.Models;
using CourseNotesSharing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseNotesSharing.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Create Account";
            ViewBag.AuthLead = "Build a language routine with structured tracks, lessons, and review sessions.";
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["Title"] = "Create Account";
            ViewBag.AuthLead = "Build a language routine with structured tracks, lessons, and review sessions.";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                NativeLanguage = model.NativeLanguage,
                LearningMotto = model.LearningMotto
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["StatusMessage"] = $"Welcome to LinguaJourney, {user.FirstName}.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            ViewBag.AuthLead = "Use your username and password to open your study dashboard.";
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["Title"] = "Login";
            ViewBag.AuthLead = "Use your username and password to open your study dashboard.";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "You are back in your learning workspace.";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Username or password is incorrect.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["StatusMessage"] = "You have been signed out.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            ViewData["Title"] = "My Profile";

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NativeLanguage = user.NativeLanguage,
                LearningMotto = user.LearningMotto,
                ProfilePicture = user.ProfilePicture,
                JoinDate = user.JoinDate,
                LanguageTrackCount = await _context.LanguageTracks.CountAsync(track => track.UserId == user.Id),
                LessonCount = await _context.Lessons.CountAsync(lesson => lesson.UserId == user.Id),
                PracticeLogCount = await _context.PracticeLogs.CountAsync(log => log.UserId == user.Id),
                LastPracticeDate = await _context.PracticeLogs
                    .Where(log => log.UserId == user.Id)
                    .OrderByDescending(log => log.PracticedOn)
                    .Select(log => (DateTime?)log.PracticedOn)
                    .FirstOrDefaultAsync()
            };

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            ViewData["Title"] = "Edit Profile";

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View(new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NativeLanguage = user.NativeLanguage,
                LearningMotto = user.LearningMotto,
                ProfilePicture = user.ProfilePicture,
                JoinDate = user.JoinDate
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            ViewData["Title"] = "Edit Profile";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(user.ProfilePicture))
                {
                    var previousPath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePicture.TrimStart('/'));
                    if (System.IO.File.Exists(previousPath))
                    {
                        System.IO.File.Delete(previousPath);
                    }
                }

                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile-pictures");
                Directory.CreateDirectory(uploadFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProfilePictureFile.FileName)}";
                var savePath = Path.Combine(uploadFolder, uniqueFileName);

                await using var fileStream = new FileStream(savePath, FileMode.Create);
                await model.ProfilePictureFile.CopyToAsync(fileStream);

                user.ProfilePicture = $"/uploads/profile-pictures/{uniqueFileName}";
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.NativeLanguage = model.NativeLanguage;
            user.LearningMotto = model.LearningMotto;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            ViewData["Title"] = "Change Password";
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ViewData["Title"] = "Change Password";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Your password has been changed.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Access Denied";
            return View();
        }
    }
}

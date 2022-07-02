using IdentityTesting.Areas.Identity.Pages.Account;
using IdentityTesting.Data;
using IdentityTesting.Models;
using IdentityTesting.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityTesting.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        private readonly ApplicationDbContext _context;


        public AdminsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _context = context;

            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Lecturers()
        {
            var lects = await _userManager.GetUsersInRoleAsync("Lecturer");
            return View(lects);
        }

        public IActionResult CreateLectAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLectAdmin(RegisterLectViewModel model) 
        {
            if(await CheckICExists(model.IC) != null)
            {
                ModelState.AddModelError("", "User with specified IC already exists!");
            }

            if(model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match!");
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IC = model.IC,
                    Matric = model.Matric,
                    DomainID = 1
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Enums.Roles.Lecturer.ToString());
                    return RedirectToAction(nameof(Lecturers));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            //Something failed.
            return View(model);
        }

        public async Task<IActionResult> ToggleCommitee(string? id) // TODO improve visibility
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Lecturers));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(Enums.Roles.Commitee.ToString()))
            {
                await _userManager.RemoveFromRoleAsync(user, Enums.Roles.Commitee.ToString());
            }
            else
            {
                await _userManager.AddToRoleAsync(user, Enums.Roles.Commitee.ToString());
            }
            return RedirectToAction(nameof(Lecturers));
        }

        public async Task<bool> IsCommitee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(Enums.Roles.Commitee.ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ApplicationUser> CheckICExists(string ic)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.IC == ic);
            return user;
        }

        public async Task<IActionResult> EditLecturer(string? id)
        {
            if(id == null || _context.Users == null)
            {
                return NotFound();
            }

            var lect = await _userManager.FindByIdAsync(id);
            EditLectViewModel pageModel = new EditLectViewModel
            {
                Email = lect.Email,
                FirstName = lect.FirstName,
                LastName = lect.LastName,
                IC = lect.IC,
                Matric = lect.Matric
            };


            ViewBag.Lecturer = lect.FirstName;

            if(lect == null)
            {
                return NotFound();
            }
            return View(pageModel);
        }

        [HttpPost,ActionName("EditLecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLect(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lectToUpdate = await _context.Users.FirstOrDefaultAsync(x=> x.Id == id);

            if (await TryUpdateModelAsync<ApplicationUser>(lectToUpdate, "", l => l.Email, l => l.FirstName, l => l.LastName, l => l.IC, l => l.Matric)) 
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Lecturers));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                                            "Try again, and if the problem persists, " +
                                            "see your system administrator.");
                }
            }

            return View(lectToUpdate);
        }



        // GET: Admins/Delete/id
        public async Task<IActionResult> DeleteLecturer(string? id, bool? saveChangesError = false)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var lect = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if(lect == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again, and if the problem persists " + "see your system administrator.";
            }


            return View(lect);
        }

        [HttpPost, ActionName("DeleteLecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLect(string id)
        {
            var lect = await _userManager.FindByIdAsync(id);

            if (lect == null)
            {
                return RedirectToAction(nameof(Lecturers));
            }

            try
            {
                _context.Users.Remove(lect);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Lecturers));
            }
            catch(DbUpdateException)
            {
                return RedirectToAction(nameof(DeleteLecturer), new {id = id, saveChangesError = true});
            }
        }
    }
}

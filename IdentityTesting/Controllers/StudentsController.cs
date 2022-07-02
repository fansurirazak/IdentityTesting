using IdentityTesting.Areas.Identity.Pages.Account;
using IdentityTesting.Data;
using IdentityTesting.Models;
using IdentityTesting.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace IdentityTesting.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentsController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IFileProvider _fileProvider;

        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IFileProvider provider)
        {
            _context = context;
            _fileProvider = provider;

            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }



        //Select Supervisor
        //Display general agreement
        //Submit project Proposal

        public async Task<IActionResult> CreateProposal()
        {
            // add supervisor
            var listSuper = await _userManager.GetUsersInRoleAsync(Enums.Roles.Lecturer.ToString());
            //
            ViewData["DomainID"] = new SelectList(_context.ACADDomains, "ACADDomainID", "ACADDomainName");
            ViewData["SupervisorID"] = listSuper.Select(x => new SelectListItem { Text = x.FirstName, Value = x.Id }).ToList(); 
            return View();
        }

        [HttpPost,ActionName("CreateProposal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProposalPost(ProjectProp model, IFormFile file)
        {
            model.StudentID = _userManager.GetUserId(User);

            // god in heaven help me on this

            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\uploadfiles",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //from https://www.c-sharpcorner.com/article/upload-download-files-in-asp-net-core-2-0/


            try
            {
                if (ModelState.IsValid)
                {
                    model.FileName = file.FileName;
                    model.FilePath = Path.Combine("\\uploadfiles", file.FileName);
                    // add this with string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                    // file will be accessible with the uri
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to Create Proposal! Please try again!");
            }

            var listSuper = await _userManager.GetUsersInRoleAsync(Enums.Roles.Lecturer.ToString());
            ViewData["DomainID"] = new SelectList(_context.ACADDomains, "ACADDomainID", "ACADDomainName");
            ViewData["SupervisorID"] = listSuper.Select(x => new SelectListItem { Text = x.FirstName, Value = x.Id }).ToList();
            return View(model);
        }



        //View proposal status

        public async Task<IActionResult> Proposals()
        {
            var props = await _context.ProjectProps.Where(x=>x.StudentID== _userManager.GetUserId(User)).ToListAsync();
            return View(props);
        }

        public async Task<IActionResult> ViewProposal(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var prop = await _context.ProjectProps.FirstOrDefaultAsync(x => x.ID == id);

            if(prop == null || prop.StudentID != _userManager.GetUserId(User)) 
            {
                return NotFound(); // no peeking other people's proposals!
            }

            return View(prop);
        }

        public async Task<IActionResult> DeleteProposal(int? id)
        {
            if (id == null || _context.ProjectProps == null)
            {
                return NotFound();
            }

            var proposal = await _context.ProjectProps.AsNoTracking().FirstOrDefaultAsync(pee => pee.ID == id);

            if(proposal == null)
            {
                return NotFound();
            }

            if (proposal.FileName != null || proposal.FilePath != String.Empty)
            {
                var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\uploadfiles",
                        proposal.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    proposal.FilePath = null;
                    proposal.FileName = null;
                    _context.SaveChanges();
                }
            }

            _context.ProjectProps.Remove(proposal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Proposals));
        }
        //resubmit proposal

        public async Task<IActionResult> ResubmitProposal(int? id)
        {
            if (id == null || _context.ProjectProps == null)
            {
                return NotFound();
            }

            var prop = await _context.ProjectProps.FirstOrDefaultAsync(pee=> pee.ID == id);


            if(prop.StudentID != _userManager.GetUserId(User) || prop == null)
            {
                return NotFound();
            }

            if(prop.FileName != null || prop.FilePath != String.Empty)
            {
                var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\uploadfiles",
                        prop.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    prop.FilePath = null;
                    prop.FileName = null;
                    _context.SaveChanges();
                }
            }

            return View(prop);
            //edit proposal deets.
        }

        [HttpPost, ActionName("ResubmitProposal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResubmitProposalPost(int? id, IFormFile file)
        {
            if(id == null)
            {
                return NotFound();
            }

            var propToUpdate = await _context.ProjectProps.FirstOrDefaultAsync(x => x.ID == id);


            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\uploadfiles",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (await TryUpdateModelAsync(propToUpdate, "", l => l.FileName,l=>l.FilePath))
            {
                try
                {
                    propToUpdate.FileName = file.FileName;
                    propToUpdate.FilePath = Path.Combine("\\uploadfiles", file.FileName);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Proposals));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                                            "Try again, and if the problem persists, " +
                                            "see your system administrator.");
                }
            }

            return View(propToUpdate);
        }


        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        }
}

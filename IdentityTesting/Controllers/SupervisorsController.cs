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

namespace IdentityTesting.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class SupervisorsController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;

        private readonly ApplicationDbContext _context;

        public SupervisorsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _context = context;

            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> StudentProposals()
        {
            var stuProps = await _context.ProjectProps.Where(x=>x.SupervisorID== _userManager.GetUserId(User)).ToListAsync();

            var stuProps1 = (from props in _context.ProjectProps
                             join domain in _context.ACADDomains on props.DomainID equals domain.ACADDomainID
                             join student in _context.Users on props.StudentID equals student.Id
                             join supervisor in _context.Users on props.SupervisorID equals supervisor.Id
                             join eva1 in _context.Users on props.Evaluator1ID equals eva1.Id into ev1_prop
                             from ev1 in ev1_prop.DefaultIfEmpty()
                             join eva2 in _context.Users on props.Evaluator2ID equals eva2.Id into ev2_prop
                             from ev2 in ev2_prop.DefaultIfEmpty()
                             where props.SupervisorID == _userManager.GetUserId(User)
                             select new
                             {
                                 projectId = props.ID,
                                 projTitle = props.ProjectTitle,
                                 projDomainID = props.DomainID,
                                 projDomain = domain.ACADDomainName,
                                 projEvalAssess = props.EvalAssess,
                                 projEval1Com = props.EvalComment1,
                                 projEval2Com = props.EvalComment2,
                                 projSupApp = props.SuperApprove,
                                 projSupCom = props.SuperComment,
                                 projPropStat = props.ProposalStatus,
                                 projSem = props.ProjSemester,
                                 projSes = props.ProjSession,
                                 projFileName = props.FileName,
                                 projFilePath = props.FilePath,
                                 projStuId = props.StudentID,
                                 projStuName = student.FirstName,
                                 projSupId = props.SupervisorID,
                                 projSupName = supervisor.FirstName,
                                 projEva1Id = props.Evaluator1ID,
                                 projEva1Name = ev1.FirstName ?? String.Empty,
                                 projEva2Id = props.Evaluator2ID,
                                 projEva2Name = ev2.FirstName ?? String.Empty
                             }
                             ).ToList().Select(proj => new ProjPropDisplayVM()
                             {
                                 ID = proj.projectId,
                                 ProjectTitle = proj.projTitle,
                                 DomainID = proj.projDomainID,
                                 DomainName = proj.projDomain,
                                 EvalAssess = proj.projEvalAssess,
                                 EvalComment1 = proj.projEval1Com,
                                 EvalComment2 = proj.projEval2Com,
                                 SuperApprove = proj.projSupApp,
                                 SuperComment = proj.projSupCom,
                                 ProposalStatus = proj.projPropStat,
                                 ProjSemester = proj.projSem,
                                 ProjSession = proj.projSes,
                                 FileName = proj.projFileName,
                                 FilePath = proj.projFilePath,
                                 StudentID = proj.projStuId,
                                 StudentName = proj.projStuName,
                                 SupervisorID = proj.projSupId,
                                 SupervisorName = proj.projSupName,
                                 Evaluator1ID = proj.projEva1Id,
                                 Evaluator1Name = proj.projEva1Name,
                                 Evaluator2ID = proj.projEva2Id,
                                 Evaluator2Name = proj.projEva2Name
                             });

            return View(stuProps1);
        }

        //view students associated with supervisor

        //view own student proposal
        public async Task<IActionResult> ViewProposal(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(StudentProposals));
            }

            var prop = await _context.ProjectProps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id);

            if (prop == null || prop.SupervisorID != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(prop);  //view proposal results and comments after evaluator assessment, link will be unavailable if so
        }

        //and leave comment
        public async Task<IActionResult> SupeComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prop = await _context.ProjectProps
                .FirstOrDefaultAsync(x => x.ID == id);

            if (prop == null)
            {
                return NotFound();
            }

            //check if evaluator already assessed

            return View(prop);
        }

        [HttpPost, ActionName("SupeComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupeCommentPost(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ViewProposal));
            }

            var propToUpdate = await _context.ProjectProps.FirstOrDefaultAsync(x => x.ID == id);

            if (propToUpdate == null || propToUpdate.SupervisorID != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(propToUpdate, "", p => p.SuperApprove, p => p.SuperComment))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(StudentProposals));
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

       

    }
}

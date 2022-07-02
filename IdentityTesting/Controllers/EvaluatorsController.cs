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
    public class EvaluatorsController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;

        private readonly ApplicationDbContext _context;

        public EvaluatorsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
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

        //view all proposals related to this evaluator
        public async Task<IActionResult> ViewEvalProposals()
        {
            var proposals = (from props in _context.ProjectProps
                             join domain in _context.ACADDomains on props.DomainID equals domain.ACADDomainID
                             join student in _context.Users on props.StudentID equals student.Id
                             join supervisor in _context.Users on props.SupervisorID equals supervisor.Id
                             join eva1 in _context.Users on props.Evaluator1ID equals eva1.Id into ev1_prop
                             from ev1 in ev1_prop.DefaultIfEmpty()
                             join eva2 in _context.Users on props.Evaluator2ID equals eva2.Id into ev2_prop
                             from ev2 in ev2_prop.DefaultIfEmpty()
                             where props.Evaluator1ID == _userManager.GetUserId(User) || props.Evaluator2ID == _userManager.GetUserId(User)
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

            //await _context.ProjectProps.Where(p=>p.Evaluator1ID == _userManager.GetUserId(User) || p.Evaluator2ID == _userManager.GetUserId(User) ).ToListAsync();



            if (proposals == null) 
            {
                return RedirectToAction(nameof(Index));
            }
            return View(proposals);
        }

        //Edit proposal
        public async Task<IActionResult> EvalProposal(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var prop = await _context.ProjectProps.FirstOrDefaultAsync(x => x.ID == id);

            var enumData = from ProposalStatus e in Enum.GetValues(typeof(ProposalStatus))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };

            ViewData["ProposalStatus"] = new SelectList(enumData, "ID","Name");

            if (prop == null)
            {
                return NotFound();
            }


            return View(prop);
        }

        [HttpPost, ActionName("EvalProposal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EvalProposalPost(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ViewEvalProposals));
            }

            var prop = await _context.ProjectProps.FirstOrDefaultAsync(x => x.ID == id);

            if (prop == null)
            {
                return RedirectToAction(nameof(ViewEvalProposals));
            }

            if(await TryUpdateModelAsync(prop,"",p => p.EvalAssess, p => p.EvalComment1, p => p.EvalComment2, p=>p.ProposalStatus))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ViewEvalProposals));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                                            "Try again, and if the problem persists, " +
                                            "see your system administrator.");
                }
            }


            var enumData = from ProposalStatus e in Enum.GetValues(typeof(ProposalStatus))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };

            ViewData["ProposalStatus"] = new SelectList(enumData, "ID", "Name");

            return View(prop);
        }
    }
}

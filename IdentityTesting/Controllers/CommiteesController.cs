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
    [Authorize(Roles = "Commitee")]

    public class CommiteesController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;

        private readonly ApplicationDbContext _context;

        public CommiteesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
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

        //list of lecturer

        public async Task<IActionResult> Lecturers()
        {
            var lects = await _userManager.GetUsersInRoleAsync(Enums.Roles.Lecturer.ToString());
            var domain = await _context.ACADDomains.ToListAsync();

            // eternal damnation to the ORM that is known as EF Core!
            var lectsWithDomains = (from user in _context.Users
                                    join userroles in _context.UserRoles
                                    on user.Id equals userroles.UserId
                                    join roles in _context.Roles
                                    on userroles.RoleId equals roles.Id
                                    join domains in _context.ACADDomains
                                    on user.DomainID equals domains.ACADDomainID
                                    select new
                                    {
                                        userId = user.Id,
                                        FirstName = user.FirstName,
                                        LastName = user.LastName,
                                        IC = user.IC,
                                        Matric = user.Matric,
                                        Email = user.Email,
                                        Role = roles.Name,
                                        ACADDomainName = domains.ACADDomainName
                                    }
                                    ).ToList().Select(pee=> new LecturerDomainViewModel()
                                    {
                                        Id = pee.userId,
                                        FirstName = pee.FirstName,
                                        LastName = pee.LastName,
                                        IC = pee.IC,
                                        Matric = pee.Matric,
                                        Email = pee.Email,
                                        Role = pee.Role,
                                        ACADDomainName = pee.ACADDomainName
                                    });

            //var lectsWithDomains = (from user in _context.Users
            //                        join userroles in _context.UserRoles
            //                        on user.Id equals userroles.UserId
            //                        join roles in _context.Roles
            //                        on userroles.RoleId equals roles.Id
            //                        join domains in _context.ACADDomains
            //                        on user.DomainID equals domains.ACADDomainID
            //                        select new
            //                        {
            //                            userId = user.Id,
            //                            FirstName = user.FirstName,
            //                            LastName = user.LastName,
            //                            IC = user.IC,
            //                            Matric = user.Matric,
            //                            Email = user.Email,
            //                            Role = roles.Name,
            //                            ACADDomainName = domains.ACADDomainName
            //                        }
            //                        ).ToList().Select(pee => new LecturerDomainViewModel()
            //                        {
            //                            Id = pee.userId,
            //                            FirstName = pee.FirstName,
            //                            LastName = pee.LastName,
            //                            IC = pee.IC,
            //                            Matric = pee.Matric,
            //                            Email = pee.Email,
            //                            Role = pee.Role,
            //                            ACADDomainName = pee.ACADDomainName
            //                        });

            return View(lectsWithDomains);
        }

        public async Task<IActionResult> AssignDomain(string? id) //Assign domain here
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var lect = await _userManager.FindByIdAsync(id);

            if (lect == null)
            {
                return NotFound(); // redirect if lecturer exists
            }
            ViewData["DomainID"] = new SelectList(_context.ACADDomains, "ACADDomainID", "ACADDomainName", lect.DomainID);
            return View(lect);

        }


        [HttpPost, ActionName("AssignDomain")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifyDomain(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lectToUpdate = await _context.Users.Include(i=> i.Domain).FirstOrDefaultAsync(x => x.Id == id);
            if (await TryUpdateModelAsync<ApplicationUser>(lectToUpdate,"",l => l.DomainID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Lecturers));
                }catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to Change Domain!");
                }
            }
            ViewData["DomainID"] = new SelectList(_context.ACADDomains, "ACADDomainID", "ACADDomainName", lectToUpdate.DomainID);
            return View(lectToUpdate);
        }


        //list of student
        public async Task<IActionResult> Students()
        {
            var studs = await _userManager.GetUsersInRoleAsync(Enums.Roles.Student.ToString());
            return View(studs);
        }

        //list of proposals
        public async Task<IActionResult> Proposals()
        {
            //var Oldprops = await _context.ProjectProps.Include(x=>x.Student).AsNoTracking().ToListAsync();

            var allProps = (from props in _context.ProjectProps
                             join domain in _context.ACADDomains on props.DomainID equals domain.ACADDomainID
                             join student in _context.Users on props.StudentID equals student.Id
                             join supervisor in _context.Users on props.SupervisorID equals supervisor.Id
                             join eva1 in _context.Users on props.Evaluator1ID equals eva1.Id into ev1_prop
                             from ev1 in ev1_prop.DefaultIfEmpty()
                             join eva2 in _context.Users on props.Evaluator2ID equals eva2.Id into ev2_prop
                             from ev2 in ev2_prop.DefaultIfEmpty()
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

            return View(allProps);
        }

        //view proposal details
        //GET: Commitees/ViewProposal/1
        public async Task<IActionResult> ViewProposal(int? id)
        {
            if(id == null)
            {
                return RedirectToAction(nameof(Proposals));
            }

            //var Oldprop = await _context.ProjectProps
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(x => x.ID == id);

            var allProps = (from props in _context.ProjectProps
                            join domain in _context.ACADDomains on props.DomainID equals domain.ACADDomainID
                            join student in _context.Users on props.StudentID equals student.Id
                            join supervisor in _context.Users on props.SupervisorID equals supervisor.Id
                            join eva1 in _context.Users on props.Evaluator1ID equals eva1.Id into ev1_prop
                            from ev1 in ev1_prop.DefaultIfEmpty()
                            join eva2 in _context.Users on props.Evaluator2ID equals eva2.Id into ev2_prop
                            from ev2 in ev2_prop.DefaultIfEmpty()
                            where props.ID == id
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

            if (allProps == null)
            {
                return NotFound();
            }

            return View(allProps);
        }

        //assign evaluators
        public async Task<IActionResult> AssignEvaluator(int? id) //accept proposal id
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Proposals));
            }

            var prop = await _context.ProjectProps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id);

            if (prop == null)
            {
                return NotFound();
            }

            var listLect = await _userManager.GetUsersInRoleAsync(Enums.Roles.Lecturer.ToString());

            listLect = listLect.Where(a=>a.DomainID == prop.DomainID).ToList();
            if (listLect == null)
            {
                return NotFound();
            }

            ViewData["Evaluator1ID"] = listLect.Select(x => new SelectListItem { Text = x.FirstName, Value = x.Id }).ToList();
            ViewData["Evaluator2ID"] = listLect.Select(x => new SelectListItem { Text = x.FirstName, Value = x.Id }).ToList();

            

            return View(prop);
        }

        [HttpPost,ActionName("AssignEvaluator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEvaluatorPost(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Proposals));
            }

            var propToUpdate = await _context.ProjectProps.FirstOrDefaultAsync(x => x.ID == id);

            if (propToUpdate == null)
            {
                return NotFound();
            }

            if (Request.Form["Evaluator1ID"] == propToUpdate.SupervisorID || Request.Form["Evaluator2ID"] == propToUpdate.SupervisorID)
            {
                ModelState.AddModelError("", "Cannot have supervisor as evaluator!");
            }


            if (Request.Form["Evaluator1ID"] == Request.Form["Evaluator2ID"])
            {
                ModelState.AddModelError("", "Cannot have the same evaluators!");
            }


            ////propToUpdate.Evaluator1ID = model.Evaluator1ID;
            ////propToUpdate.Evaluator2ID = model.Evaluator2ID;
            if (ModelState.IsValid)
            {

                var tryRun = await TryUpdateModelAsync(propToUpdate, "", p => p.Evaluator1ID, p => p.Evaluator2ID);
                if(tryRun)
                {
                    try
                    {
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
            }
            
            var listLect = await _userManager.GetUsersInRoleAsync(Enums.Roles.Lecturer.ToString());
            ViewData["Evaluator1ID"] = listLect.Select(x => new SelectListItem { Text = x.FirstName, Value = x.Id }).ToList();
            ViewData["Evaluator2ID"] = listLect.Select(x => new SelectListItem { Text = x.FirstName, Value = x.Id }).ToList();
            //ModelState.AddModelError("", "Some error occured");
            return View(propToUpdate);
        }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityTesting.Models.ViewModels
{
    public class ProjPropViewModel
    {
        public IEnumerable<ApplicationUser>? Students { get; set; }
        public IEnumerable<ProjectProp>? Proposals { get; set; }
        public IEnumerable<ApplicationUser>? Supervisors { get; set; }
        public IEnumerable<ApplicationUser>? Evaluators { get; set; }
    }

    public class StuProjPropViewModel
    {
        public ApplicationUser? Student { get; set; }
        public IEnumerable<ProjectProp>? Proposals { get; set; }
        public IEnumerable<ApplicationUser>? Supervisors { get; set; }
        public IEnumerable<ApplicationUser>? Evaluators { get; set; }
    }

    public class ProjPropDisplayVM
    {
        public int ID { get; set; }

        public string ProjectTitle { get; set; } = null!;
        //[DisplayName("Project Domain")]
        //[DisplayFormat(NullDisplayText = "No Type Set")]
        //public ACADDomain? ProjectType { get; set; }
        [DisplayName("Domain Name")]
        public int DomainID { get; set; }
        public string? DomainName { get; set; }
        [DisplayName("Evaluator Assessment")]
        public bool EvalAssess { get; set; } = false;
        [DisplayName("First Evaluator Comment")]
        public string EvalComment1 { get; set; } = string.Empty;
        [DisplayName("Second Evaluator Comment")]
        public string EvalComment2 { get; set; } = string.Empty;
        [DisplayName("Supervisor Approval")]
        public bool SuperApprove { get; set; } = false;
        [DisplayName("Supervisor Comment")]
        public string SuperComment { get; set; } = string.Empty;
        [DisplayName("Proposal Status")]
        public ProposalStatus ProposalStatus { get; set; } = ProposalStatus.NotChecked;

        [DisplayName("Semester of Project")]
        public string? ProjSemester { get; set; }
        [DisplayName("Session of Project")]
        public string? ProjSession { get; set; }

        [DisplayName("File Name")]
        public string? FileName { get; set; }
        public string? FilePath { get; set; }


        //public ApplicationUser? Student { get; set; }
        [DisplayName("Student")]
        public string? StudentID { get; set; }
        [DisplayName("Student Name")]
        public string? StudentName  { get; set; }

        public ApplicationUser? Supervisor { get; set; }
        [DisplayName("Supervisor")]
        public string? SupervisorID { get; set; }
        [DisplayName("Supervisor Name")]
        public string? SupervisorName { get; set; }


        public ApplicationUser? Evaluator1 { get; set; }
        [DisplayName("Evaluator 1")]
        public string? Evaluator1ID { get; set; }
        [DisplayName("Evaluator 1 Name")]
        public string? Evaluator1Name { get; set; }


        public ApplicationUser? Evaluator2 { get; set; }
        [DisplayName("Evaluator 2")]
        public string? Evaluator2ID { get; set; }
        [DisplayName("Evaluator 2 Name")]
        public string? Evaluator2Name { get; set; }

    }
}

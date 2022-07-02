using IdentityTesting.Models;
using Microsoft.AspNetCore.Identity;
using IdentityTesting.Enums;

namespace IdentityTesting.Data
{
    public class ContextSeed
    {


        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Commitee.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Lecturer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Student.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Supervisor.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Evaluator.ToString()));

            //Admin,
            //Commitee,
            //ResearchLect, --irrelevant
            //DevelopLect,
            //Student,
            //Supervisor,
            //Evaluator
        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "superadmin@gmail.com",
                Email = "superadmin@gmail.com",
                FirstName = "Ariff",
                LastName = "Fansuri",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "testPassword12345");
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Commitee.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Lecturer.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Student.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Supervisor.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Evaluator.ToString());
                }

            }
        }

        public static async Task SeedDefaultUsers(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Admin User
            var admin = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FirstName = "Jeggean",
                LastName = "Rajendran",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != admin.Id))
            {
                var user = await userManager.FindByEmailAsync(admin.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(admin, "testPassword12345");
                    await userManager.AddToRoleAsync(admin, Enums.Roles.Admin.ToString());
                }
            }

            //Seed Lecturer User
            var lect = new ApplicationUser
            {
                UserName = "lect@gmail.com",
                Email = "lect@gmail.com",
                FirstName = "Chua",
                LastName = "Chen Wei",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != lect.Id))
            {
                var user1 = await userManager.FindByEmailAsync(lect.Email);
                if (user1 == null)
                {
                    await userManager.CreateAsync(lect, "testPassword12345");
                    await userManager.AddToRoleAsync(lect, Enums.Roles.Lecturer.ToString());
                }
            }

            //Seed Student User
            var student = new ApplicationUser
            {
                UserName = "student@gmail.com",
                Email = "student@gmail.com",
                FirstName = "Chia",
                LastName = "Wei Hong",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != student.Id))
            {
                var user2 = await userManager.FindByEmailAsync(student.Email);
                if (user2 == null)
                {
                    await userManager.CreateAsync(student, "testPassword12345");
                    await userManager.AddToRoleAsync(student, Enums.Roles.Student.ToString());
                }
            }

            var supervisor = new ApplicationUser
            {
                UserName = "supervisor@gmail.com",
                Email = "supervisor@gmail.com",
                FirstName = "Zulhakim",
                LastName = "Zulkarnain",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != supervisor.Id))
            {
                var user3 = await userManager.FindByEmailAsync(supervisor.Email);
                if (user3 == null)
                {
                    await userManager.CreateAsync(supervisor, "testPassword12345");
                    await userManager.AddToRoleAsync(supervisor, Enums.Roles.Lecturer.ToString());
                    await userManager.AddToRoleAsync(supervisor, Enums.Roles.Supervisor.ToString());
                }
            }

            var commitee = new ApplicationUser
            {
                UserName = "commitee@gmail.com",
                Email = "commitee@gmail.com",
                FirstName = "Irfan",
                LastName = "Daniel",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != commitee.Id))
            {
                var user4 = await userManager.FindByEmailAsync(commitee.Email);
                if (user4 == null)
                {
                    await userManager.CreateAsync(commitee, "testPassword12345");
                    await userManager.AddToRoleAsync(commitee, Enums.Roles.Lecturer.ToString());
                    await userManager.AddToRoleAsync(commitee, Enums.Roles.Commitee.ToString());
                }
            }

        }




        public static async Task Initialize(ApplicationDbContext context)
        {
            if (!context.ACADDomains.Any())
            {
                var Domains = new ACADDomain[]
                {
                    new ACADDomain{ ACADDomainName = "Development"},
                    new ACADDomain{ ACADDomainName = "Research"}
                };

                foreach (ACADDomain d in Domains)
                {
                    await context.ACADDomains.AddAsync(d);
                }
                context.SaveChanges();
            }

            if (!context.ACDPrograms.Any())
            {
                var ACADPrograms = new ACDProgram[]
                {
                    new ACDProgram { ACDProgramName = "Data Engineering", ACDProgramCode = "SECP"},
                    new ACDProgram { ACDProgramName = "Software Engineering", ACDProgramCode = "SECD"},
                    new ACDProgram { ACDProgramName = "Network Security", ACDProgramCode = "SECS"}

                };

                foreach(ACDProgram program in ACADPrograms)
                {
                    await context.ACDPrograms.AddAsync(program);
                }
                context.SaveChanges();
            }
        }
    }
}

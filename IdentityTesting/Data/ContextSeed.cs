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
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.ResearchLect.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.DevelopLect.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Student.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Supervisor.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Evaluator.ToString()));

            //Admin,
            //Commitee,
            //ResearchLect,
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
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.DevelopLect.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.ResearchLect.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Student.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Supervisor.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Evaluator.ToString());
                }

            }
        }
    }
}

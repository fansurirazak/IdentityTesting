using IdentityTesting.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityTesting.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ACDProgram> ACDPrograms { get; set; }

        public DbSet<ACADDomain> ACADDomains { get; set; }
        public DbSet<ProjectProp> ProjectProps { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ACDProgram>().ToTable("ACDProgram");
            modelBuilder.Entity<ACADDomain>().ToTable("ACADDomain");
            modelBuilder.Entity<ProjectProp>().ToTable("ProjectProp");




            base.OnModelCreating(modelBuilder);
        }
    }
}
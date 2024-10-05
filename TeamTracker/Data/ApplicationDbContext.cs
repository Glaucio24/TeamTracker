using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using TeamTracker.Models;


namespace TeamTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base OnModelCreating to apply Identity configurations
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationships
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Locations)
                .WithMany(l => l.Employees)
                .UsingEntity(j => j.ToTable("EmployeeLocations"));

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Departments)
                .WithMany(d => d.Employees)
                .UsingEntity(j => j.ToTable("EmployeeDepartments"));

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Locations)
                .WithMany(l => l.Departments)
                .UsingEntity(j => j.ToTable("DepartmentLocations"));



        }
    }
}
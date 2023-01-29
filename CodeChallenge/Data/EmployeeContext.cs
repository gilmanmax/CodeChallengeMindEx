using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    public class EmployeeContext : DbContext
    {

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Compensation> Compensations { get; set; }
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Employee>()
            .Navigation(b => b.Compensation)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Compensation>()
                .HasOne(c => c.Employee)
                .WithOne();
        }
    }
}

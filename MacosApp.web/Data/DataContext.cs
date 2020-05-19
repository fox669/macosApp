using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MacosApp.Web.Data.Entities;
using MacosApp.web.Data.Entities;

namespace MacosApp.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Agenda> Agendas { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Manager> Managers { get; set; }

        public DbSet<Labour> Labours { get; set; }

        public DbSet<LabourType> LabourTypes { get; set; }

        public DbSet<ServiceType> ServiceTypes { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Infrastructure.Persistence
{
    public class FMCTurnosDbContext : DbContext
    {
        public FMCTurnosDbContext(DbContextOptions<FMCTurnosDbContext> options)
            : base(options)
        {

        }

        // DbSets de tus Entidades

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Business> Businesses { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Staff> Staffs { get; set; } = null!;
        public DbSet<SysAdminUser> SysAdminUsers { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Plan> Plans { get; set; } = null!;
        public DbSet<BusinessSubscription> BusinessSubscriptions { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Esto obliga a EF Core a convertir cualquier propiedad de tipo Enum a un string en la base de datos
            configurationBuilder.Properties<Enum>().HaveConversion<string>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------------------------------------------------------
            // CONFIGURACIÓN DE RELACIONES Y CLAVES (Mapeo Básico y Explícito)
            // ------------------------------------------------------------------

            modelBuilder.Entity<User>()
            .ToTable("Users")
            .HasDiscriminator<string>("UserType") // Crea una columna para saber si es Staff o SysAdmin
            .HasValue<Staff>("Staff")
            .HasValue<SysAdminUser>("SysAdmin");

            modelBuilder.Entity<Business>()
                .HasIndex(b => b.Url)
                .IsUnique();

            // Relación Business -> Branches (1 a muchos)
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Business)
                .WithMany(bus => bus.Branches)
                .HasForeignKey(b => b.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Business -> Clients (1 a muchos)
            modelBuilder.Entity<Client>()
                .HasOne(c => c.Business)
                .WithMany(bus => bus.Clients)
                .HasForeignKey(c => c.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Business -> Services (1 a muchos)
            modelBuilder.Entity<Service>()
                .HasOne(s => s.Business)
                .WithMany(bus => bus.Services)
                .HasForeignKey(s => s.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Branch -> Staff (1 a muchos)
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Branch)
                .WithMany(b => b.Staff)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Business -> Staff (1 a muchos)
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Business)
                .WithMany()
                .HasForeignKey(s => s.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones de Appointment (Turnos)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Staff)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones de Suscripciones de Negocios
            modelBuilder.Entity<BusinessSubscription>()
                .HasOne(bs => bs.Business)
                .WithMany()
                .HasForeignKey(bs => bs.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusinessSubscription>()
                .HasOne(bs => bs.Plan)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(bs => bs.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
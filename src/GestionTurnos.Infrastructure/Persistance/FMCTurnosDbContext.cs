using GestionTurnos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionTurnos.Infrastructure.Persistance
{
    public class FMCTurnosDbContext : DbContext
    {
        // 🔥 REGLA DE ORO TPH: Agrega el DbSet de la clase base siempre arriba
        public DbSet<User> Users { get; set; }

        // Dejamos los sub-DbSets por comodidad (EF Core ya sabrá que heredan de Users)
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Client> Clients { get; set; }

        public DbSet<Business> Businesses { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<BusinessSubscription> BusinessSubscriptions { get; set; }
        public DbSet<SysAdminUser> SysAdminUsers { get; set; }

        public FMCTurnosDbContext(DbContextOptions<FMCTurnosDbContext> options) : base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<Enum>().HaveConversion<string>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1️⃣ 🔥 CONFIGURA LA HERENCIA PRIMERO QUE NADA
            // Le avisamos a EF Core la jerarquía antes de declarar cualquier clave foránea
            modelBuilder.Entity<Staff>().HasBaseType<User>();
            modelBuilder.Entity<Client>().HasBaseType<User>();

            // 2️⃣ CONFIGURACIÓN DE APPOINTMENTS (Citas/Turnos)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3️⃣ 🔥 RELACIÓN EXPLÍCITA DE CLIENT CON BUSINESS
            // Con esto obligamos a EF Core a usar la propiedad de navegación 'Clients' 
            // que declaramos en Business.cs, asociándola a 'BusinessId'. Chau columnas fantasma.
            modelBuilder.Entity<Client>()
                .HasOne(c => c.Business)
                .WithMany(b => b.Clients) // 👈 Vinculamos con la colección inversa en tu Business.cs
                .HasForeignKey(c => c.BusinessId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
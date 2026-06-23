using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace plp.Shared.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        // DbSet para todas las entidades
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Juegos> Juegos { get; set; }
        public DbSet<HorariosJuegos> HorariosJuegos { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<Comentarios> Comentarios { get; set; }
        public DbSet<ComprasClientes> ComprasClientes { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Contenidos> Contenidos { get; set; }
        public DbSet<Insumos> Insumos { get; set; }
        public DbSet<Promociones> Promociones { get; set; }
        public DbSet<Eventos> Eventos { get; set; }
        public DbSet<TicketsJuegos> TicketsJuegos { get; set; }
        public DbSet<Roles> Roles { get; set; } 


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TicketsJuegos>()
            .HasKey(tj => new { tj.IdJuego, tj.IdTicket });

            // Configuración de la relación muchos a muchos entre Tickets y Juegos a través de TicketsJuegos
            modelBuilder.Entity<Tickets>()
                .HasMany(t => t.TicketsJuegos)
                .WithOne(tj => tj.Ticket)
                .HasForeignKey(tj => tj.IdTicket);

            modelBuilder.Entity<Juegos>()
                .HasMany(j => j.TicketsJuegos)
                .WithOne(tj => tj.Juego)
                .HasForeignKey(tj => tj.IdJuego);

            modelBuilder.Entity<HorariosJuegos>()
                .HasOne(hj => hj.Juego)
                .WithOne(j => j.HorarioJuego)
                .HasForeignKey<HorariosJuegos>(hj => hj.IdJuego);

            // Modificar la tabla de precio de los tickets
            modelBuilder.Entity<Tickets>()
                .Property(t => t.Precio)
                .HasColumnType("decimal(10, 2)"); 

            // Opcional: especifica la acción en cascada si se borra el ticket
            modelBuilder.Entity<ComprasClientes>()
               .HasOne(cc => cc.Ticket)
               .WithMany(t => t.ComprasClientes)
               .HasForeignKey(cc => cc.IdTicket)
               .OnDelete(DeleteBehavior.Cascade); 

            // Para definir que cada usuario tenga un Rol, osea que el rol no sea nulo
            modelBuilder.Entity<Usuarios>()
                .HasOne(u => u.Rol)
                .WithMany()
                .HasForeignKey(u => u.IdRol)
                .IsRequired();


            modelBuilder.Entity<Roles>().HasData(
                new Roles { IdRol = 1, NombreRol = "Administrador" },
                new Roles { IdRol = 2, NombreRol = "Empleado" },
                new Roles { IdRol = 3, NombreRol = "Cliente" }
            );


            base.OnModelCreating(modelBuilder);
        }

        // Clase para facilitar la creación del contexto en tiempo de diseño
        public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
        {
            public MyDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-S5244ST;Initial Catalog=bd_plp3;Integrated Security=True; TrustServerCertificate=True"); 

                return new MyDbContext(optionsBuilder.Options);
            }
        }
    }
}

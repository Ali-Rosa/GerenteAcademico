using GerenteAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GerenteAcademico.Infrastructure.Persistence
{
    public class AcademiaDbContext : DbContext
    {
        public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options)
            : base(options)
        {
        }

        // ========== DbSets ==========
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== CONFIGURACIÓN: ROL ==========
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(r => r.Id);

                entity.HasIndex(r => r.Nombre)
                    .IsUnique()
                    .HasDatabaseName("IX_Roles_Nombre");

                entity.Property(r => r.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.Descripcion)
                    .HasMaxLength(500);

                entity.Property(r => r.UsuarioCreacion)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(r => r.UsuarioModificacion)
                    .HasMaxLength(100);

                entity.HasMany(r => r.Usuarios)
                    .WithOne(u => u.Rol)
                    .HasForeignKey(u => u.RolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== CONFIGURACIÓN: USUARIO ==========
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(u => u.Id);

                entity.HasIndex(u => u.Username)
                    .IsUnique()
                    .HasDatabaseName("IX_Usuarios_Username");

                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Usuarios_Email");

                entity.HasIndex(u => u.Documentacion)
                    .IsUnique()
                    .HasDatabaseName("IX_Usuarios_Documentacion");

                entity.HasIndex(u => u.RolId)
                    .HasDatabaseName("IX_Usuarios_RolId");

                entity.HasIndex(u => u.Activo)
                    .HasDatabaseName("IX_Usuarios_Activo");

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Telefono)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(u => u.Apellido)
                    .HasMaxLength(100);

                entity.Property(u => u.TelefonoEmergencia)
                    .HasMaxLength(20);

                entity.Property(u => u.Documentacion)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.TipoDocumentacion)
                    .HasMaxLength(50);

                entity.Property(u => u.Genero)
                    .HasMaxLength(20);

                entity.Property(u => u.Nacionalidad)
                    .HasMaxLength(100);

                entity.Property(u => u.Direccion)
                    .HasMaxLength(500);

                entity.Property(u => u.FotoUrl)
                    .HasMaxLength(255);

                entity.Property(u => u.UsuarioCreacion)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.UsuarioModificacion)
                    .HasMaxLength(100);

                entity.HasOne(u => u.Rol)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(u => u.RolId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Usuarios_Roles");
            });

            // ========== SEED DATA: ROLES PREDEFINIDOS ==========
            var fechaSeed = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Rol>().HasData(
                new Rol
                {
                    Id = 1,
                    Nombre = "SuperUsuario",
                    Descripcion = "Acceso total al sistema. Puede hacer cualquier cosa.",
                    EsPredefinido = true,
                    Activo = true,
                    FechaCreacion = fechaSeed,
                    UsuarioCreacion = "system"
                },
                new Rol
                {
                    Id = 2,
                    Nombre = "AdminMaster",
                    Descripcion = "Administrador general de la academia.",
                    EsPredefinido = true,
                    Activo = true,
                    FechaCreacion = fechaSeed,
                    UsuarioCreacion = "system"
                },
                new Rol
                {
                    Id = 3,
                    Nombre = "Admin",
                    Descripcion = "Administrador con permisos limitados.",
                    EsPredefinido = true,
                    Activo = true,
                    FechaCreacion = fechaSeed,
                    UsuarioCreacion = "system"
                },
                new Rol
                {
                    Id = 4,
                    Nombre = "Profesor",
                    Descripcion = "Profesor que imparte clases y califica.",
                    EsPredefinido = true,
                    Activo = true,
                    FechaCreacion = fechaSeed,
                    UsuarioCreacion = "system"
                },
                new Rol
                {
                    Id = 5,
                    Nombre = "Estudiante",
                    Descripcion = "Estudiante de la academia.",
                    EsPredefinido = true,
                    Activo = true,
                    FechaCreacion = fechaSeed,
                    UsuarioCreacion = "system"
                },
                new Rol
                {
                    Id = 6,
                    Nombre = "Supervisor",
                    Descripcion = "Supervisor que monitorea actividades.",
                    EsPredefinido = true,
                    Activo = true,
                    FechaCreacion = fechaSeed,
                    UsuarioCreacion = "system"
                }
            );
        }
    }
}

using GerenteAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GerenteAcademico.Infrastructure.Persistence
{
    public class ConfigDbContext : DbContext
    {
        public ConfigDbContext(DbContextOptions<ConfigDbContext> options) : base(options)
        {
        }

        public DbSet<AcademiaConfig> Academias => Set<AcademiaConfig>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AcademiaConfig>(entity =>
            {
                entity.ToTable("Academias");

                entity.HasKey(a => a.Id);

                // Índice único para Código
                entity.HasIndex(a => a.Codigo)
                      .IsUnique();

                // Índice único para IdFiscal
                entity.HasIndex(a => a.IdFiscal)
                      .IsUnique();

                entity.Property(a => a.Codigo)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(a => a.Nombre)
                      .IsRequired()
                      .HasMaxLength(250);

                entity.Property(a => a.LogoUrl)
                      .HasMaxLength(350);

                entity.Property(a => a.CadenaConexionPrincipal)
                      .IsRequired();

                entity.Property(a => a.Descripcion);

                entity.Property(a => a.Direccion)
                      .HasMaxLength(350);

                entity.Property(a => a.Telefono)
                      .HasMaxLength(30);

                entity.Property(a => a.EmailContacto)
                      .HasMaxLength(300);

                entity.Property(a => a.Pais)
                      .HasMaxLength(150);

                entity.Property(a => a.Ciudad)
                      .HasMaxLength(150);

                entity.Property(a => a.IdFiscal)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(a => a.UrlSitioWeb)
                      .HasMaxLength(300);

                entity.Property(a => a.EsDemo)
                      .IsRequired();

                entity.Property(a => a.Activo)
                      .IsRequired();

                entity.Property(a => a.FechaCreacion)
                      .IsRequired();

                entity.Property(a => a.FechaModificacion);

                entity.Property(a => a.UsuarioCreacion)
                      .IsRequired()
                      .HasMaxLength(250);

                entity.Property(a => a.UsuarioModificacion)
                      .HasMaxLength(250);
            });
        }
    }
}

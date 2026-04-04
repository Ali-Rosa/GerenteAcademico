using GerenteAcademico.Application.Services;
using GerenteAcademico.Domain.Interfaces;
using GerenteAcademico.Infrastructure.Persistence;
using GerenteAcademico.Infrastructure.Repositories;
using GerenteAcademico.Infrastructure.Services;
using GerenteAcademico.Web.Components;
using GerenteAcademico.Web.Middleware;
using GerenteAcademico.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace GerenteAcademico
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Lee cadena de conexión desde appsettings.json
            var configConnectionString = builder.Configuration.GetConnectionString("ConfigDatabase");
            // var academiaConnectionString = builder.Configuration.GetConnectionString("AcademiaDatabase");

            // 2. Registrar el DbContext de configuración
            // ConfigDbContext contiene la tabla Academias que incluye CadenaConexionPrincipal
            builder.Services.AddDbContext<ConfigDbContext>(options =>
            {
                options.UseSqlServer(configConnectionString);
            });

            // 2.1 IMPORTANTE: NO registrar AcademiaDbContext de forma estática
            // 
            // RAZÓN: Necesitamos que cada academia use su propia base de datos
            // con la cadena de conexión almacenada en Academias.CadenaConexionPrincipal
            //
            // SOLUCIÓN: Se crea dinámicamente por cada academia usando:
            //   - IAcademiaDbContextFactory (factory pattern)
            //   - IConnectionStringProvider (obtiene cadena desde ConfigDB)
            //   - IConnectionStringValidator (valida la conexión)
            //
            // FLUJO: AuthService → UsuarioRepository → IAcademiaDbContextFactory
            //
            // builder.Services.AddDbContext<AcademiaDbContext>(options =>
            // {
            //     options.UseSqlServer(academiaConnectionString);
            // });

            // 3. Servicios Blazor
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // 4. Controllers + Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 4.1 Autenticación con cookies y JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true; // Seguridad: no accesible desde JavaScript
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
            });

            builder.Services.AddAuthorization();

            // 4.2 Servicios de Validación de Conexión
            builder.Services.AddScoped<IConnectionStringValidator, SqlConnectionValidator>();
            builder.Services.AddScoped<IConnectionStringProvider, ConnectionStringProvider>();
            builder.Services.AddScoped<IAcademiaDbContextFactory, AcademiaDbContextFactory>();

            // Repositorios y servicios
            builder.Services.AddScoped<IAcademiaRepository, AcademiaRepository>(); 
            builder.Services.AddScoped<AcademiaService>();


            // HttpClient con BaseAddress
            builder.Services.AddHttpClient("ServerAPI", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["AppBaseUrl"]!);
            });

            // HttpClient por defecto para Blazor
            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

            // para cargar mi estado de seccion de la academia con sus datos utilizo el patron Singleton
            // para que persista entre componentes y redireccionamientos
            builder.Services.AddSingleton<AcademiaState>();

            // Servicio de autenticación del cliente
            builder.Services.AddScoped<Web.Services.AuthService>();

            // Repositorios y servicios de autenticación
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<Application.Services.AuthService>();
            builder.Services.AddScoped<JwtTokenService>();



            var app = builder.Build();

            // 5. Middleware Global de Excepciones (DEBE SER PRIMERO)
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            // 6. Swagger solo en desarrollo
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 7. Manejo de errores y seguridad
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();
            app.UseAntiforgery();

            // 7.1 Autenticación ANTES de autorización
            app.UseAuthentication();
            app.UseAuthorization();

            // 8. Archivos estáticos
            app.MapStaticAssets();

            // 9. API Controllers
            app.MapControllers();

            // 10. Blazor
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        } 
    }
}
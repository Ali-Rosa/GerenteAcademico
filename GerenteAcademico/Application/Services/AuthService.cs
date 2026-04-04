using GerenteAcademico.Domain.Interfaces;
using GerenteAcademico.Application.Dtos.Auth;
using GerenteAcademico.Infrastructure.Repositories;

namespace GerenteAcademico.Application.Services
{
    /// <summary>
    /// Servicio de autenticación con soporte para múltiples academias.
    /// 
    /// IMPORTANTE: Este servicio implementa autenticación dinámica:
    ///   - Cada academia tiene su propia base de datos de usuarios
    ///   - La cadena de conexión se obtiene dinámicamente desde ConfigDB
    ///   - Los usuarios se buscan en la BD específica de la academia
    /// </summary>
    public class AuthService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly JwtTokenService _jwtTokenService;

        public AuthService(IUsuarioRepository usuarios, JwtTokenService jwtTokenService)
        {
            _usuarios = usuarios;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // IMPORTANTE: Establecer el código de academia ANTES de acceder al repositorio
            // 
            // El repositorio UsuarioRepository necesita conocer de qué academia se está
            // buscando el usuario para crear dinámicamente el DbContext con la BD correcta.
            //
            // Ejemplo:
            //   Si request.AcademiaCodigo = "Konektia"
            //   Se buscará el usuario en la BD de Konektia, no en otras academias
            ((UsuarioRepository)_usuarios).SetAcademiaContext(request.AcademiaCodigo);

            var usuario = await _usuarios.GetByUsernameAsync(request.Username);

            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            if (!usuario.Activo)
                throw new Exception("Usuario inactivo.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            {
                usuario.IntentosFallidos++;
                await _usuarios.UpdateAsync(usuario);
                throw new Exception("Contraseña incorrecta.");
            }

            usuario.IntentosFallidos = 0;
            usuario.UltimoLogin = DateTime.UtcNow;
            await _usuarios.UpdateAsync(usuario);

            // Generar JWT token
            var token = _jwtTokenService.GenerateToken(
                usuarioId: usuario.Id.ToString(),
                nombreUsuario: usuario.Nombre,
                rol: usuario.Rol.Nombre,
                academiaCodigo: request.AcademiaCodigo
            );

            return new LoginResponseDto
            {
                NombreUsuario = usuario.Nombre,
                Rol = usuario.Rol.Nombre,
                Token = token
            };
        }
    }
}


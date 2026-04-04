using GerenteAcademico.Application.Dtos.Auth;
using GerenteAcademico.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GerenteAcademico.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(AuthService auth, JwtTokenService jwtTokenService)
        {
            _auth = auth;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            try
            {
                var result = await _auth.LoginAsync(request);

                // Validar y extraer claims del JWT
                var claimsPrincipal = _jwtTokenService.ValidateToken(result.Token);
                if (claimsPrincipal == null)
                    return BadRequest(new { message = "Token inválido" });

                // Crear cookie de autenticación con los claims del JWT
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    }
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Sesión cerrada correctamente" });
        }

        [HttpGet("check")]
        public ActionResult Check()
        {
            // Verificar si existe una sesión autenticada por cookie
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                return Ok(new
                {
                    authenticated = true,
                    user = new
                    {
                        name = User.FindFirst(ClaimTypes.Name)?.Value,
                        role = User.FindFirst(ClaimTypes.Role)?.Value,
                        academia = User.FindFirst("AcademiaCodigo")?.Value
                    }
                });
            }

            return Unauthorized(new { authenticated = false });
        }
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Business.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerarToken(SesionUsuario sesion)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, sesion.UsuarioId.ToString()),
            new(JwtRegisteredClaimNames.Email, sesion.Correo),
            new(ClaimTypes.Name, sesion.Nombre),
            new("clienteId", sesion.ClienteId.ToString()),
            new("rolId", sesion.RolId.ToString()),
            new(ClaimTypes.Role, sesion.RolId == 2 ? "Administrador" : "Cliente"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"] ?? "60")),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

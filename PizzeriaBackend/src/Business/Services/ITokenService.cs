namespace Business.Services;

public interface ITokenService
{
    string GenerarToken(Models.SesionUsuario sesion);
}

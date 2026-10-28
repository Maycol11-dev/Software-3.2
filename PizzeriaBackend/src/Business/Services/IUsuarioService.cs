using Models;

namespace Business.Services;

public interface IUsuarioService
{
    Task<SesionUsuario?> AutenticarAsync(string correo, string contrasena);
    Task<int> RegistrarAsync(RegistroUsuario registro);
}

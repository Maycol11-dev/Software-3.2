using Business.Validators;
using Data.Repositories;
using Models;

namespace Business.Services;

public class UsuarioService : IUsuarioService
{
    private const int RolClienteId = 1;

    private readonly IUsuarioRepository _usuarios;
    private readonly IClienteRepository _clientes;
    private readonly IPasswordHasher _hasher;

    public UsuarioService(IUsuarioRepository usuarios, IClienteRepository clientes, IPasswordHasher hasher)
    {
        _usuarios = usuarios;
        _clientes = clientes;
        _hasher = hasher;
    }

    public async Task<SesionUsuario?> AutenticarAsync(string correo, string contrasena)
    {
        if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
        {
            return null;
        }

        var usuario = await _usuarios.GetByCorreoAsync(correo.Trim());
        if (usuario is null || !_hasher.Verificar(contrasena, usuario.Contrasena))
        {
            return null;
        }

        var cliente = await _clientes.GetByUsuarioIdAsync(usuario.IdUsuario);

        return new SesionUsuario
        {
            UsuarioId = usuario.IdUsuario,
            ClienteId = cliente?.IdCliente ?? 0,
            Correo = usuario.Correo,
            Nombre = cliente?.Nombre ?? string.Empty,
            Telefono = cliente?.Telefono ?? string.Empty,
            Direccion = cliente?.Direccion ?? string.Empty,
            RolId = usuario.RolId
        };
    }

    public async Task<int> RegistrarAsync(RegistroUsuario registro)
    {
        var correo = registro.Correo.Trim();
        if (await _usuarios.GetByCorreoAsync(correo) is not null)
        {
            throw new InvalidOperationException("Ya existe una cuenta con ese correo.");
        }

        var usuario = new Usuario
        {
            Correo = correo,
            Contrasena = _hasher.Hash(registro.Contrasena),
            RolId = RolClienteId
        };
        var usuarioId = await _usuarios.InsertAsync(usuario);

        var cliente = new Cliente
        {
            UsuarioId = usuarioId,
            Nombre = registro.Nombre.Trim(),
            Telefono = TelefonoHelper.Normalizar(registro.Telefono),
            Direccion = registro.Direccion.Trim()
        };
        await _clientes.InsertAsync(cliente);

        return usuarioId;
    }
}

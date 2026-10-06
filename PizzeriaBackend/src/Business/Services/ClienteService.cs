using Data.Repositories;
using Models;
using System.Text.RegularExpressions;

namespace Business.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repositorio;

    public ClienteService(IClienteRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync() => await _repositorio.GetAllAsync();

    public async Task<Cliente?> GetByIdAsync(int id) => await _repositorio.GetByIdAsync(id);

    public async Task<int> CreateAsync(Cliente cliente)
    {
        Validar(cliente);
        cliente.Telefono = Regex.Replace(cliente.Telefono, @"[^\d]", "");
        return await _repositorio.InsertAsync(cliente);
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        Validar(cliente);
        cliente.Telefono = Regex.Replace(cliente.Telefono, @"[^\d]", "");
        await _repositorio.UpdateAsync(cliente);
    }

    public async Task DeleteAsync(int id) => await _repositorio.DeleteAsync(id);

    private static void Validar(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombre))
        {
            throw new InvalidOperationException("El nombre del cliente es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(cliente.Direccion))
        {
            throw new InvalidOperationException("La dirección del cliente es obligatoria.");
        }
    }
}

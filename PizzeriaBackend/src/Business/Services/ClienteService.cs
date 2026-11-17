using Business.Validators;
using Data.Repositories;
using Models;

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
        cliente.Telefono = TelefonoHelper.Normalizar(cliente.Telefono);
        return await _repositorio.InsertAsync(cliente);
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        cliente.Telefono = TelefonoHelper.Normalizar(cliente.Telefono);
        await _repositorio.UpdateAsync(cliente);
    }

    public async Task DeleteAsync(int id) => await _repositorio.DeleteAsync(id);
}

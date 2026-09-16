using Models;

namespace Data.Repositories;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<int> InsertAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
}
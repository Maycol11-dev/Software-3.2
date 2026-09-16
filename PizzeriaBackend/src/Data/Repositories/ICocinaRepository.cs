using Models;

namespace Data.Repositories;

public interface ICocinaRepository : IRepository<Cocina>
{
    Task<int> InsertAsync(Cocina cocina);
    Task UpdateAsync(Cocina cocina);
    Task DeleteAsync(int id);
}
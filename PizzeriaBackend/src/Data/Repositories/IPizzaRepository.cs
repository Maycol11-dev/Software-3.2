using Models;

namespace Data.Repositories;

public interface IPizzaRepository : IRepository<Pizza>
{
    Task<int> InsertAsync(Pizza pizza);
    Task UpdateAsync(Pizza pizza);
    Task DeleteAsync(int id);
}
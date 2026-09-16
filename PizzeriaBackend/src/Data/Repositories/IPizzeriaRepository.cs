using Models;

namespace Data.Repositories;

public interface IPizzeriaRepository : IRepository<Pizzeria>
{
    Task<int> InsertAsync(Pizzeria pizzeria);
    Task UpdateAsync(Pizzeria pizzeria);
    Task DeleteAsync(int id);
}
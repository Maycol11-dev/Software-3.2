using Models;

namespace Business.Services;

public interface IPizzeriaService
{
    Task<IEnumerable<Pizzeria>> GetAllAsync();
    Task<Pizzeria?> GetByIdAsync(int id);
    Task<int> CreateAsync(Pizzeria pizzeria);
    Task UpdateAsync(Pizzeria pizzeria);
    Task DeleteAsync(int id);
}
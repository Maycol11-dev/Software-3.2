using Models;

namespace Business.Services;

public interface ICocinaService
{
    Task<IEnumerable<Cocina>> GetAllAsync();
    Task<Cocina?> GetByIdAsync(int id);
    Task<int> CreateAsync(Cocina cocina);
    Task UpdateAsync(Cocina cocina);
    Task StartPreparacionAsync(int id);
    Task FinishPreparacionAsync(int id);
    Task DeleteAsync(int id);
}
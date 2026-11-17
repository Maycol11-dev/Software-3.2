using Data.Repositories;
using Models;

namespace Business.Services;

public class PizzaService : IPizzaService
{
    private readonly IPizzaRepository _repositorio;

    public PizzaService(IPizzaRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Pizza>> GetAllAsync() => await _repositorio.GetAllAsync();

    public async Task<Pizza?> GetByIdAsync(int id) => await _repositorio.GetByIdAsync(id);

    public async Task<int> CreateAsync(Pizza pizza)
        => await _repositorio.InsertAsync(pizza);

    public async Task UpdateAsync(Pizza pizza)
        => await _repositorio.UpdateAsync(pizza);

    public async Task DeleteAsync(int id) => await _repositorio.DeleteAsync(id);
}
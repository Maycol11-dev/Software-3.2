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
    {
        Validar(pizza);
        return await _repositorio.InsertAsync(pizza);
    }

    public async Task UpdateAsync(Pizza pizza)
    {
        Validar(pizza);
        await _repositorio.UpdateAsync(pizza);
    }

    public async Task DeleteAsync(int id) => await _repositorio.DeleteAsync(id);

    private static void Validar(Pizza pizza)
    {
        if (string.IsNullOrWhiteSpace(pizza.Nombre))
        {
            throw new InvalidOperationException("El nombre de la pizza es obligatorio.");
        }

        if (pizza.Precio <= 0)
        {
            throw new InvalidOperationException("El precio de la pizza debe ser mayor a cero.");
        }
    }
}
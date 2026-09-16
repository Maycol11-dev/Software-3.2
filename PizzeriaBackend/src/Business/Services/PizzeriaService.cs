using Data.Repositories;
using Models;

namespace Business.Services;

public class PizzeriaService : IPizzeriaService
{
    private readonly IPizzeriaRepository _repositorio;

    public PizzeriaService(IPizzeriaRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Pizzeria>> GetAllAsync() => await _repositorio.GetAllAsync();

    public async Task<Pizzeria?> GetByIdAsync(int id) => await _repositorio.GetByIdAsync(id);

    public async Task<int> CreateAsync(Pizzeria pizzeria)
    {
        Validar(pizzeria);
        return await _repositorio.InsertAsync(pizzeria);
    }

    public async Task UpdateAsync(Pizzeria pizzeria)
    {
        Validar(pizzeria);
        await _repositorio.UpdateAsync(pizzeria);
    }

    public async Task DeleteAsync(int id) => await _repositorio.DeleteAsync(id);

    private static void Validar(Pizzeria pizzeria)
    {
        if (string.IsNullOrWhiteSpace(pizzeria.Nombre))
        {
            throw new InvalidOperationException("El nombre de la pizzería es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(pizzeria.Direccion))
        {
            throw new InvalidOperationException("La dirección de la pizzería es obligatoria.");
        }
    }
}
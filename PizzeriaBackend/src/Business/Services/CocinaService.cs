using Data.Repositories;
using Models;

namespace Business.Services;

public class CocinaService : ICocinaService
{
    private readonly ICocinaRepository _repositorio;

    public CocinaService(ICocinaRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<Cocina>> GetAllAsync() => await _repositorio.GetAllAsync();

    public async Task<Cocina?> GetByIdAsync(int id) => await _repositorio.GetByIdAsync(id);

    public async Task<int> CreateAsync(Cocina cocina)
    {
        Validar(cocina);
        return await _repositorio.InsertAsync(cocina);
    }

    public async Task UpdateAsync(Cocina cocina)
    {
        Validar(cocina);
        await _repositorio.UpdateAsync(cocina);
    }

    public async Task StartPreparacionAsync(int id)
    {
        var cocina = await ObtenerOExiste(id);
        cocina.PedidosEnPreparacion++;
        await _repositorio.UpdateAsync(cocina);
    }

    public async Task FinishPreparacionAsync(int id)
    {
        var cocina = await ObtenerOExiste(id);
        if (cocina.PedidosEnPreparacion <= 0)
        {
            throw new InvalidOperationException("La cocina no tiene pedidos en preparación.");
        }

        cocina.PedidosEnPreparacion--;
        await _repositorio.UpdateAsync(cocina);
    }

    public async Task DeleteAsync(int id) => await _repositorio.DeleteAsync(id);

    private async Task<Cocina> ObtenerOExiste(int id)
    {
        return await _repositorio.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"La cocina {id} no existe.");
    }

    private static void Validar(Cocina cocina)
    {
        if (string.IsNullOrWhiteSpace(cocina.Nombre))
        {
            throw new InvalidOperationException("El nombre de la cocina es obligatorio.");
        }
    }
}
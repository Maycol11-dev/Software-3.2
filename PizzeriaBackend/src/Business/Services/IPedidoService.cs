using Models;

namespace Business.Services;

public interface IPedidoService
{
    Task<IEnumerable<Pedido>> GetAllAsync();
    Task<Pedido?> GetByIdAsync(int id);
    Task<int> CreateAsync(int clienteId, IEnumerable<PedidoPizza> pizzas);
    Task ChangeEstadoAsync(int id, PedidoEstado nuevoEstado);
    Task DeleteAsync(int id);
}
using Models;

namespace Data.Repositories;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<int> InsertAsync(Pedido pedido);
    Task<IEnumerable<Pedido>> GetByClienteAsync(int clienteId);
    Task UpdateEstadoAsync(int id, PedidoEstado estado);
    Task DeleteAsync(int id);
}
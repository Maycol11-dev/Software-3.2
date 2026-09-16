using Models;

namespace Data.Repositories;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<int> InsertAsync(Pedido pedido);
    Task UpdateEstadoAsync(int id, PedidoEstado estado);
    Task DeleteAsync(int id);
}
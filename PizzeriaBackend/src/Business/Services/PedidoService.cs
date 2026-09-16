using Data.Repositories;
using Models;

namespace Business.Services;

public class PedidoService : IPedidoService
{
    private static readonly Dictionary<PedidoEstado, PedidoEstado[]> Transiciones = new()
    {
        [PedidoEstado.EsperaDeConfirmacion] = new[] { PedidoEstado.EnPreparacion },
        [PedidoEstado.EnPreparacion] = new[] { PedidoEstado.EnViaje },
        [PedidoEstado.EnViaje] = new[] { PedidoEstado.Entregado },
        [PedidoEstado.Entregado] = Array.Empty<PedidoEstado>()
    };

    private readonly IPedidoRepository _pedidos;
    private readonly IClienteRepository _clientes;
    private readonly IPizzaRepository _pizzas;
    private readonly ICocinaGateway _cocina;

    public PedidoService(IPedidoRepository pedidos, IClienteRepository clientes, IPizzaRepository pizzas, ICocinaGateway cocina)
    {
        _pedidos = pedidos;
        _clientes = clientes;
        _pizzas = pizzas;
        _cocina = cocina;
    }

    public async Task<IEnumerable<Pedido>> GetAllAsync() => await _pedidos.GetAllAsync();

    public async Task<Pedido?> GetByIdAsync(int id) => await _pedidos.GetByIdAsync(id);

    public async Task<int> CreateAsync(int clienteId, IEnumerable<PedidoPizza> pizzas)
    {
        var cliente = await _clientes.GetByIdAsync(clienteId)
            ?? throw new InvalidOperationException($"El cliente {clienteId} no existe.");

        var items = pizzas.ToList();
        if (items.Count == 0)
        {
            throw new InvalidOperationException("El pedido debe incluir al menos una pizza.");
        }

        foreach (var item in items)
        {
            if (item.Cantidad <= 0)
            {
                throw new InvalidOperationException("La cantidad de cada pizza debe ser mayor a cero.");
            }

            var pizza = await _pizzas.GetByIdAsync(item.IdPizza)
                ?? throw new InvalidOperationException($"La pizza {item.IdPizza} no existe.");
        }

        var pedido = new Pedido { IdCliente = cliente.IdCliente, Pizzas = items };
        return await _pedidos.InsertAsync(pedido);
    }

    public async Task ChangeEstadoAsync(int id, PedidoEstado nuevoEstado)
    {
        var pedido = await _pedidos.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"El pedido {id} no existe.");

        if (!Transiciones[pedido.Estado].Contains(nuevoEstado))
        {
            throw new InvalidOperationException(
                $"No se puede pasar el pedido de {pedido.Estado} a {nuevoEstado}.");
        }

        if (nuevoEstado == PedidoEstado.EnPreparacion)
        {
            var respuesta = await _cocina.PrepararPedidoAsync(id);
            if (!respuesta.StartsWith("LISTO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"La cocina no confirmó la preparación: {respuesta}");
            }
        }

        await _pedidos.UpdateEstadoAsync(id, nuevoEstado);
    }

    public async Task DeleteAsync(int id) => await _pedidos.DeleteAsync(id);
}
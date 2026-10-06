using Dapper;
using Models;
using System.Data;

namespace Data.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private const string SelectPedido =
        @"SELECT p.id AS IdPedido, p.cliente_id AS IdCliente, p.estado AS Estado,
                 p.fecha_creacion AS FechaCreacion, p.total AS Total,
                 c.nombre AS NombreCliente, c.direccion AS DireccionEntrega
          FROM Pedido p
          INNER JOIN Cliente c ON c.id = p.cliente_id";

    private const string SelectPizzas =
        @"SELECT pp.pedido_id AS IdPedido, pp.pizza_id AS IdPizza, pp.cantidad AS Cantidad,
                 pp.precio_unitario AS PrecioUnitario, z.nombre AS NombrePizza
          FROM PedidoPizza pp
          INNER JOIN Pizza z ON z.id = pp.pizza_id";

    private const string SelectPizzasDeUnPedido = SelectPizzas + " WHERE pp.pedido_id = @IdPedido";

    private const string SelectPizzasDeTodos =
        SelectPizzas + " WHERE pp.pedido_id IN @Ids";

    private readonly IDbConnectionFactory _factory;

    public PedidoRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Pedido>> GetAllAsync()
    {
        using var conn = _factory.CreateConnection();
        var rows = (await conn.QueryAsync(SelectPedido + " ORDER BY p.fecha_creacion DESC")).ToList();
        var pedidos = rows.Select(ToPedido).ToList();

        if (pedidos.Count == 0)
        {
            return pedidos;
        }

        var ids = pedidos.Select(p => p.IdPedido).ToArray();
        var items = await conn.QueryAsync<PedidoPizza>(SelectPizzasDeTodos, new { Ids = ids });

        var porPedido = items.GroupBy(i => i.IdPedido).ToDictionary(g => g.Key, g => g.ToList());
        foreach (var pedido in pedidos)
        {
            pedido.Pizzas = porPedido.GetValueOrDefault(pedido.IdPedido) ?? new List<PedidoPizza>();
        }

        return pedidos;
    }

    public async Task<Pedido?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync(SelectPedido + " WHERE p.id = @Id", new { Id = id });
        if (row is null)
        {
            return null;
        }

        var pedido = ToPedido(row);
        var pizzas = await conn.QueryAsync<PedidoPizza>(SelectPizzasDeUnPedido, new { IdPedido = id });
        pedido.Pizzas = pizzas.ToList();
        return pedido;
    }

    public async Task<int> InsertAsync(Pedido pedido)
    {
        const string sql = @"
            INSERT INTO Pedido (cliente_id, estado, fecha_creacion, total)
            VALUES (@IdCliente, @Estado, @FechaCreacion, @Total);
            SELECT CAST(LAST_INSERT_ID() AS UNSIGNED);";

        using var conn = _factory.CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        var id = await conn.ExecuteScalarAsync<int>(sql, new
        {
            pedido.IdCliente,
            Estado = pedido.Estado.ToString(),
            pedido.FechaCreacion,
            pedido.Total
        }, transaction);

        foreach (var pizza in pedido.Pizzas)
        {
            await conn.ExecuteAsync(
                "INSERT INTO PedidoPizza (pedido_id, pizza_id, cantidad, precio_unitario) VALUES (@IdPedido, @IdPizza, @Cantidad, @PrecioUnitario)",
                new { IdPedido = id, pizza.IdPizza, pizza.Cantidad, pizza.PrecioUnitario },
                transaction);
        }

        transaction.Commit();
        return id;
    }

    public async Task UpdateEstadoAsync(int id, PedidoEstado estado)
    {
        const string sql = "UPDATE Pedido SET estado = @Estado WHERE id = @Id";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id, Estado = estado.ToString() });
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Pedido WHERE id = @Id";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    private static Pedido ToPedido(dynamic row)
    {
        return new Pedido
        {
            IdPedido = row.IdPedido,
            IdCliente = row.IdCliente,
            Estado = Enum.Parse<PedidoEstado>(row.Estado),
            FechaCreacion = row.FechaCreacion,
            Total = row.Total,
            NombreCliente = row.NombreCliente,
            DireccionEntrega = row.DireccionEntrega
        };
    }
}
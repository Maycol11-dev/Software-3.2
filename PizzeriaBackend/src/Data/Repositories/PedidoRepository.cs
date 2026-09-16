using Dapper;
using Models;
using System.Data;

namespace Data.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private const string SelectPedido =
        "SELECT id AS IdPedido, cliente_id AS IdCliente, estado AS Estado, fecha_creacion AS FechaCreacion FROM Pedido";

    private const string SelectPizzas =
        "SELECT pedido_id AS IdPedido, pizza_id AS IdPizza, cantidad AS Cantidad FROM PedidoPizza WHERE pedido_id = @IdPedido";

    private readonly IDbConnectionFactory _factory;

    public PedidoRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Pedido>> GetAllAsync()
    {
        using var conn = _factory.CreateConnection();
        var rows = await conn.QueryAsync(SelectPedido + " ORDER BY fecha_creacion DESC");
        return rows.Select(ToPedido).ToList();
    }

    public async Task<Pedido?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync(SelectPedido + " WHERE id = @Id", new { Id = id });
        if (row is null)
        {
            return null;
        }

        var pedido = ToPedido(row);
        var pizzas = await conn.QueryAsync<PedidoPizza>(SelectPizzas, new { IdPedido = id });
        pedido.Pizzas = pizzas.ToList();
        return pedido;
    }

    public async Task<int> InsertAsync(Pedido pedido)
    {
        const string sql = @"
            INSERT INTO Pedido (cliente_id, estado, fecha_creacion)
            VALUES (@IdCliente, @Estado, @FechaCreacion);
            SELECT CAST(LAST_INSERT_ID() AS UNSIGNED);";

        using var conn = _factory.CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        var id = await conn.ExecuteScalarAsync<int>(sql, new
        {
            pedido.IdCliente,
            Estado = pedido.Estado.ToString(),
            pedido.FechaCreacion
        }, transaction);

        foreach (var pizza in pedido.Pizzas)
        {
            await conn.ExecuteAsync(
                "INSERT INTO PedidoPizza (pedido_id, pizza_id, cantidad) VALUES (@IdPedido, @IdPizza, @Cantidad)",
                new { IdPedido = id, pizza.IdPizza, pizza.Cantidad },
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
            FechaCreacion = row.FechaCreacion
        };
    }
}
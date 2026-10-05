using Dapper;
using Models;

namespace Data.Repositories;

public class PizzaRepository : IPizzaRepository
{
    private const string SelectPizza =
        "SELECT id AS IdPizza, nombre AS Nombre, descripcion AS Descripcion, precio AS Precio, imagen_url AS ImagenUrl FROM Pizza";

    private readonly IDbConnectionFactory _factory;

    public PizzaRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Pizza>> GetAllAsync()
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<Pizza>(SelectPizza);
    }

    public async Task<Pizza?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Pizza>(SelectPizza + " WHERE id = @Id", new { Id = id });
    }

    public async Task<int> InsertAsync(Pizza pizza)
    {
        const string sql = @"
            INSERT INTO Pizza (nombre, descripcion, precio, imagen_url)
            VALUES (@Nombre, @Descripcion, @Precio, @ImagenUrl);
            SELECT CAST(LAST_INSERT_ID() AS UNSIGNED);";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, pizza);
    }

    public async Task UpdateAsync(Pizza pizza)
    {
        const string sql = @"
            UPDATE Pizza
            SET nombre = @Nombre, descripcion = @Descripcion, precio = @Precio, imagen_url = @ImagenUrl
            WHERE id = @IdPizza";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, pizza);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Pizza WHERE id = @Id";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
using Dapper;
using Models;

namespace Data.Repositories;

public class CocinaRepository : ICocinaRepository
{
    private const string SelectCocina =
        "SELECT id AS IdCocina, nombre AS Nombre, disponible AS Disponible, pedidos_en_preparacion AS PedidosEnPreparacion FROM Cocina";

    private readonly IDbConnectionFactory _factory;

    public CocinaRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Cocina>> GetAllAsync()
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<Cocina>(SelectCocina);
    }

    public async Task<Cocina?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Cocina>(SelectCocina + " WHERE id = @Id", new { Id = id });
    }

    public async Task<int> InsertAsync(Cocina cocina)
    {
        const string sql = @"
            INSERT INTO Cocina (nombre, disponible, pedidos_en_preparacion)
            VALUES (@Nombre, @Disponible, @PedidosEnPreparacion);
            SELECT CAST(LAST_INSERT_ID() AS UNSIGNED);";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, cocina);
    }

    public async Task UpdateAsync(Cocina cocina)
    {
        const string sql = @"
            UPDATE Cocina
            SET nombre = @Nombre, disponible = @Disponible, pedidos_en_preparacion = @PedidosEnPreparacion
            WHERE id = @IdCocina";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, cocina);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Cocina WHERE id = @Id";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
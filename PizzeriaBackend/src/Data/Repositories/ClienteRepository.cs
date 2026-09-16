using Dapper;
using Models;

namespace Data.Repositories;

public class ClienteRepository : IClienteRepository
{
    private const string SelectCliente =
        "SELECT id AS IdCliente, nombre AS Nombre, telefono AS Telefono, direccion AS Direccion FROM Cliente";

    private readonly IDbConnectionFactory _factory;

    public ClienteRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<Cliente>(SelectCliente);
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Cliente>(SelectCliente + " WHERE id = @Id", new { Id = id });
    }

    public async Task<int> InsertAsync(Cliente cliente)
    {
        const string sql = @"
            INSERT INTO Cliente (nombre, telefono, direccion)
            VALUES (@Nombre, @Telefono, @Direccion);
            SELECT CAST(LAST_INSERT_ID() AS UNSIGNED);";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, cliente);
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        const string sql = @"
            UPDATE Cliente
            SET nombre = @Nombre, telefono = @Telefono, direccion = @Direccion
            WHERE id = @IdCliente";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, cliente);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Cliente WHERE id = @Id";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
using Dapper;
using Models;

namespace Data.Repositories;

public class PizzeriaRepository : IPizzeriaRepository
{
    private const string SelectPizzeria =
        "SELECT id AS IdPizzeria, nombre AS Nombre, direccion AS Direccion, telefono AS Telefono, horario_atencion AS HorarioAtencion FROM Pizzeria";

    private readonly IDbConnectionFactory _factory;

    public PizzeriaRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Pizzeria>> GetAllAsync()
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<Pizzeria>(SelectPizzeria);
    }

    public async Task<Pizzeria?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Pizzeria>(SelectPizzeria + " WHERE id = @Id", new { Id = id });
    }

    public async Task<int> InsertAsync(Pizzeria pizzeria)
    {
        const string sql = @"
            INSERT INTO Pizzeria (nombre, direccion, telefono, horario_atencion)
            VALUES (@Nombre, @Direccion, @Telefono, @HorarioAtencion);
            SELECT CAST(LAST_INSERT_ID() AS UNSIGNED);";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, pizzeria);
    }

    public async Task UpdateAsync(Pizzeria pizzeria)
    {
        const string sql = @"
            UPDATE Pizzeria
            SET nombre = @Nombre, direccion = @Direccion, telefono = @Telefono, horario_atencion = @HorarioAtencion
            WHERE id = @IdPizzeria";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, pizzeria);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Pizzeria WHERE id = @Id";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
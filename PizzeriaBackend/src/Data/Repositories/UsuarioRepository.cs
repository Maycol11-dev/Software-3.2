using Dapper;
using Models;

namespace Data.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private const string SelectUsuario =
        "SELECT id AS IdUsuario, correo AS Correo, contrasena AS Contrasena, rol_id AS RolId, fecha_registro AS FechaRegistro FROM Usuario";

    private readonly IDbConnectionFactory _factory;

    public UsuarioRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<Usuario>(SelectUsuario);
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Usuario>(SelectUsuario + " WHERE id = @Id", new { Id = id });
    }

    public async Task<Usuario?> GetByCorreoAsync(string correo)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Usuario>(SelectUsuario + " WHERE correo = @Correo", new { Correo = correo });
    }

    public async Task<int> InsertAsync(Usuario usuario)
    {
        const string sql = @"
            INSERT INTO Usuario (correo, contrasena, rol_id)
            VALUES (@Correo, @Contrasena, @RolId);
            SELECT CAST(LAST_INSERT_ID() AS UNSIGNED);";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, usuario);
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        const string sql = @"
            UPDATE Usuario
            SET correo = @Correo, contrasena = @Contrasena, rol_id = @RolId
            WHERE id = @IdUsuario";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, usuario);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Usuario WHERE id = @Id";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
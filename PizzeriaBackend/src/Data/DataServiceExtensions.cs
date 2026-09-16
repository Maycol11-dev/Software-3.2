using Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class DataServiceExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PizzeriaDB")
            ?? throw new InvalidOperationException("Falta la connection string 'PizzeriaDB' en appsettings.");
        services.AddSingleton<IDbConnectionFactory>(new MySqlConnectionFactory(connectionString));

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IPizzaRepository, PizzaRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<ICocinaRepository, CocinaRepository>();
        services.AddScoped<IPizzeriaRepository, PizzeriaRepository>();

        return services;
    }
}
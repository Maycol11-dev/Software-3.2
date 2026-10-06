using Business.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business;

public static class BusinessServiceExtensions
{
    public static IServiceCollection AddBusiness(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration["CocinaSocket:Host"] ?? "127.0.0.1";
        var puerto = int.TryParse(configuration["CocinaSocket:Port"], out var p) ? p : 5000;

        services.AddSingleton<ICocinaGateway>(new CocinaSocketClient(host, puerto));

        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IPizzaService, PizzaService>();
        services.AddScoped<IPedidoService, PedidoService>();

        return services;
    }
}
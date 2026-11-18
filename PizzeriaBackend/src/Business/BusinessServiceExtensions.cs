using Business.Services;
using Business.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business;

public static class BusinessServiceExtensions
{
    public static IServiceCollection AddBusiness(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IPizzaService, PizzaService>();
        services.AddScoped<IPedidoService, PedidoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddValidatorsFromAssemblyContaining<RegistroUsuarioValidator>();

        return services;
    }
}
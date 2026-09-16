using Business.Services;
using Models;

namespace API.Endpoints;

public static class ClienteEndpoints
{
    public static IEndpointRouteBuilder MapClienteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clientes").WithTags("Clientes");

        group.MapGet("/", async (IClienteService service) => Results.Ok(await service.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, IClienteService service) =>
        {
            var cliente = await service.GetByIdAsync(id);
            return cliente is null ? Results.NotFound() : Results.Ok(cliente);
        });

        group.MapPost("/", async (Cliente cliente, IClienteService service) =>
        {
            var id = await service.CreateAsync(cliente);
            return Results.Created($"/api/clientes/{id}", new { id });
        });

        group.MapPut("/{id:int}", async (int id, Cliente cliente, IClienteService service) =>
        {
            cliente.IdCliente = id;
            await service.UpdateAsync(cliente);
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, IClienteService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });

        return app;
    }
}
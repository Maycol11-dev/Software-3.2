using Business.Services;
using Models;

namespace API.Endpoints;

public static class PedidoEndpoints
{
    public static IEndpointRouteBuilder MapPedidoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pedidos").WithTags("Pedidos");

        group.MapGet("/", async (IPedidoService service) => Results.Ok(await service.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, IPedidoService service) =>
        {
            var pedido = await service.GetByIdAsync(id);
            return pedido is null ? Results.NotFound() : Results.Ok(pedido);
        });

        group.MapPost("/", async (CrearPedidoRequest request, IPedidoService service) =>
        {
            var pizzas = request.Pizzas
                .Select(p => new PedidoPizza { IdPizza = p.IdPizza, Cantidad = p.Cantidad })
                .ToList();

            var id = await service.CreateAsync(request.ClienteId, pizzas);
            return Results.Created($"/api/pedidos/{id}", new { id });
        });

        group.MapPatch("/{id:int}/estado", async (int id, CambiarEstadoRequest request, IPedidoService service) =>
        {
            await service.ChangeEstadoAsync(id, request.Estado);
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, IPedidoService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });

        return app;
    }
}

public record CrearPedidoRequest(int ClienteId, List<PedidoPizzaRequest> Pizzas);

public record PedidoPizzaRequest(int IdPizza, int Cantidad);

public record CambiarEstadoRequest(PedidoEstado Estado);
using API.Filters;
using Business.Services;
using Models;
using System.Security.Claims;

namespace API.Endpoints;

public static class PedidoEndpoints
{
    public static IEndpointRouteBuilder MapPedidoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pedidos").WithTags("Pedidos");

        group.MapGet("/", async (IPedidoService service) => Results.Ok(await service.GetAllAsync()))
            .RequireAuthorization(policy => policy.RequireRole("Administrador"));

        group.MapGet("/{id:int}", async (int id, IPedidoService service, ClaimsPrincipal usuario) =>
        {
            var pedido = await service.GetByIdAsync(id);
            if (pedido is null)
            {
                return Results.NotFound();
            }

            var esAdmin = usuario.IsInRole("Administrador");
            var clienteIdClaim = usuario.FindFirst("clienteId")?.Value;
            if (!esAdmin && (!int.TryParse(clienteIdClaim, out var clienteId) || pedido.IdCliente != clienteId))
            {
                return Results.Forbid();
            }

            return Results.Ok(pedido);
        }).RequireAuthorization();

        group.MapGet("/cliente/{clienteId:int}", async (int clienteId, IPedidoService service, ClaimsPrincipal usuario) =>
        {
            var esAdmin = usuario.IsInRole("Administrador");
            var clienteIdClaim = usuario.FindFirst("clienteId")?.Value;
            if (!esAdmin && (!int.TryParse(clienteIdClaim, out var cid) || cid != clienteId))
            {
                return Results.Forbid();
            }

            return Results.Ok(await service.GetByClienteAsync(clienteId));
        }).RequireAuthorization();

        group.MapPost("/", async (CrearPedidoRequest request, IPedidoService service, ClaimsPrincipal usuario) =>
        {
            var esAdmin = usuario.IsInRole("Administrador");
            var clienteIdClaim = usuario.FindFirst("clienteId")?.Value;
            if (!esAdmin && (!int.TryParse(clienteIdClaim, out var cid) || cid != request.ClienteId))
            {
                return Results.Forbid();
            }

            var pizzas = request.Pizzas
                .Select(p => new PedidoPizza { IdPizza = p.IdPizza, Cantidad = p.Cantidad })
                .ToList();

            var id = await service.CreateAsync(request.ClienteId, pizzas);
            return Results.Created($"/api/pedidos/{id}", new { id });
        }).RequireAuthorization().AddEndpointFilter<ValidationFilter<CrearPedidoRequest>>();

        group.MapPatch("/{id:int}/estado", async (int id, CambiarEstadoRequest request, IPedidoService service) =>
        {
            await service.ChangeEstadoAsync(id, request.Estado);
            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

        group.MapDelete("/{id:int}", async (int id, IPedidoService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

        return app;
    }
}
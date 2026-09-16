using Business.Services;
using Models;

namespace API.Endpoints;

public static class PizzeriaEndpoints
{
    public static IEndpointRouteBuilder MapPizzeriaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pizzeria").WithTags("Pizzeria");

        group.MapGet("/", async (IPizzeriaService service) => Results.Ok(await service.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, IPizzeriaService service) =>
        {
            var pizzeria = await service.GetByIdAsync(id);
            return pizzeria is null ? Results.NotFound() : Results.Ok(pizzeria);
        });

        group.MapPost("/", async (Pizzeria pizzeria, IPizzeriaService service) =>
        {
            var id = await service.CreateAsync(pizzeria);
            return Results.Created($"/api/pizzeria/{id}", new { id });
        });

        group.MapPut("/{id:int}", async (int id, Pizzeria pizzeria, IPizzeriaService service) =>
        {
            pizzeria.IdPizzeria = id;
            await service.UpdateAsync(pizzeria);
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, IPizzeriaService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });

        return app;
    }
}
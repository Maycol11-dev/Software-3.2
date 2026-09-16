using Business.Services;
using Models;

namespace API.Endpoints;

public static class CocinaEndpoints
{
    public static IEndpointRouteBuilder MapCocinaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cocinas").WithTags("Cocinas");

        group.MapGet("/", async (ICocinaService service) => Results.Ok(await service.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, ICocinaService service) =>
        {
            var cocina = await service.GetByIdAsync(id);
            return cocina is null ? Results.NotFound() : Results.Ok(cocina);
        });

        group.MapPost("/", async (Cocina cocina, ICocinaService service) =>
        {
            var id = await service.CreateAsync(cocina);
            return Results.Created($"/api/cocinas/{id}", new { id });
        });

        group.MapPut("/{id:int}", async (int id, Cocina cocina, ICocinaService service) =>
        {
            cocina.IdCocina = id;
            await service.UpdateAsync(cocina);
            return Results.NoContent();
        });

        group.MapPost("/{id:int}/iniciar", async (int id, ICocinaService service) =>
        {
            await service.StartPreparacionAsync(id);
            return Results.NoContent();
        });

        group.MapPost("/{id:int}/finalizar", async (int id, ICocinaService service) =>
        {
            await service.FinishPreparacionAsync(id);
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, ICocinaService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });

        return app;
    }
}
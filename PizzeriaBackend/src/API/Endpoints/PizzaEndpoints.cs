using API.Filters;
using Business.Services;
using Models;

namespace API.Endpoints;

public static class PizzaEndpoints
{
    public static IEndpointRouteBuilder MapPizzaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pizzas").WithTags("Pizzas");

        group.MapGet("/", async (IPizzaService service) => Results.Ok(await service.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, IPizzaService service) =>
        {
            var pizza = await service.GetByIdAsync(id);
            return pizza is null ? Results.NotFound() : Results.Ok(pizza);
        });

        group.MapPost("/", async (Pizza pizza, IPizzaService service) =>
        {
            var id = await service.CreateAsync(pizza);
            return Results.Created($"/api/pizzas/{id}", new { id });
        }).RequireAuthorization(policy => policy.RequireRole("Administrador")).AddEndpointFilter<ValidationFilter<Pizza>>();

        group.MapPut("/{id:int}", async (int id, Pizza pizza, IPizzaService service) =>
        {
            pizza.IdPizza = id;
            await service.UpdateAsync(pizza);
            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador")).AddEndpointFilter<ValidationFilter<Pizza>>();

        group.MapDelete("/{id:int}", async (int id, IPizzaService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole("Administrador"));

        return app;
    }
}
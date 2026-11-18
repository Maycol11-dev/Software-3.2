using API.Filters;
using Business.Services;
using Models;

namespace API.Endpoints;

public record LoginResponse(
    string Token,
    int UsuarioId,
    int ClienteId,
    string Correo,
    string Nombre,
    string Telefono,
    string Direccion,
    int RolId);

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/registro", async (RegistroUsuario registro, IUsuarioService service) =>
        {
            var id = await service.RegistrarAsync(registro);
            return Results.Created($"/api/usuarios/{id}", new { id });
        }).AddEndpointFilter<ValidationFilter<RegistroUsuario>>();

        group.MapPost("/login", async (LoginRequest request, IUsuarioService service, ITokenService tokenService) =>
        {
            var sesion = await service.AutenticarAsync(request.Correo, request.Contrasena);
            if (sesion is null)
            {
                return Results.Unauthorized();
            }

            var token = tokenService.GenerarToken(sesion);
            return Results.Ok(new LoginResponse(
                token,
                sesion.UsuarioId,
                sesion.ClienteId,
                sesion.Correo,
                sesion.Nombre,
                sesion.Telefono,
                sesion.Direccion,
                sesion.RolId));
        })
        .AddEndpointFilter<ValidationFilter<LoginRequest>>()
        .Produces<LoginResponse>(200)
        .Produces(401);

        return app;
    }
}

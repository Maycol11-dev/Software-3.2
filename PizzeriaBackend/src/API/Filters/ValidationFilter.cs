using FluentValidation;

namespace API.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var modelo = context.Arguments.OfType<T>().FirstOrDefault();
        if (modelo is null)
        {
            return Results.BadRequest(new { titulo = "Solicitud inválida", mensaje = "El cuerpo de la solicitud no es válido." });
        }

        var resultado = await _validator.ValidateAsync(modelo);
        if (resultado.IsValid)
        {
            return await next(context);
        }

        var errores = resultado.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return Results.ValidationProblem(errores);
    }
}

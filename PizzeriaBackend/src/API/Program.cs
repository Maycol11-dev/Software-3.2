using System.Text.Json.Serialization;
using API.Endpoints;
using Business;
using Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5184", "https://localhost:7184")
              .AllowAnyMethod()
              .AllowAnyHeader()));
builder.Services.AddData(builder.Configuration);
builder.Services.AddBusiness(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
        options.WithTitle("Pizzeria API");
    });
}

app.UseCors("AllowFrontend");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        var (statusCode, titulo) = exception switch
        {
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Error de validación"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            titulo,
            mensaje = exception?.Message ?? "Ocurrió un error inesperado."
        });
    });
});

app.MapGet("/", () => "Pizzeria API funcionando");
app.MapClienteEndpoints();
app.MapPizzaEndpoints();
app.MapPedidoEndpoints();
app.MapCocinaEndpoints();
app.MapPizzeriaEndpoints();

app.Run();
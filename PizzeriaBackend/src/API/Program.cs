using System.Text;
using System.Text.Json.Serialization;
using API.Endpoints;
using Business;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pegá el token que devuelve /api/auth/login"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();
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
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
        options.WithTitle("Pizzeria API");
    });
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

var logger = app.Logger;

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        var (statusCode, titulo, mensaje) = exception switch
        {
            InvalidOperationException validacion => (StatusCodes.Status400BadRequest, "Error de validación", validacion.Message),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor", "Ocurrió un error inesperado. Intentá más tarde.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Error no manejado en la API");
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            titulo,
            mensaje
        });
    });
});

app.MapGet("/", () => "Pizzeria API funcionando");
app.MapClienteEndpoints();
app.MapPizzaEndpoints();
app.MapPedidoEndpoints();
app.MapAuthEndpoints();

app.Run();
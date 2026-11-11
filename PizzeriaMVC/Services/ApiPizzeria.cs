using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using PizzeriaMVC.Data;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Services;

public class ApiPizzeria
{
    public const string SessionKeyToken = "JwtToken";

    private const string CacheKeyPizzas = "pizzas";
    private static readonly TimeSpan CacheDuracion = TimeSpan.FromSeconds(30);

    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _http;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiPizzeria(HttpClient client, IConfiguration config, IMemoryCache cache, IHttpContextAccessor http)
    {
        _client = client;
        _client.BaseAddress = new Uri(config["Api:BaseUrl"]!);
        _cache = cache;
        _http = http;
    }

    private void AgregarToken()
    {
        var token = _http.HttpContext?.Session.GetString(SessionKeyToken);
        _client.DefaultRequestHeaders.Authorization =
            string.IsNullOrEmpty(token) ? null : new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<PizzaMenuItem>> GetPizzasAsync()
    {
        if (_cache.TryGetValue(CacheKeyPizzas, out List<PizzaMenuItem>? cacheadas) && cacheadas is not null)
        {
            return cacheadas;
        }

        List<PizzaMenuItem> pizzas;
        try
        {
            var response = await _client.GetAsync("/api/pizzas");
            response.EnsureSuccessStatusCode();
            pizzas = await response.Content.ReadFromJsonAsync<List<PizzaMenuItem>>(JsonOptions) ?? new();
        }
        catch (HttpRequestException)
        {
            return MenuMock.Pizzas;
        }

        _cache.Set(CacheKeyPizzas, pizzas, CacheDuracion);
        return pizzas;
    }

    public async Task<Dictionary<int, PizzaMenuItem>> GetPizzasPorIdAsync(IEnumerable<int> ids)
    {
        var pedidos = ids.Distinct().ToList();
        if (pedidos.Count == 0)
        {
            return new Dictionary<int, PizzaMenuItem>();
        }

        var pizzas = await GetPizzasAsync();
        return pizzas.Where(p => pedidos.Contains(p.IdPizza)).ToDictionary(p => p.IdPizza);
    }

    public async Task<int> CrearClienteAsync(ClienteDto cliente)
    {
        var response = await _client.PostAsJsonAsync("/api/clientes", cliente);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<int> CrearPedidoAsync(CrearPedidoDto pedido)
    {
        AgregarToken();
        var response = await _client.PostAsJsonAsync("/api/pedidos", pedido);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<PedidoDetalleDto?> GetPedidoAsync(int id)
    {
        var response = await _client.GetAsync($"/api/pedidos/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PedidoDetalleDto>(JsonOptions);
    }

    public async Task<List<PedidoDetalleDto>> GetPedidosAsync()
    {
        AgregarToken();
        var response = await _client.GetAsync("/api/pedidos");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<PedidoDetalleDto>>(JsonOptions) ?? new();
    }

    public async Task<List<PedidoDetalleDto>> GetPedidosDeClienteAsync(int clienteId)
    {
        AgregarToken();
        var response = await _client.GetAsync($"/api/pedidos/cliente/{clienteId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<PedidoDetalleDto>>(JsonOptions) ?? new();
    }

    public async Task RegistrarAsync(RegistroDto registro)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/registro", registro);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(await LeerMensajeError(response));
        }
    }

    public async Task<SesionDto?> LoginAsync(string correo, string contrasena)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { correo, contrasena });

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(await LeerMensajeError(response));
        }

        var sesion = await response.Content.ReadFromJsonAsync<SesionDto>(JsonOptions);

        if (sesion is not null && !string.IsNullOrEmpty(sesion.Token))
        {
            _http.HttpContext?.Session.SetString(SessionKeyToken, sesion.Token);
        }

        return sesion;
    }

    public void CerrarSesion()
    {
        _http.HttpContext?.Session.Remove(SessionKeyToken);
    }

    private static async Task<string> LeerMensajeError(HttpResponseMessage response)
    {
        if ((int)response.StatusCode >= 500)
        {
            return "No se pudo completar la operación. Intentá de nuevo.";
        }

        try
        {
            var problema = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(JsonOptions);
            if (problema is not null && problema.TryGetValue("mensaje", out var mensaje))
            {
                return mensaje;
            }
        }
        catch (JsonException)
        {
        }

        return "No se pudo completar la operación. Intentá de nuevo.";
    }
}

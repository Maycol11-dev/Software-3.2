using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using PizzeriaMVC.Data;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Services;

public class ApiPizzeria
{
    private const string CacheKeyPizzas = "pizzas";
    private static readonly TimeSpan CacheDuracion = TimeSpan.FromSeconds(30);

    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiPizzeria(HttpClient client, IConfiguration config, IMemoryCache cache)
    {
        _client = client;
        _client.BaseAddress = new Uri(config["Api:BaseUrl"]!);
        _cache = cache;
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
        var response = await _client.GetAsync("/api/pedidos");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<PedidoDetalleDto>>(JsonOptions) ?? new();
    }
}

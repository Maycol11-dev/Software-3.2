using System.Text;
using System.Text.Json;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Services;

public class ApiPizzeria
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiPizzeria(HttpClient client, IConfiguration config)
    {
        _client = client;
        _client.BaseAddress = new Uri(config["Api:BaseUrl"]!);
    }

    public async Task<List<PizzaMenuItem>> GetPizzasAsync()
    {
        var response = await _client.GetAsync("pizzas");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<PizzaMenuItem>>(JsonOptions) ?? new();
    }

    public async Task<int> CrearClienteAsync(ClienteDto cliente)
    {
        var response = await _client.PostAsJsonAsync("clientes", cliente);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<int> CrearPedidoAsync(CrearPedidoDto pedido)
    {
        var response = await _client.PostAsJsonAsync("pedidos", pedido);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task<PedidoDetalleDto?> GetPedidoAsync(int id)
    {
        var response = await _client.GetAsync($"pedidos/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PedidoDetalleDto>(JsonOptions);
    }

    public async Task<List<PedidoDetalleDto>> GetPedidosAsync()
    {
        var response = await _client.GetAsync("pedidos");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<PedidoDetalleDto>>(JsonOptions) ?? new();
    }
}

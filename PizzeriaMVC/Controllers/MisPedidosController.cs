using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

public class MisPedidosController : Controller
{
    private readonly ApiPizzeria _api;
    private readonly CarritoService _carrito;

    public MisPedidosController(ApiPizzeria api, CarritoService carrito)
    {
        _api = api;
        _carrito = carrito;
    }

    [HttpGet]
    public async Task<IActionResult> MisPedidos()
    {
        List<PedidoDetalleDto> pedidosApi;
        try
        {
            pedidosApi = await _api.GetPedidosAsync();
        }
        catch (HttpRequestException)
        {
            pedidosApi = new List<PedidoDetalleDto>();
        }

        var pedidos = pedidosApi.Select(p => new PedidoHistorial
        {
            Id = p.IdPedido,
            Fecha = p.FechaCreacion,
            Total = p.Total,
            NombreCliente = p.NombreCliente,
            DireccionEntrega = p.DireccionEntrega,
            Items = p.Pizzas.Select(i => new PedidoHistorialItem
            {
                IdPizza = i.IdPizza,
                Nombre = i.NombrePizza,
                Precio = i.PrecioUnitario,
                Cantidad = i.Cantidad
            }).ToList()
        }).ToList();

        return View(new MisPedidosViewModel { Pedidos = pedidos });
    }

    [HttpPost]
    public async Task<IActionResult> Repetir(int id)
    {
        var pedido = await _api.GetPedidoAsync(id);

        if (pedido is not null)
        {
            foreach (var item in pedido.Pizzas)
            {
                _carrito.Agregar(HttpContext.Session, item.IdPizza, item.Cantidad);
            }

            TempData["MensajeInfo"] = $"Pedido #{pedido.IdPedido} agregado al carrito.";
        }

        return RedirectToAction(nameof(MenuController.Menu), "Menu");
    }
}
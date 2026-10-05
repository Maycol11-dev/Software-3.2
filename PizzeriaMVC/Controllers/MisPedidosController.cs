using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

public class MisPedidosController : Controller
{
    private readonly ApiPizzeria _api;

    public MisPedidosController(ApiPizzeria api)
    {
        _api = api;
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
                Nombre = "",
                Precio = 0,
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
            var carrito = ObtenerCarrito();

            foreach (var item in pedido.Pizzas)
            {
                var existente = carrito.Items.FirstOrDefault(i => i.IdPizza == item.IdPizza);
                if (existente is not null)
                {
                    existente.Cantidad += item.Cantidad;
                }
                else
                {
                    carrito.Items.Add(new CarritoItem
                    {
                        IdPizza = item.IdPizza,
                        Nombre = "",
                        Precio = 0,
                        Cantidad = item.Cantidad
                    });
                }
            }

            GuardarCarrito(carrito);
            TempData["MensajeInfo"] = $"Pedido #{pedido.IdPedido} agregado al carrito.";
        }

        return RedirectToAction(nameof(MenuController.Menu), "Menu");
    }

    private CarritoViewModel ObtenerCarrito()
        => HttpContext.Session.Get<CarritoViewModel>(SessionKeys.Carrito) ?? new CarritoViewModel();

    private void GuardarCarrito(CarritoViewModel carrito)
        => HttpContext.Session.Set(SessionKeys.Carrito, carrito);
}

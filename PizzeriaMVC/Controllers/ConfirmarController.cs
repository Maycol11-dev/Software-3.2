using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

[Authorize]
public class ConfirmarController : Controller
{
    private readonly ApiPizzeria _api;
    private readonly CarritoService _carrito;

    public ConfirmarController(ApiPizzeria api, CarritoService carrito)
    {
        _api = api;
        _carrito = carrito;
    }

    [HttpGet]
    public async Task<IActionResult> Confirmar()
    {
        var modelo = new ConfirmarViewModel
        {
            Carrito = await _carrito.ObtenerHidratado(HttpContext.Session),
            Nombre = User.Identity?.Name ?? string.Empty,
            Telefono = User.FindFirst("telefono")?.Value ?? string.Empty,
            Direccion = User.FindFirst("direccion")?.Value ?? string.Empty
        };

        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarEnvio()
    {
        var carrito = await _carrito.ObtenerHidratado(HttpContext.Session);

        if (!carrito.Vacio)
        {
            var clienteId = int.Parse(User.FindFirst("clienteId")?.Value ?? "0");

            try
            {
                var pedidoId = await _api.CrearPedidoAsync(new CrearPedidoDto
                {
                    ClienteId = clienteId,
                    Pizzas = carrito.Items.Select(i => new PedidoPizzaDto
                    {
                        IdPizza = i.IdPizza,
                        Cantidad = i.Cantidad
                    }).ToList()
                });

                _carrito.Guardar(HttpContext.Session, new CarritoViewModel());
                TempData["MensajeExito"] = $"Pedido recibido. ¡Gracias {User.Identity?.Name}!";
                return RedirectToAction(nameof(SeguimientoController.Seguimiento), "Seguimiento", new { id = pedidoId });
            }
            catch (HttpRequestException)
            {
                TempData["MensajeError"] = "No se pudo procesar el pedido. El servidor no está disponible.";
                return RedirectToAction(nameof(MenuController.Index), "Menu");
            }
        }

        _carrito.Guardar(HttpContext.Session, new CarritoViewModel());
        return RedirectToAction(nameof(MenuController.Index), "Menu");
    }
}

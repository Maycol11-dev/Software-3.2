using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

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
        return View(await _carrito.ObtenerHidratado(HttpContext.Session));
    }

    [HttpPost]
    public async Task<IActionResult> Confirmar(string nombre, string telefono, string direccion)
    {
        var carrito = await _carrito.ObtenerHidratado(HttpContext.Session);

        if (!carrito.Vacio)
        {
            try
            {
                var clienteId = await _api.CrearClienteAsync(new ClienteDto
                {
                    Nombre = nombre,
                    Telefono = telefono,
                    Direccion = direccion
                });

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
                TempData["MensajeExito"] = $"Pedido recibido. ¡Gracias {nombre}! Lo enviamos a {direccion}.";
                return RedirectToAction(nameof(SeguimientoController.Seguimiento), "Seguimiento", new { id = pedidoId, paso = 1 });
            }
            catch (HttpRequestException)
            {
                TempData["MensajeError"] = "No se pudo procesar el pedido. El servidor no está disponible.";
                return RedirectToAction(nameof(MenuController.Menu), "Menu");
            }
        }

        _carrito.Guardar(HttpContext.Session, new CarritoViewModel());
        return RedirectToAction(nameof(MenuController.Menu), "Menu");
    }
}
using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

public class ConfirmarController : Controller
{
    private readonly ApiPizzeria _api;

    public ConfirmarController(ApiPizzeria api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Confirmar()
    {
        return View(ObtenerCarrito());
    }

    [HttpPost]
    public async Task<IActionResult> Confirmar(string nombre, string telefono, string direccion)
    {
        var carrito = ObtenerCarrito();

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

                GuardarCarrito(new CarritoViewModel());
                TempData["MensajeExito"] = $"Pedido recibido. ¡Gracias {nombre}! Lo enviamos a {direccion}.";
                return RedirectToAction(nameof(SeguimientoController.Seguimiento), "Seguimiento", new { id = pedidoId, paso = 1 });
            }
            catch (HttpRequestException)
            {
                TempData["MensajeError"] = "No se pudo procesar el pedido. El servidor no está disponible.";
                return RedirectToAction(nameof(MenuController.Menu), "Menu");
            }
        }

        GuardarCarrito(new CarritoViewModel());
        return RedirectToAction(nameof(MenuController.Menu), "Menu");
    }

    private CarritoViewModel ObtenerCarrito()
        => HttpContext.Session.Get<CarritoViewModel>(SessionKeys.Carrito) ?? new CarritoViewModel();

    private void GuardarCarrito(CarritoViewModel carrito)
        => HttpContext.Session.Set(SessionKeys.Carrito, carrito);
}

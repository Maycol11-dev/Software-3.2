using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Controllers;

public class MisPedidosController : Controller
{
    [HttpGet]
    public IActionResult MisPedidos()
    {
        return View(new MisPedidosViewModel { Pedidos = ObtenerHistorial() });
    }

    [HttpPost]
    public IActionResult Repetir(int id)
    {
        var historial = ObtenerHistorial();
        var pedido = historial.FirstOrDefault(p => p.Id == id);

        if (pedido is not null)
        {
            var carrito = ObtenerCarrito();

            foreach (var item in pedido.Items)
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
                        Nombre = item.Nombre,
                        Precio = item.Precio,
                        Cantidad = item.Cantidad
                    });
                }
            }

            GuardarCarrito(carrito);
            TempData["MensajeInfo"] = $"Pedido #{pedido.Id} agregado al carrito.";
        }

        return RedirectToAction(nameof(MenuController.Menu), "Menu");
    }

    private CarritoViewModel ObtenerCarrito()
        => HttpContext.Session.Get<CarritoViewModel>(SessionKeys.Carrito) ?? new CarritoViewModel();

    private void GuardarCarrito(CarritoViewModel carrito)
        => HttpContext.Session.Set(SessionKeys.Carrito, carrito);

    private List<PedidoHistorial> ObtenerHistorial()
        => HttpContext.Session.Get<List<PedidoHistorial>>(SessionKeys.HistorialPedidos) ?? new List<PedidoHistorial>();
}

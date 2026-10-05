using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Controllers;

public class ConfirmarController : Controller
{
    [HttpGet]
    public IActionResult Confirmar()
    {
        return View(ObtenerCarrito());
    }

    [HttpPost]
    public IActionResult Confirmar(string nombre, string telefono, string direccion)
    {
        var carrito = ObtenerCarrito();

        if (!carrito.Vacio)
        {
            var historial = ObtenerHistorial();
            var nuevoPedido = new PedidoHistorial
            {
                Id = historial.Count == 0 ? 1 : historial.Max(h => h.Id) + 1,
                Fecha = DateTime.Now,
                Total = carrito.Total,
                NombreCliente = nombre,
                DireccionEntrega = direccion,
                Items = carrito.Items
                    .Select(i => new PedidoHistorialItem
                    {
                        IdPizza = i.IdPizza,
                        Nombre = i.Nombre,
                        Precio = i.Precio,
                        Cantidad = i.Cantidad
                    })
                    .ToList()
            };

            historial.Insert(0, nuevoPedido);
            GuardarHistorial(historial);

            GuardarCarrito(new CarritoViewModel());
            TempData["MensajeExito"] = $"Pedido recibido. ¡Gracias {nombre}! Lo enviamos a {direccion}.";
            return RedirectToAction(nameof(SeguimientoController.Seguimiento), "Seguimiento", new { id = nuevoPedido.Id, paso = 1 });
        }

        GuardarCarrito(new CarritoViewModel());
        return RedirectToAction(nameof(MenuController.Menu), "Menu");
    }

    private CarritoViewModel ObtenerCarrito()
        => HttpContext.Session.Get<CarritoViewModel>(SessionKeys.Carrito) ?? new CarritoViewModel();

    private void GuardarCarrito(CarritoViewModel carrito)
        => HttpContext.Session.Set(SessionKeys.Carrito, carrito);

    private List<PedidoHistorial> ObtenerHistorial()
        => HttpContext.Session.Get<List<PedidoHistorial>>(SessionKeys.HistorialPedidos) ?? new List<PedidoHistorial>();

    private void GuardarHistorial(List<PedidoHistorial> historial)
        => HttpContext.Session.Set(SessionKeys.HistorialPedidos, historial);
}

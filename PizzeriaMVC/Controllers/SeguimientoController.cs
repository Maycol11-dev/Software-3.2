using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Controllers;

public class SeguimientoController : Controller
{
    [HttpGet]
    public IActionResult Seguimiento(int id, int paso = 1)
    {
        var estados = new (string Nombre, string Emoji)[]
        {
            ("Espera de confirmación", "🕐"),
            ("En preparación", "👨‍🍳"),
            ("En viaje", "🛵"),
            ("Entregado", "🍕")
        };

        var modelo = new SeguimientoViewModel
        {
            IdPedido = id,
            Total = ObtenerCarrito().Total > 0 ? ObtenerCarrito().Total : 54700,
            DireccionEntrega = "Av. Siempre Viva 123",
            Pasos = estados
                .Select((e, i) => new EstadoPedidoStep
                {
                    Nombre = e.Nombre,
                    Emoji = e.Emoji,
                    Completo = i + 1 < paso,
                    Activo = i + 1 == paso
                })
                .ToList()
        };

        return View(modelo);
    }

    private CarritoViewModel ObtenerCarrito()
        => HttpContext.Session.Get<CarritoViewModel>(SessionKeys.Carrito) ?? new CarritoViewModel();
}

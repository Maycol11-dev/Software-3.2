using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Data;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Controllers;

public class PedidosController : Controller
{
    private const string CarritoKey = "Carrito";

    [HttpGet]
    public IActionResult Menu()
    {
        var modelo = new MenuViewModel
        {
            Pizzas = MenuMock.Pizzas,
            Carrito = ObtenerCarrito()
        };

        return View(modelo);
    }

    [HttpPost]
    public IActionResult Agregar(int id, int cantidad = 1)
    {
        var carrito = ObtenerCarrito();

        var existente = carrito.Items.FirstOrDefault(i => i.IdPizza == id);
        if (existente is not null)
        {
            existente.Cantidad += Math.Max(1, cantidad);
        }
        else
        {
            var pizza = MenuMock.Pizzas.FirstOrDefault(p => p.IdPizza == id);
            if (pizza is not null)
            {
                carrito.Items.Add(new CarritoItem
                {
                    IdPizza = pizza.IdPizza,
                    Nombre = pizza.Nombre,
                    Precio = pizza.Precio,
                    Cantidad = Math.Max(1, cantidad)
                });
            }
        }

        GuardarCarrito(carrito);
        return RedirectToAction(nameof(Menu));
    }

    [HttpPost]
    public IActionResult Quitar(int id)
    {
        var carrito = ObtenerCarrito();
        carrito.Items.RemoveAll(i => i.IdPizza == id);
        GuardarCarrito(carrito);
        return RedirectToAction(nameof(Menu));
    }

    [HttpGet]
    public IActionResult Confirmar()
    {
        return View(ObtenerCarrito());
    }

    [HttpPost]
    public IActionResult Confirmar(string nombre, string telefono, string direccion)
    {
        GuardarCarrito(new CarritoViewModel());
        TempData["MensajeExito"] = $"Pedido recibido. ¡Gracias {nombre}! Lo enviamos a {direccion}.";
        return RedirectToAction(nameof(Seguimiento), new { id = 123, paso = 2 });
    }

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
    {
        return HttpContext.Session.Get<CarritoViewModel>(CarritoKey) ?? new CarritoViewModel();
    }

    private void GuardarCarrito(CarritoViewModel carrito)
    {
        HttpContext.Session.Set(CarritoKey, carrito);
    }
}

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
    }

    public static T? Get<T>(this ISession session, string key)
    {
        var data = session.GetString(key);
        return data is null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(data);
    }
}
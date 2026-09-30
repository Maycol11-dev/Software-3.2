using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Data;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Controllers;

public class PedidosController : Controller
{
    private const string CarritoKey = "Carrito";
    private const string HistorialKey = "HistorialPedidos";

    [HttpGet]
    public IActionResult Menu()
    {
        var carrito = ObtenerCarrito();

        // Guardar aunque esté vacío fuerza a ASP.NET Core a emitir la cookie de sesión
        // en la primera visita; si no, cada request abriría una sesión nueva.
        GuardarCarrito(carrito);

        var modelo = new MenuViewModel
        {
            Pizzas = MenuMock.Pizzas,
            Carrito = carrito
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
    public IActionResult Incrementar(int id)
    {
        var carrito = ObtenerCarrito();
        var existente = carrito.Items.FirstOrDefault(i => i.IdPizza == id);

        if (existente is not null)
        {
            existente.Cantidad++;
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
                    Cantidad = 1
                });
            }
        }

        GuardarCarrito(carrito);
        return Json(RespuestaCarrito(carrito));
    }

    [HttpPost]
    public IActionResult Decrementar(int id)
    {
        var carrito = ObtenerCarrito();
        var existente = carrito.Items.FirstOrDefault(i => i.IdPizza == id);

        if (existente is not null)
        {
            existente.Cantidad--;
            if (existente.Cantidad <= 0)
            {
                carrito.Items.Remove(existente);
            }
        }

        GuardarCarrito(carrito);
        return Json(RespuestaCarrito(carrito));
    }

    [HttpGet]
    public IActionResult Carrito()
    {
        return PartialView("_Carrito", ObtenerCarrito());
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
        }

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

        return RedirectToAction(nameof(Menu));
    }

    private CarritoViewModel ObtenerCarrito()
        => HttpContext.Session.Get<CarritoViewModel>(CarritoKey) ?? new CarritoViewModel();

    private object RespuestaCarrito(CarritoViewModel carrito)
    {
        return new
        {
            cantidades = carrito.Items.ToDictionary(i => i.IdPizza, i => i.Cantidad),
            total = carrito.Total
        };
    }

    private void GuardarCarrito(CarritoViewModel carrito)
        => HttpContext.Session.Set(CarritoKey, carrito);

    private List<PedidoHistorial> ObtenerHistorial()
        => HttpContext.Session.Get<List<PedidoHistorial>>(HistorialKey) ?? new List<PedidoHistorial>();

    private void GuardarHistorial(List<PedidoHistorial> historial)
        => HttpContext.Session.Set(HistorialKey, historial);
}

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
        => session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));

    public static T? Get<T>(this ISession session, string key)
    {
        var data = session.GetString(key);
        return data is null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(data);
    }
}

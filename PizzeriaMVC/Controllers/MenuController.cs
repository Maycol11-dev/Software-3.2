using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Data;
using PizzeriaMVC.Models;

namespace PizzeriaMVC.Controllers;

public class MenuController : Controller
{
    [HttpGet]
    public IActionResult Menu()
    {
        var carrito = ObtenerCarrito();

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

    private CarritoViewModel ObtenerCarrito()
        => HttpContext.Session.Get<CarritoViewModel>(SessionKeys.Carrito) ?? new CarritoViewModel();

    private object RespuestaCarrito(CarritoViewModel carrito)
    {
        return new
        {
            cantidades = carrito.Items.ToDictionary(i => i.IdPizza, i => i.Cantidad),
            total = carrito.Total
        };
    }

    private void GuardarCarrito(CarritoViewModel carrito)
        => HttpContext.Session.Set(SessionKeys.Carrito, carrito);
}

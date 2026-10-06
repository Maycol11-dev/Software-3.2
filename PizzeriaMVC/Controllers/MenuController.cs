using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

public class MenuController : Controller
{
    private readonly ApiPizzeria _api;
    private readonly CarritoService _carrito;

    public MenuController(ApiPizzeria api, CarritoService carrito)
    {
        _api = api;
        _carrito = carrito;
    }

    [HttpGet]
    public async Task<IActionResult> Menu()
    {
        var carrito = await _carrito.ObtenerHidratado(HttpContext.Session);

        var pizzas = await _api.GetPizzasAsync();

        var modelo = new MenuViewModel
        {
            Pizzas = pizzas,
            Carrito = carrito
        };

        return View(modelo);
    }

    [HttpPost]
    public IActionResult Agregar(int id, int cantidad = 1)
    {
        _carrito.Agregar(HttpContext.Session, id, Math.Max(1, cantidad));
        return RedirectToAction(nameof(Menu));
    }

    [HttpPost]
    public async Task<IActionResult> Incrementar(int id)
    {
        _carrito.Sumar(HttpContext.Session, id, 1);
        return Json(await RespuestaCarrito());
    }

    [HttpPost]
    public async Task<IActionResult> Decrementar(int id)
    {
        _carrito.Sumar(HttpContext.Session, id, -1);
        return Json(await RespuestaCarrito());
    }

    [HttpGet]
    public async Task<IActionResult> Carrito()
    {
        return PartialView("_Carrito", await _carrito.ObtenerHidratado(HttpContext.Session));
    }

    [HttpPost]
    public IActionResult Quitar(int id)
    {
        _carrito.Quitar(HttpContext.Session, id);
        return RedirectToAction(nameof(Menu));
    }

    private async Task<object> RespuestaCarrito()
    {
        var carrito = await _carrito.ObtenerHidratado(HttpContext.Session);

        return new
        {
            cantidades = carrito.Items.ToDictionary(i => i.IdPizza, i => i.Cantidad),
            total = carrito.Total
        };
    }
}
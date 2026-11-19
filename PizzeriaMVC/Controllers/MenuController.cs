using Microsoft.AspNetCore.Mvc;
using PizzeriaMVC.Models;
using PizzeriaMVC.Services;

namespace PizzeriaMVC.Controllers;

public class MenuController : Controller
{
    private readonly ApiPizzeria _api;
    private readonly CarritoService _carrito;
    private readonly IWebHostEnvironment _env;

    public MenuController(ApiPizzeria api, CarritoService carrito, IWebHostEnvironment env)
    {
        _api = api;
        _carrito = carrito;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<PizzaMenuItem> pizzas;
        try
        {
            pizzas = await _api.GetPizzasAsync();
        }
        catch (HttpRequestException)
        {
            pizzas = new List<PizzaMenuItem>();
            TempData["MensajeError"] = "No se pudo conectar con el servicio de pizzas. Verificá que la API esté iniciada.";
        }

        var carrito = await _carrito.ObtenerHidratado(HttpContext.Session);

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
        return RedirectToAction(nameof(Index));
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
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Nueva()
    {
        if (!User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        return View("Form", new PizzaFormViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        if (!User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        var pizza = await _api.GetPizzaPorIdAsync(id);
        if (pizza is null)
        {
            TempData["MensajeError"] = "La pizza solicitada no existe.";
            return RedirectToAction(nameof(Index));
        }

        var modelo = new PizzaFormViewModel
        {
            IdPizza = pizza.IdPizza,
            Nombre = pizza.Nombre,
            Descripcion = pizza.Descripcion,
            Precio = pizza.Precio,
            ImagenUrl = pizza.ImagenUrl
        };

        return View("Form", modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(PizzaFormViewModel modelo)
    {
        if (!User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", modelo);
        }

        try
        {
            if (modelo.ArchivoImagen is not null && modelo.ArchivoImagen.Length > 0)
            {
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(modelo.ArchivoImagen.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(ext))
                {
                    ModelState.AddModelError("ArchivoImagen", "Solo se permiten imágenes (.jpg, .jpeg, .png, .webp).");
                    return View("Form", modelo);
                }

                var carpetaPizzas = Path.Combine(_env.WebRootPath, "images", "pizzas");
                if (!Directory.Exists(carpetaPizzas))
                {
                    Directory.CreateDirectory(carpetaPizzas);
                }

                var nombreArchivo = $"{Guid.NewGuid():N}{ext}";
                var rutaDestino = Path.Combine(carpetaPizzas, nombreArchivo);

                using (var stream = new FileStream(rutaDestino, FileMode.Create))
                {
                    await modelo.ArchivoImagen.CopyToAsync(stream);
                }

                modelo.ImagenUrl = $"/images/pizzas/{nombreArchivo}";
            }
            else if (string.IsNullOrWhiteSpace(modelo.ImagenUrl))
            {
                modelo.ImagenUrl = "/images/Inicio.jpg";
            }

            if (modelo.EsEdicion)
            {
                await _api.ActualizarPizzaAsync(modelo);
                TempData["MensajeExito"] = $"Pizza \"{modelo.Nombre}\" actualizada correctamente.";
            }
            else
            {
                await _api.CrearPizzaAsync(modelo);
                TempData["MensajeExito"] = $"Pizza \"{modelo.Nombre}\" creada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Form", modelo);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo conectar con el servidor para guardar la pizza.");
            return View("Form", modelo);
        }
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